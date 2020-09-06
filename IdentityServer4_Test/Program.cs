using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace IdentityServer4_Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "IdentityServer4";

            //Log.Logger = new LoggerConfiguration()
            //    .MinimumLevel.Debug()
            //    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            //    .MinimumLevel.Override("System", LogEventLevel.Warning)
            //    .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
            //    .Enrich.FromLogContext()
            //    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate)
            //    .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //.UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
