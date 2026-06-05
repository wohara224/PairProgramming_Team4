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
    static string deviceUrl = "http://172.16.7.10:50080/"; // サーバーURL

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
                return Grades.ViewGrades;
            }

            // API通信と表示
            try
            {
                var student = await client.GetFromJsonAsync<StudentGrade>($"{deviceUrl}/api/performance?student={studentId}");

                Console.SetCursorPosition(0, 2);
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine("");
                }
                Console.SetCursorPosition(0, 3);

                Console.WriteLine("");
                Console.WriteLine($"名前：{student.Name,5}");
                Console.WriteLine("----------------------------------");
                foreach (var subject in student.Subjects)
                {
                    Console.WriteLine($"{subject.Name,5}:{subject.Score,4}点");
                }
            }
            catch (Exception ex)
            {

            }
            

            

            Console.SetCursorPosition(0, 0);
        }

    }

    // 成績追加画面
    static async Task<Grades> AddScene()
    {
        Console.Clear();



        return Grades.Title;
    }

    // 成績ランキング画面
    static async Task<Grades> RankingScene()
    {
        Console.Clear();

        //Console.WriteLine("0：戻る 1：閲覧 2：登録");
        //Console.Write("操作入力＞");
        //string? command = Console.ReadLine();
        Thread.Sleep(300);
        return Grades.Title;
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




