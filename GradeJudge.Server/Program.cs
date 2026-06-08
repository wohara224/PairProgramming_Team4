using GradeJudge.Server.Model;
using GradeJudge.Server.Repository;
using NLog;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

// =============================
//   設定
// =============================

// リポジトリ
IGradeRepository gradeRepository = new GradeRepository();

// JSON書き込み時の設定
var writeOptions = new JsonSerializerOptions
{
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // Unicode表記から日本語そのままにする
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase // キャメルケースに変換
};

// JSON読み取り時の設定
var readOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
};

// ロガー設定
var sysLogger = LogManager.GetLogger("GradeJudge.Server.System"); // メイン操作用
var apiLogger = LogManager.GetLogger("GradeJudge.Server.Api"); // 通信ログ

// =============================
//   サーバー起動
// =============================

/* システムログ：アプリ起動 */
sysLogger.Info("アプリケーション起動");

string portView = "   ポート番号: 8080";
HttpListener listener = new();
listener.Prefixes.Add("http://+:8080/api/");

try
{
    listener.Start();

    /* システムログ：リスナー起動 */
    sysLogger.Info("リスナー起動：Port 8080");

}
catch (Exception)
{
    /* 通常起動できなかった場合 */
    listener.Close();

    listener = new();
    listener.Prefixes.Add("http://localhost:8080/api/");
    try
    {
        listener.Start();
    }
    catch (Exception ex)
    {
        /* 完全に起動できなかった場合 */
        listener.Close();

        // 表示
        Console.WriteLine($"サーバーが起動できませんでした。：Error={ex.Message}");

        /* システムログ：起動失敗 */
        sysLogger.Fatal(ex, "サーバー起動失敗");

        Environment.ExitCode = 1;
        Console.WriteLine("アプリケーションを終了します．．．");

        /* システムログ：アプリ終了 */
        sysLogger.Info("アプリケーション終了：Exit={0}", Environment.ExitCode);

        return;
    }

    // 表示用文字列を修正
    portView = "   ポート番号: 8080 (localhost)";

    /* システムログ：リスナー起動 */
    sysLogger.Info("リスナー起動：Port 8080");
    /* システムログ：ローカルホスト */
    sysLogger.Warn("localhostのみ受け付け");

}

// ヘッダー表示
Console.WriteLine("=========================================");
Console.WriteLine("【成績管理スタブ】が起動しました");
Console.WriteLine($"{portView}");
Console.WriteLine("=========================================");
Console.WriteLine("ペアのPC（クライアント）からの通信を待っています...\n");

// システムログ
sysLogger.Info("リスナー起動完了");

// =============================
//  リクエスト受信待機
// =============================

// 正常な終了を定義する
CancellationTokenSource cts = new();
Console.CancelKeyPress += (_, e) => 
{
    e.Cancel = true; // 即時終了はしない
    cts.Cancel(); // キャンセルオブジェクトにキャンセル要求を発行
    listener.Stop(); // リスナーを止める
};
var token = cts.Token; // キャンセル要求のプロパティ

// リクエスト受信開始
sysLogger.Info("リクエスト受信開始");

try
{
    while (!token.IsCancellationRequested)
    {
        HttpListenerContext context = listener.GetContext();
        HttpListenerRequest request = context.Request;
        HttpListenerResponse response = context.Response;

        // パスを解析してレスポンス生成
        IPAddress? remoteIp = request.RemoteEndPoint.Address.MapToIPv4();
        string? path = request.Url?.AbsolutePath;
        string? query = request.Url?.Query;
        string? method = request.HttpMethod;
        HttpStatusCode statusCode = HttpStatusCode.OK;

        apiLogger.Info("リクエスト受信：IP={0} PATH={1}{2} Method={3}", remoteIp, path, query, method);
        switch (path)
        {
            case "/api/register":
                /* 成績登録 */

                // POSTじゃない場合
                if (!string.Equals(method, "POST"))
                {
                    /* レスポンス生成・送信 */
                    statusCode = HttpStatusCode.MethodNotAllowed;
                    SendErrorResponse(response, statusCode);

                    /* 通信ログ：リクエスト異常 */
                    apiLogger.Warn("リクエスト異常：不許可メソッドを受信");

                    break;
                }

                // リクエストの読み取り
                string json;
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    json = reader.ReadToEnd();
                }

                // デシリアライズ：Json -> C#クラス
                RegisterRequest? inputData;
                try
                {
                    inputData = JsonSerializer.Deserialize<RegisterRequest>(json, readOptions)
                        ?? throw new JsonException();
                }
                catch (JsonException)
                {
                    /* JSON->C#クラスへのデシリアライズに失敗した場合 */

                    /* レスポンス生成・送信 */
                    statusCode = HttpStatusCode.BadRequest;
                    SendErrorResponse(response, statusCode, ["INVALID_REQUEST"]);

                    /* 通信ログ：リクエスト異常 */
                    apiLogger.Warn("リクエスト異常：ErrorJson={0}", json);

                    break;
                }

                // 入力値のバリデーションチェック
                var studentExists = gradeRepository.StudentExists(inputData.StudentId);
                var subjectExists = gradeRepository.SubjectExists(inputData.SubjectId);
                var scoreValid = gradeRepository.IsValidScore(inputData.Score);

                // バリデーションエラー
                if (!studentExists || !subjectExists || !scoreValid)
                {
                    List<string> errorMessages = [];
                    if (!studentExists)
                    {
                        /* レスポンス用オブジェクト追加 */
                        errorMessages.Add("STUDENT_NOT_EXIST");

                        /* 通信ログ：生徒ID不正 */
                        apiLogger.Warn("リクエスト異常：生徒ID不正 ID={0}", inputData.StudentId);
                    }
                    if (!subjectExists)
                    {
                        /* レスポンス用オブジェクト追加 */
                        errorMessages.Add("SUBJECT_NOT_EXIST");

                        /* 通信ログ：科目ID不正 */
                        apiLogger.Warn("リクエスト異常：科目ID不正 ID={0}", inputData.SubjectId);

                    }
                    if (!scoreValid)
                    {
                        /* レスポンス用オブジェクト追加 */
                        errorMessages.Add("SCORE_OUT_OF_RANGE");

                        /* 通信ログ：点数範囲不正 */
                        apiLogger.Warn("リクエスト異常：点数範囲不正 Score={0}", inputData.Score);
                    }

                    /* レスポンス生成・送信 */
                    statusCode = HttpStatusCode.BadRequest;
                    SendErrorResponse(response, statusCode, [.. errorMessages]);

                    break;
                }

                /* レスポンス生成・送信 */
                gradeRepository.Register(inputData.StudentId, inputData.SubjectId, inputData.Score);
                SendResponse(response, statusCode);

                /* 通信ログ：正常 */
                apiLogger.Info("リクエスト正常");

                break;
            case "/api/scores/student":
                /* 個人別スコア取得 */

                // GETじゃない場合
                if (!string.Equals(method, "GET"))
                {
                    /* レスポンス生成・送信 */
                    statusCode = HttpStatusCode.MethodNotAllowed;
                    SendErrorResponse(response, statusCode);

                    /* 通信ログ：リクエスト異常 */
                    apiLogger.Warn("リクエスト異常：不許可メソッドを受信");

                    break;
                }

                // idクエリがない、数値変換できない場合
                if (!int.TryParse(request.QueryString["id"], out int studentId))
                {
                    /* レスポンス生成・送信 */
                    statusCode = HttpStatusCode.BadRequest;
                    SendErrorResponse(response, statusCode, ["INVALID_REQUEST"]);

                    /* 通信ログ：クエリ不正 */
                    apiLogger.Warn("リクエスト異常：クエリ不正 Query={0}", query);

                    break;
                }

                // データ取得 => データ取得できない場合NotFound
                var studentScores = gradeRepository.GetStudentScores(studentId);
                if (studentScores is null)
                {
                    /* レスポンス生成・送信 */
                    statusCode = HttpStatusCode.NotFound;
                    SendErrorResponse(response, statusCode, ["STUDENT_NOT_FOUND"]);

                    /* 通信ログ：生徒ID不正 */
                    apiLogger.Warn("リクエスト異常：生徒ID不正 ID={0}", studentId);

                    break;
                }

                /* レスポンス生成・送信 */
                {
                    byte[] jsonResponse = JsonSerializer.SerializeToUtf8Bytes(studentScores, writeOptions);
                    SendResponse(response, statusCode, jsonResponse);
                }

                /* 通信ログ：正常 */
                apiLogger.Info("リクエスト正常");

                break;
            case "/api/scores/subject":
                /* 科目別成績リスト取得 */

                // GETじゃない場合
                if (!string.Equals(method, "GET"))
                {
                    /* レスポンス生成・送信 */
                    statusCode = HttpStatusCode.MethodNotAllowed;
                    SendErrorResponse(response, statusCode);

                    /* 通信ログ：リクエスト異常 */
                    apiLogger.Warn("リクエスト異常：不許可メソッドを受信");

                    break;
                }

                // idクエリがない、数値変換できない場合
                if (!int.TryParse(request.QueryString["id"], out int subjectId))
                {
                    /* レスポンス生成・送信 */
                    statusCode = HttpStatusCode.BadRequest;
                    SendErrorResponse(response, statusCode, ["INVALID_REQUEST"]);

                    /* 通信ログ：クエリ不正 */
                    apiLogger.Warn("リクエスト異常：クエリ不正 Query={0}", query);

                    break;
                }

                // データ取得 => データ取得できない場合NotFound
                var subjectScores = gradeRepository.GetSubjectScores(subjectId);
                if (subjectScores is null)
                {
                    /* レスポンス生成・送信 */
                    statusCode = HttpStatusCode.NotFound;
                    SendErrorResponse(response, statusCode, ["SUBJECT_NOT_FOUND"]);

                    /* 通信ログ：生徒ID不正 */
                    apiLogger.Warn("リクエスト異常：生徒ID不正 ID={0}", subjectId);

                    break;
                }

                /* レスポンス生成・送信 */
                {
                    byte[] jsonResponse = JsonSerializer.SerializeToUtf8Bytes(subjectScores, writeOptions);
                    SendResponse(response, statusCode, jsonResponse);
                }

                /* 通信ログ：正常 */
                apiLogger.Info("リクエスト正常");

                break;
            default:
                /* 指定外URLへのアクセス */

                /* レスポンス生成・送信 */
                statusCode = HttpStatusCode.NotFound;
                SendErrorResponse(response, statusCode);

                /* 通信ログ：リクエスト異常 */
                apiLogger.Warn("リクエスト異常：URL不正");

                break;
        }
        ;

        // 通信履歴を表示
        Console.WriteLine(GetApiLog(
            DateTime.Now,
            remoteIp.ToString(),
            method,
            $"{path}{query}",
            $"{(int)statusCode} {statusCode}"));

        /* 通信ログ：レスポンス送信 */
        apiLogger.Info("レスポンス送信：Status={0}", (int)statusCode);
    }
}
catch (Exception ex) when (
    (ex is HttpListenerException || ex is OperationCanceledException)
    && token.IsCancellationRequested)
{
    /* キャンセル入力：正常終了扱い */
}
finally
{
    // リスナー停止
    listener.Close();

    /* システムログ：リスナー停止 */
    sysLogger.Info("リスナー停止");

    Console.WriteLine("アプリケーションを終了します．．．");

    /* システムログ：アプリ終了 */
    sysLogger.Info("アプリケーション終了：Exit={0}", Environment.ExitCode);
}

// =============================
//   ローカル関数
// =============================

// レスポンスの送信
static void SendResponse(
    HttpListenerResponse response,
    HttpStatusCode code,
    byte[]? jsonResponse = null)
{
    response.StatusCode = (int)code;

    // レスポンスボディあれば書き込む
    if(jsonResponse is not null)
    {
        response.ContentEncoding = Encoding.UTF8;
        response.ContentType = "application/json; charset=utf-8";
        response.ContentLength64 = jsonResponse.Length; // データサイズをヘッダーに追記

        Stream output = response.OutputStream;
        output.Write(jsonResponse, 0, jsonResponse.Length);
    }
    else
    {
        response.ContentLength64 = 0; // ボディ無し
    }

    // 送信
    response.Close();
}

// 共通のエラー処理（単体）
void SendErrorResponse(
    HttpListenerResponse response,
    HttpStatusCode code,
    string[]? message = null)
{
    // メッセージがnullまたはリスト0件のとき
    if(message is null or { Length:0 })
    {
        // ボディ無しで送信
        SendResponse(response, code);

        return;
    }

    // Jsonの作成
    var errors = new ErrorResponse([.. message.Select(x => new ErrorItem(x))]);
    var jsonResponse = JsonSerializer.SerializeToUtf8Bytes(errors, writeOptions);
    
    // レスポンス送信処理
    SendResponse(response, code, jsonResponse);
}

// 通信履歴
static string GetApiLog(DateTime date, string? ip, string method, string? path, string? result) =>
    $"{date:yyyy-MM-dd HH:mm:ss} {ip, -15} {method, -7} {path, -32} {result}";
