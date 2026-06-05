using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Hosting;

using System.Net;

// 多重起動の防止
const string MutexName = @"Global\GradeJudgeUniqueUuidv7-019e983f-37e7-7eb9-9d03-d3d40b5b1894";

using var mutex = new Mutex(false, MutexName, out bool isNewInstance);

if (!isNewInstance)
{
    Console.WriteLine("アプリケーションはすでに起動しています。");
    Environment.ExitCode = 1; // 異常終了
    return;
}

// --- DIコンテナ ---
var builder = Host.CreateApplicationBuilder(args);

// サービスを登録
using var app = builder.Build();

// --- ロギングの設定 ---
builder.Logging.ClearProviders();
builder.UseNLog(); // ILoggerをNLog化する

// --- アプリ実行部 ---
var systemLogger = NLog.LogManager.GetCurrentClassLogger(); // NLog本体のロガー
try
{
    systemLogger.Info($"アプリケーション起動");

    await app.RunAsync();

    Environment.ExitCode = 0; // 正常終了
}
catch (Exception ex)
{
    Environment.ExitCode = 1; // 異常終了
    systemLogger.Fatal(ex, "実行中エラー発生");
}
finally
{
    systemLogger.Info("アプリケーション終了 ExitCode={0}", Environment.ExitCode);

    NLog.LogManager.Shutdown();
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

