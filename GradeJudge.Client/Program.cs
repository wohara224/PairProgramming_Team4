using System.Net.Http.Json;
//using NLog.Web;

public class Program
{
    static HttpClient client = new();
    static string deviceUrl = "https://172.16.7.10:50080/"; // サーバーURL

    static async Task Main()
    {
        try
        {
            var answer = await client.GetAsync(deviceUrl);
            Console.WriteLine(answer);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"接続失敗{ex.Message}");
        }
        
    }

    // 画面の列挙
    //enum Grades
    //{
    //    Title,
    //    ViewGrades,
    //    SortGrades,
    //    AddGrades,
    //    Exit
    //}

    //static async Task Main()
    //{
    //    Grades state = Grades.Title;

    //    while(state != Grades.Exit)
    //    {
    //        switch(state)
    //        {
    //            case Grades.Title:
    //                break;
    //            case Grades.ViewGrades: 
    //                break;

    //        }
    //    }
    //}
}

