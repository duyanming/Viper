using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Viper.GetWay
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var urls = Anno.Rpc.Server.ArgsValue.GetValueByName("-h", args);
                    webBuilder.UseStartup<Startup>();
                    if (!string.IsNullOrWhiteSpace(urls))
                    {
                        webBuilder.UseUrls(urls);
                    }
                    /**
                     * active 为prod 的时候关闭日志 None，默认日志级别 Information
                     */
                    var active = Anno.Rpc.Server.ArgsValue.GetValueByName("-active", args);
                    LogLevel logLevel = LogLevel.Information;
                    if (!string.IsNullOrWhiteSpace(active)&&active.Equals("prod"))
                    {
                        logLevel = LogLevel.None;
                    }
                    webBuilder
                    .UseAnnoSvc()
                        .ConfigureLogging(log => log.SetMinimumLevel(logLevel))
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseKestrel(option =>
                        {
                            option.Limits.MaxRequestBodySize = 200 * 1024 * 1000;
                        }).ConfigureServices(service =>
                        {
                            service.Configure<FormOptions>(options =>
                            {
                                options.MultipartBodyLengthLimit = long.MaxValue;
                            });
                        });
                });
    }
}
