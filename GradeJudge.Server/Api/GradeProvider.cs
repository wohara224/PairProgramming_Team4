using GradeJudge.Server.Domain.Dto;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;

namespace GradeJudge.Server.Api;


public partial class GradeProvider (ILogger<GradeProvider> logger) : BackgroundService
{
    // === 定数 ===
    private const string PrimaryPrefix = "http://+";
    private const string DefaultPrefix = "http://localhost";
    private const int PortNumber = 50080;

    private const string RegisterUri = "/api/register";
    private const string PerformanceUri = "/api/performance";
    private const string DropoutUri = "/api/dropout";
    private const string RankingUri = "/api/ranking";

    private const string StudentQuery = "student";
    private const string SubjectQuery = "subject";

    // === エラーコード ===
    private const string INVALID_REQUEST    = "INVALID_REQUEST";
    private const string STUDENT_NOT_EXIST  = "STUDENT_NOT_EXIST";
    private const string SUBJECT_NOT_EXIST  = "SUBJECT_NOT_EXIST";
    private const string SCORE_OUT_OF_RANGE = "SCORE_OUT_OF_RANGE";
    private const string INVALID_STUDENT_ID = "INVALID_STUDENT_ID";
    private const string INVALID_SUBJECT_ID = "INVALID_SUBJECT_ID";
    private const string NO_PERMISSION      = "NO_PERMISSION";
    private const string STUDENT_NOT_FOUND  = "STUDENT_NOT_FOUND";
    private const string SUBJECT_NOT_FOUND  = "SUBJECT_NOT_FOUND";


    // === フィールド ===
    private readonly HttpListener _listener = new();

    // === 起動メソッド ===
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // サーバー接続開始
        logger.LogInformation("サーバ起動開始");
        _listener.Prefixes.Add($"{PrimaryPrefix}/{PortNumber}");

        try
        {
            _listener.Start();
        }
        catch (Exception)
        {
            _listener.Prefixes.Clear();
            _listener.Prefixes.Add($"{DefaultPrefix}/{PortNumber}");

            try
            {
                // ローカルホストとしてサーバー起動
                _listener.Start();
            }
            catch (HttpListenerException ex)
            {
                // ファイヤーウォールやnetshの設定ズレなどでlocalhostすら接続できないことも想定
                logger.LogError(ex, $"サーバ起動失敗");
                throw; // そのままMainに例外をスロー
            }

            logger.LogWarning($"警告：ローカルホストでサーバ開設");
        }

        LogConnectionCompleted(PortNumber); // サーバ起動完了 Port=****

        // リクエスト待受け処理
        try
        {
            // キャンセルリクエストになっていない間実行
            while (!stoppingToken.IsCancellationRequested)
            {
                
            }
        }
        finally
        {
            logger.LogInformation("サーバ終了");
            _listener.Stop();
            _listener.Close();
        }
    }

    // === ヘルパー ===
    [LoggerMessage(Level = LogLevel.Information, Message = "サーバ起動完了 Port={port}")]
    private partial void LogConnectionCompleted(int port);

    // リクエスト処理
    private void ProcessRequest(HttpListenerContext context)
    {
        try
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            string path = request.Url?.AbsolutePath ?? "/";

            switch (path)
            {
                case RegisterUri:
                    RegisterGrade(request, response);
                    break;
                case PerformanceUri:
                    ReturnPerformance(request, response);
                    break;
                case DropoutUri:
                    ExtractDropoutStudents(request, response);
                    break;
                case RankingUri:
                    ReturnPerformance(request, response);
                    break;
                default:
                    // 403 Forbidden 権限なしとして処理
                    var error = new ErrorResponse(nameof(HttpStatusCode.Forbidden), NO_PERMISSION);
                    CreateForbiddenResponse(response, error);
                    /* ログ処理 */
                    /* 表示処理 */
                    break;
            }
        }
        catch
        {

        }
    }

    // === レスポンス生成 ===

    // 成績登録
    private void RegisterGrade(HttpListenerRequest request,HttpListenerResponse response)
    {

    }

    // 個人成績
    private void ReturnPerformance(HttpListenerRequest request, HttpListenerResponse response)
    {

    }

    // 落第者抽出
    private void ExtractDropoutStudents(HttpListenerRequest request, HttpListenerResponse response)
    {

    }

    // ランキング
    private void ReturnScoreRanking(HttpListenerRequest request, HttpListenerResponse response)
    {

    }

    // === エラー系レスポンス生成 ===

    // 400レスポンス生成
    private static void CreateBadRequestResponse(HttpListenerResponse response, ErrorResponse error)
    {

    }

    // 403レスポンス生成
    private static void CreateForbiddenResponse(HttpListenerResponse response, ErrorResponse error)
    {

    }

    // 404レスポンス生成
    private static void CreateNotFoundResponse(HttpListenerResponse response, ErrorResponse error)
    {

    }
}
