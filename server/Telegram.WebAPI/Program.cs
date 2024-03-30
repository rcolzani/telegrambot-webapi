using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Telegram.WebAPI;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) {
        Console.WriteLine("start server");
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                //webBuilder.UseSentry(options => {
                //    options.Dsn = "https://222379c8203048b094270b30cba7c6fd@o293815.ingest.sentry.io/5710142";
                //});                
                webBuilder.UseSentry();
                webBuilder.UseStartup<Startup>();
            });
    }
}

