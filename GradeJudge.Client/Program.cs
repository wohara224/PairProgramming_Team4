using GradeJudge.Client.Model;
using NLog;
using System.Net.Http.Json;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;



public partial class Program
{
    static HttpClient client = new();
    static string deviceUrl = "http://172.16.7.10:8080"; // サーバーURL
    // static string deviceUrl = "http://localhost:8080";

    // ロガー設定
    private static readonly Logger sysLogger = LogManager.GetLogger("GradeJudge.Client.System"); // メイン操作用
    private static readonly Logger apiLogger = LogManager.GetLogger("GradeJudge.Client.Api"); // 通信ログ


    // 画面の列挙
    enum Grades
    {
        Title,
        ViewGrades,
        AddGrades,
        RankingGrades,
        Exit
    }

    // メインループ
    static async Task Main()
    {
        // システムログ：アプリ起動 
        sysLogger.Info("アプリケーション起動");

        Grades state = Grades.Title;
        
        while (state != Grades.Exit)
        {
            switch (state)
            {
                case Grades.Title:
                    state = await TitleScene();
                    break;

                case Grades.ViewGrades:
                    state = await ViewScene();
                    break;

                case Grades.AddGrades:
                    state = await AddScene();
                    break;

                case Grades.RankingGrades:
                    state = await RankingScene();
                    break;


            }
        }

        Console.WriteLine("");
        Console.WriteLine("アプリケーションを終了します．．．");
        sysLogger.Info("アプリケーション終了");
    }

    //　初期画面
    static async Task<Grades> TitleScene()
    {
        // システムログ：初期画面
        sysLogger.Info("メニュー画面表示");

        Console.Clear();

        Console.WriteLine("0：終了 1：登録 2：個人成績閲覧 3：科目別ランキング");
        Console.Write("操作入力＞");
        string? command = Console.ReadLine();

        // 入力に応じて次の画面を返す
        switch (command)
        {
            case "0":
                return Grades.Exit;
            case "2":
                return Grades.ViewGrades;
            case "1":
                return Grades.AddGrades;
            case "3":
                return Grades.RankingGrades;
            default:
                return Grades.Title;
        }
    }

    // 成績閲覧画面
    static async Task<Grades> ViewScene()
    {
        // システムログ：成績閲覧画面
        sysLogger.Info("個人成績閲覧画面表示");

        Console.Clear();
        while (true)
        {
            Console.WriteLine("生徒ID入力　Esc：戻る");
            Console.Write("＞");

            string? studentId = Input(); // ID入力

            if(studentId == null)
            {
                return Grades.Title;
            }

            Console.SetCursorPosition(0, 1);
            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine(new string(' ', Console.WindowWidth));
            }
            Console.SetCursorPosition(0, 3);

            try
            {
                // API通信と表示
                apiLogger.Info("GET送信:URL={0}/api/scores/student?id={1}", deviceUrl, studentId);
                var response = await client.GetAsync($"{deviceUrl}/api/scores/student?id={studentId}");

                if (response.IsSuccessStatusCode)
                {
                    var student = await response.Content.ReadFromJsonAsync<StudentGrade>();

                    apiLogger.Info("正常レスポンス:{0}", response.StatusCode);

                    Console.WriteLine($"名前：{student.Name,5}");
                    Console.WriteLine("----------------------------------");
                    foreach (var subject in student.Subjects)
                    {
                        Console.WriteLine($"{subject.Name,-6}:{subject.Score,4}点");
                    }
                }
                else
                {
                    if(response.Content.Headers.ContentLength == 0)
                    {
                        apiLogger.Warn("異常レスポンス:{0}", response.StatusCode);
                        Console.WriteLine("エラーが発生しました");
                        Console.SetCursorPosition(0, 0);
                        continue;
                    }

                    var error =  await response.Content.ReadFromJsonAsync<ApiError>();

                    apiLogger.Warn("異常レスポンス:{0}", response.StatusCode);

                    foreach (var err in error.Errors)
                    {
                        apiLogger.Warn("Message:{0}", err?.Message);
                        switch (err.Message)
                        {
                            case "INVALID_REQUEST":
                                Console.WriteLine("生徒IDが不正です");
                                break;

                            case "STUDENT_NOT_FOUND":
                                Console.WriteLine("生徒が存在しません");
                                break;

                            default:
                                Console.WriteLine($"不明なエラー:{err.Message}");
                                break;
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                apiLogger.Warn("通信エラー:{0}", ex.Message);
                Console.WriteLine($"通信エラー: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                apiLogger.Warn("タイムアウト}");
                Console.WriteLine("タイムアウトしました");
            }
            //catch (Exception ex)
            //{
            //    apiLogger.Warn("予期しないエラー:{0}", ex.Message);
            //    Console.WriteLine($"予期しないエラー: {ex.Message}");
            //}

            Console.SetCursorPosition(0, 0);
        }
    }

    // 成績追加画面
    static async Task<Grades> AddScene()
    {
        // システムログ：成績追加画面
        sysLogger.Info("成績追加画面表示");

        Console.Clear();
        while (true)
        {
            Console.WriteLine("Esc：戻る");
            Console.WriteLine("");

            Console.Write("生徒ID＞");
            string? studentId = Input();
            if(studentId == null)
            {
                return Grades.Title;
            }

            Console.Write("科目ID＞");
            string? subjectId = Input();
            if (subjectId == null)
            {
                return Grades.Title;
            }

            Console.Write("点数＞");
            string? score = Input();
            if (score == null)
            {
                return Grades.Title;
            }

            Console.SetCursorPosition(0, 1);
            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine(new string(' ', Console.WindowWidth));
            }
            Console.SetCursorPosition(0, 6);

            try
            {
                // API通信と表示

                var request = new AddData
                {
                    StudentId = int.Parse(studentId),
                    SubjectId = int.Parse(subjectId),
                    Score = int.Parse(score)
                };

                apiLogger.Info("POST送信:URL={0}/api/scores/register StudentId={1},SubjectId={2},Score={3}", deviceUrl, request.StudentId,request.SubjectId,request.Score);
                var response = await client.PostAsJsonAsync($"{deviceUrl}/api/register", request);

                if (response.IsSuccessStatusCode)
                {
                    apiLogger.Info("正常レスポンス:{0}", response.StatusCode);

                    Console.WriteLine("成績の登録に成功しました");
                }
                else
                {
                    if (response.Content.Headers.ContentLength == 0)
                    {
                        apiLogger.Warn("異常レスポンス:{0}", response.StatusCode);
                        Console.WriteLine("エラーが発生しました");
                        Console.SetCursorPosition(0, 0);
                        continue;
                    }

                    var error = await response.Content.ReadFromJsonAsync<ApiError>();

                    apiLogger.Warn("異常レスポンス:{0}", response.StatusCode);

                    foreach (var err in error.Errors)
                    {
                        apiLogger.Warn("Message:{0}", err?.Message);
                        switch (err?.Message)
                        {
                            case "INVALID_REQUEST":
                                Console.WriteLine("リクエストが不正です");
                                break;

                            case "STUDENT_NOT_EXIST":
                                Console.WriteLine("生徒が存在しません");
                                break;

                            case "SUBJECT_NOT_EXIST":
                                Console.WriteLine("科目が存在しません");
                                break;

                            case "SCORE_OUT_OF_RANGE":
                                Console.WriteLine("点数が範囲外です");
                                break;

                            default:
                                Console.WriteLine("アクセス失敗");
                                break;
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                apiLogger.Warn("通信エラー:{0}", ex.Message);
                Console.WriteLine($"通信エラー: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                apiLogger.Warn("タイムアウト}");
                Console.WriteLine("タイムアウトしました");
            }
            catch (FormatException ex)
            {
                apiLogger.Warn("入力不正:{0}", ex.Message);
                Console.WriteLine($"入力が不正です: {ex.Message}");
            }

            Console.SetCursorPosition(0, 0);
        }
    }

    // 成績ランキング画面
    static async Task<Grades> RankingScene()
    {
        // システムログ：成績ランキング画面
        sysLogger.Info("ランキング画面表示");

        Console.Clear();
        while (true)
        {
            Console.WriteLine("科目ID入力　Esc：戻る");
            Console.Write("＞");

            string? subjectId = Input(); // ID入力

            if (subjectId == null)
            {
                return Grades.Title;
            }

            Console.SetCursorPosition(0, 1);
            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine(new string(' ', Console.WindowWidth));
            }
            Console.SetCursorPosition(0, 3);

            try
            {
                // API通信と表示
                apiLogger.Info("GET送信:URL={0}/api/scores/subject?id={1}", deviceUrl, subjectId);
                var response = await client.GetAsync($"{deviceUrl}/api/scores/subject?id={subjectId}");

                if (response.IsSuccessStatusCode)
                {
                    var subject = await response.Content.ReadFromJsonAsync<SubjectGrade>();

                    apiLogger.Info("正常レスポンス:{0}", response.StatusCode);

                    var top5 = subject.Students.OrderByDescending(s => s.Score).Take(5).ToList();


                    Console.WriteLine($"名前：{subject.Name,5}");
                    Console.WriteLine("----------------------------------");
                    foreach (var student in top5)
                    {
                        Console.WriteLine($"{student.Name,-16}:{student.Score,4}点");
                    }
                }
                else
                {
                    if (response.Content.Headers.ContentLength == 0)
                    {
                        apiLogger.Warn("異常レスポンス:{0}", response.StatusCode);
                        Console.WriteLine("エラーが発生しました");
                        Console.SetCursorPosition(0, 0);
                        continue;
                    }

                    var error = await response.Content.ReadFromJsonAsync<ApiError>();

                    apiLogger.Warn("異常レスポンス:{0}", response.StatusCode);

                    foreach (var err in error.Errors)
                    {
                        apiLogger.Warn("Message:{0}", err?.Message);
                        switch (err?.Message)
                        {
                            case "INVALID_REQUEST":
                                Console.WriteLine("科目IDが不正です");
                                break;

                            case "SUBJECT_NOT_FOUND":
                                Console.WriteLine("科目が存在しません");
                                break;

                            default:
                                Console.WriteLine("アクセス失敗");
                                break;
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                apiLogger.Warn("通信エラー:{0}", ex.Message);
                Console.WriteLine($"通信エラー: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                apiLogger.Warn("タイムアウト}");
                Console.WriteLine("タイムアウトしました");
            }
            //catch (Exception ex)
            //{
            //    apiLogger.Warn("予期しないエラー:{0}", ex.Message);
            //    Console.WriteLine($"予期しないエラー: {ex.Message}");
            //}

            Console.SetCursorPosition(0, 0);
        }
    }

    static string? Input()
    {
        string input = "";

        while (true)
        {
            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Escape)
            {
                return null;
            }

            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return input;
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                if (input.Length > 0)
                {
                    input = input[..^1];
                    Console.Write("\b \b");
                }

                continue;
            }

            if (char.IsDigit(key.KeyChar))
            {
                input += key.KeyChar;
                Console.Write(key.KeyChar);
            }
        }
    }

}




