using System.Net;

HttpListener listener = new();
listener.Prefixes.Add("http://+:50080/");

try
{
    listener.Start();
    Console.WriteLine("サーバーを開設しました");


    while (true)
    {
        HttpListenerContext context = await listener.GetContextAsync();

        Console.WriteLine($"処理実行");
        _ = Task.Run(() => ProcessRequest(context)); // 並列実行
    }
}
catch (Exception ex)
{
    Console.WriteLine($"エラー発生: {ex.Message}");
    listener.Prefixes.Clear();
    listener.Prefixes.Add("http://localhost:8080/");
    listener.Start();
}
finally
{
    listener.Close();
}



static void ProcessRequest(HttpListenerContext context)
{
    try
    {
        HttpListenerRequest request = context.Request;
        HttpListenerResponse response = context.Response;

        if(request.HttpMethod == "GET")
        {
            Console.WriteLine("GETを受信した。");

            response.StatusCode = (int)HttpStatusCode.OK;
        }
        else
        {
            Console.WriteLine("不正なHTTPメソッドを受信した。");

            response.StatusCode = (int)HttpStatusCode.BadRequest;

        }

        response.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"エラー発生: {ex.Message}");
    }
}

