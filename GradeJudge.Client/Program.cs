using System.Net.Http.Json;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using GradeJudge.Client.Model;

//using NLog.Web;

public class Program
{
    static HttpClient client = new();
    //static string deviceUrl = "http://172.16.7.10:50080/"; // サーバーURL
    static string deviceUrl = "http://localhost:50080/";

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
    }

    //　初期画面
    static async Task<Grades> TitleScene()
    {
        Console.Clear();

        Console.WriteLine("0：終了 1：閲覧 2：登録");
        Console.Write("操作入力＞");
        string? command = Console.ReadLine();

        // 入力に応じて次の画面を返す
        switch (command)
        {
            case "0":
                return Grades.Exit;
            case "1":
                return Grades.ViewGrades;
            case "2":
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

            // API通信と表示
            var response = await client.GetAsync($"{deviceUrl}/api/performance?student={studentId}");

            Console.SetCursorPosition(0, 1);
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(new string(' ', Console.WindowWidth));
            }
            Console.SetCursorPosition(0, 3);

            if (response.IsSuccessStatusCode)
            {
                var student = await response.Content.ReadFromJsonAsync<StudentGrade>();

                Console.WriteLine($"名前：{student.Name,5}");
                Console.WriteLine("----------------------------------");
                foreach (var subject in student.Subjects)
                {
                    Console.WriteLine($"{subject.Name,5}:{subject.Score,4}点");
                }
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<ApiError>();

                if (error?.Code == "INVALID_STUDENT_ID")
                {
                    Console.WriteLine("生徒IDが不正です");
                }
                else if(error?.Code == "STUDENT_NOT_FOUND")
                {
                    Console.WriteLine("生徒が存在しません");
                }
                else if (error?.Code == "NO_PERMISSION")
                {
                    Console.WriteLine("アクセス失敗");
                }
            }

            Console.SetCursorPosition(0, 0);
        }

    }

    // 成績追加画面
    static async Task<Grades> AddScene()
    {
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
                return Grades.ViewGrades;
            }

            Console.Write("点数＞");
            string? score = Input();
            if (score == null)
            {
                return Grades.ViewGrades;
            }

            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(new string(' ', Console.WindowWidth));
            }
            Console.SetCursorPosition(0, 6);

            // API通信と表示

            var request = new AddData
            {
                StudentId = int.Parse(studentId),
                SubjectId = int.Parse(subjectId),
                Score = int.Parse(score)
            };
            
            var response = await client.PostAsJsonAsync($"{deviceUrl}/api/register", request);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("成績の登録に成功しました");
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<ApiError>();

                if (error?.Code == "INVALID_REQUEST")
                {
                    Console.WriteLine("リクエストが不正です");
                }
                else if (error?.Code == "STUDENT_NOT_EXIST")
                {
                    Console.WriteLine("生徒が存在しません");
                }
                else if (error?.Code == "SUBJECT_NOT_EXIST")
                {
                    Console.WriteLine("科目が存在しません");
                }
                else if (error?.Code == "SCORE_OUT_OF_RANGE")
                {
                    Console.WriteLine("点数が範囲外です");
                }
                else if (error?.Code == "NO_PERMISSION")
                {
                    Console.WriteLine("アクセス失敗");
                }
            }

            Console.SetCursorPosition(0, 0);
        }
    }

    // 成績ランキング画面
    static async Task<Grades> RankingScene()
    {
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

            // API通信と表示
            var response = await client.GetAsync($"{deviceUrl}/api/ranking?subject={subjectId}");

            Console.SetCursorPosition(0, 1);
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(new string(' ', Console.WindowWidth));
            }
            Console.SetCursorPosition(0, 3);

            if (response.IsSuccessStatusCode)
            {
                var subject = await response.Content.ReadFromJsonAsync<SubjectGrade>();

                Console.WriteLine($"名前：{subject.Name,5}");
                Console.WriteLine("----------------------------------");
                foreach (var student in subject.Students)
                {
                    Console.WriteLine($"{student.Name,5}:{student.Score,4}点");
                }
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<ApiError>();

                if (error?.Code == "INVALID_SUBJECT_ID")
                {
                    Console.WriteLine("科目IDが不正です");
                }
                else if (error?.Code == "SUBJECT_NOT_FOUND")
                {
                    Console.WriteLine("科目が存在しません");
                }
                else if (error?.Code == "NO_PERMISSION")
                {
                    Console.WriteLine("アクセス失敗");
                }
            }

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




