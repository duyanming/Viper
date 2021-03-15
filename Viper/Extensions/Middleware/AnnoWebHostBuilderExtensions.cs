using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Microsoft.AspNetCore
{
    using Anno.EngineData;
    using Anno.Rpc.Client;
    using Viper.GetWay;
    using Anno.RateLimit;
    using System.Collections.Concurrent;
    using Anno.CronNET;

    using NetTools;

    /// <summary>
    /// 接入服务中心的HostBuilder中间件
    /// </summary>
    public static class AnnoWebHostBuilderExtensions
    {
        public static IWebHostBuilder UseAnnoSvc(this IWebHostBuilder hostBuilder)
        {
            if (hostBuilder == null)
            {
                throw new ArgumentNullException(nameof(hostBuilder));
            }
            // 检查是否已经加载过了
            if (hostBuilder.GetSetting(nameof(UseAnnoSvc)) != null)
            {
                return hostBuilder;
            }
            // 设置已加载标记，防止重复加载
            hostBuilder.UseSetting(nameof(UseAnnoSvc), true.ToString());
            // 添加configure处理
            hostBuilder.ConfigureServices(services =>
            {
                var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
                ViperConfig viperConfig = new ViperConfig();
                configuration.Bind(viperConfig);
                services.AddSingleton<ViperConfig>(viperConfig);
                services.AddSingleton<IStartupFilter>(new AnnoSetupFilter(viperConfig));
            });


            return hostBuilder;
        }
    }
    class AnnoSetupFilter : IStartupFilter
    {
        private static volatile ConcurrentDictionary<string, LimitInfo> _rateLimitPool = new ConcurrentDictionary<string, LimitInfo>();
        private static readonly CronDaemon CronDaemon = new CronDaemon();
        private readonly ViperConfig vierConfig = new ViperConfig();
        public AnnoSetupFilter(ViperConfig _viperConfig)
        {
            CronDaemon.AddJob("1 */10 * * * ? *", ClearLimit);
            CronDaemon.Start();
            this.vierConfig = _viperConfig;
        }
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.UseRouting();
                app.UseMiddleware<AnnoMiddleware>();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.Map("SysMg/Api", Api);
                });
                next(app);
            };
        }
        private Task Api(HttpContext context)
        {
            context.Response.ContentType = "Content-Type: application/javascript; charset=utf-8";
            Dictionary<string, string> input = new Dictionary<string, string>();
            #region 接收表单参数
            var Request = context.Request;
            var headers = Request.Headers;
            try
            {
                if (headers != null && headers.ContainsKey("X-Original-For"))
                {
                    input.Add("X-Original-For", headers["X-Original-For"].ToArray()[0]);
                }
                else
                {
                    input.Add("X-Original-For", $"{context.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "")}:{context.Connection.RemotePort}");
                }
            }
            finally
            {
                try
                {
                    if (Request.Method == "POST")
                    {
                        foreach (string k in Request.Form.Keys)
                        {
                            input.Add(k, Request.Form[k]);
                        }
                        foreach (IFormFile file in Request.Form.Files)
                        {
                            var fileName = file.Name;
                            if (string.IsNullOrWhiteSpace(fileName))
                            {
                                fileName = file.FileName;
                            }
                            input.TryAdd(fileName, file.ToBase64());
                        }
                    }
                }
                finally
                {
                    foreach (string k in Request.Query.Keys)
                    {
                        if (!input.ContainsKey(k))
                        {
                            input.Add(k, Request.Query[k]);
                        }
                    }
                }
            }
            #endregion
            #region 缓存--未完

            #endregion
            #region 限流
            if (RateLimit(context))
            {
                context.Response.StatusCode = 429;
                Dictionary<string, object> rlt = new Dictionary<string, object>();
                rlt.Add("status", false);
                rlt.Add("msg", "Trigger current limiting.");
                rlt.Add("output", null);
                rlt.Add("outputData", 429);
                var rltExec = System.Text.Encoding.UTF8.GetString(rlt.ExecuteResult());
                input.TryAdd("TraceId", Guid.NewGuid().ToString());
                input.TryAdd("GlobalTraceId", Guid.NewGuid().ToString());
                input.TryAdd("AppName", vierConfig.Target.AppName);
                input.TryAdd("AppNameTarget", vierConfig.Target.AppName);
                TracePool.EnQueue(TracePool.CreateTrance(input), FailMessage("Trigger current limiting.", false));
                return context.Response.WriteAsync(rltExec);
            }
            #endregion
            #region 处理
            ActionResult actionResult = null;
            try
            {
                //分发器
                var rlt = Connector.BrokerDns(input);
                actionResult = Newtonsoft.Json.JsonConvert.DeserializeObject<ActionResult>(rlt);

            }
            catch (Exception ex)
            {
                actionResult = new ActionResult()
                {
                    Msg = ex.Message,
                    Status = true
                };
            }
            #region 将OutPut # 开头的键 推到返回数据的根目录
            Dictionary<string, object> rltd = new Dictionary<string, object>();
            List<string> keys = new List<string>();
            rltd.Add("msg", actionResult.Msg);
            rltd.Add("status", actionResult.Status);
            if (actionResult.Output != null)
            {
                keys.AddRange(actionResult.Output.Keys);
                keys = keys.FindAll(k => k.Substring(0, 1) == "#");
                foreach (string key in keys)
                {
                    string newKey = key.Substring(1);
                    rltd.Add(newKey, actionResult.Output[key]);
                    actionResult.Output.Remove(key);
                }
                rltd.Add("output", actionResult.Output);
            }
            else
            {
                rltd.Add("output", null);
            }
            rltd.Add("outputData", actionResult.OutputData);
            #endregion
            #endregion
            return context.Response.WriteAsync(System.Text.Encoding.UTF8.GetString(rltd.ExecuteResult()));
        }
        /// <summary>
        /// 构建错误消息Json字符串
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="status">默认False</param>
        /// <returns>"{\"Msg\":\""+message+"\",\"Status\":false,\"Output\":null,\"OutputData\":null}"</returns>
        internal string FailMessage(string message, bool status = false)
        {
            return "{\"Msg\":\"" + message + "\",\"Status\":" + status.ToString().ToLower() +
                   ",\"Output\":null,\"OutputData\":null}";
        }
        /// <summary>
        /// 返回True代表受限流控制
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private bool RateLimit(HttpContext httpContext)
        {
            bool limit = false;
            if (vierConfig.Limit?.Enable == true)
            {
                var ip = httpContext.Connection.RemoteIpAddress.MapToIPv4();

                #region IpLimit

                if (vierConfig.Limit != null)
                {
                    if (IsBlackRateLimit(ip))//黑名单
                    {
                        return true;
                    }
                    if (IsWhiteRateLimit(ip))//白名单不限流
                    {
                        return false;
                    }
                    _rateLimitPool.TryGetValue(ip.ToString(), out LimitInfo limitInfo);
                    if (limitInfo == null)
                    {
                        //IP限流策略
                        Iplimit iplimit = MatchIpLimit(ip);
                        var limitingService = LimitingFactory.Build(TimeSpan.FromSeconds(iplimit.timeSpan)
                              , LimitingType.LeakageBucket
                              , (int)iplimit.rps
                              , (int)iplimit.limitSize);
                        limitInfo = new LimitInfo()
                        {
                            Time = DateTime.Now,
                            limitingService = limitingService
                        };
                        _rateLimitPool.TryAdd(ip.ToString(), limitInfo);
                    }
                    //ipLimit.Request() ==true 代表不受限制
                    limitInfo.Time = DateTime.Now;
                    limit = (limitInfo.limitingService.Request() == false);
                    if (limit)
                    {
#if DEBUG
                        Console.WriteLine($"IP:{ip},Trigger current limiting.");
#endif
                        return true;
                    }
                }
                #endregion
                #region TagLimit
                if (vierConfig.Limit.TagLimits != null)
                {
                    var taglimit = MatchTag(httpContext);
                    if (taglimit != null)
                    {
                        _rateLimitPool.TryGetValue($"{ taglimit.channel}.{ taglimit.router}", out LimitInfo limitInfo);
                        if (limitInfo == null)
                        {
                            var limitingService = LimitingFactory.Build(TimeSpan.FromSeconds(taglimit.timeSpan)
                                , LimitingType.LeakageBucket
                                , (int)taglimit.rps
                                , (int)taglimit.limitSize);
                            limitInfo = new LimitInfo()
                            {
                                Time = DateTime.Now,
                                limitingService = limitingService
                            };
                            _rateLimitPool.TryAdd($"{ taglimit.channel}.{ taglimit.router}", limitInfo);
                        }
                        //ipLimit.Request() ==true 代表不受限制
                        limitInfo.Time = DateTime.Now;
                        limit = (limitInfo.limitingService.Request() == false);
                    }
                    if (limit)
                    {
#if DEBUG
                        Console.WriteLine($"Tag:{taglimit.channel}.{taglimit.router},Trigger current limiting.");
#endif
                        return true;
                    }
                }
                #endregion

            }
            return limit;
        }
        /// <summary>
        /// Is黑名单限流策略
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private bool IsBlackRateLimit(System.Net.IPAddress ip)
        {
            if (vierConfig.Limit?.PolicyBlack == null)
            {
                return false;
            }
            foreach (var range in vierConfig.Limit.PolicyBlack)
            {
                if (range.Contains(ip))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Is 白名单限流策略
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private bool IsWhiteRateLimit(System.Net.IPAddress ip)
        {
            if (vierConfig.Limit?.PolicyWhite == null)
            {
                return false;
            }
            foreach (var range in vierConfig.Limit.PolicyWhite)
            {
                if (range.Contains(ip))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 匹配IP限流
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private Iplimit MatchIpLimit(System.Net.IPAddress ip)
        {
            Iplimit ipLimit = vierConfig.Limit?.DefaultIpLimit;
            foreach (var iplimit in vierConfig.Limit.IpLimits)
            {
                if (iplimit.IpRange.Contains(ip))
                {
                    return iplimit;
                }
            }
            return ipLimit;
        }
        /// <summary>
        /// 匹配标签限流
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private Taglimit MatchTag(HttpContext httpContext)
        {
            string channel = string.Empty, router = string.Empty;
            #region Get channel、router
            try
            {
                if (httpContext.Request.Method == "POST")
                {
                    foreach (string k in httpContext.Request.Form.Keys)
                    {
                        if (k == Anno.Const.Enum.Eng.NAMESPACE)
                        {
                            channel = httpContext.Request.Form[k];
                        }
                        else if (k == Anno.Const.Enum.Eng.CLASS)
                        {
                            router = httpContext.Request.Form[k];
                        }
                        if (channel != string.Empty && router != string.Empty)
                        {
                            break;
                        }
                    }
                }
            }
            finally
            {
                if (channel == string.Empty || router == string.Empty)
                {
                    foreach (string k in httpContext.Request.Query.Keys)
                    {
                        if (k == Anno.Const.Enum.Eng.NAMESPACE)
                        {
                            channel = httpContext.Request.Query[k];
                        }
                        else if (k == Anno.Const.Enum.Eng.CLASS)
                        {
                            router = httpContext.Request.Query[k];
                        }
                        if (channel != string.Empty && router != string.Empty)
                        {
                            break;
                        }
                    }
                }
            }
            #endregion
            var tags = vierConfig.Limit.TagLimits.ToList();
            var tags0 = tags.FirstOrDefault(t => t.channel == channel && t.router == router);
            if (tags0 != null)
            {
                return tags0;
            }
            var tags1 = tags.FirstOrDefault(t => t.channel == channel && t.router == "*");
            if (tags1 != null)
            {
                return tags1;
            }
            var tags2 = tags.FirstOrDefault(t => t.channel == "*" && t.router == router);
            if (tags2 != null)
            {
                return tags2;
            }
            var tags3 = tags.FirstOrDefault(t => t.channel == "*" && t.router == "*");
            if (tags3 != null)
            {
                return tags3;
            }
            return null;
        }
        private void ClearLimit()
        {
            DateTime time = DateTime.Now.AddMinutes(30);
            _rateLimitPool.ToList().ForEach(limit =>
            {
                if (limit.Value.Time < time)
                {
                    _rateLimitPool.TryRemove(limit.Key, out LimitInfo limitInfo);
                    limitInfo.limitingService.Dispose();
                }
            });
        }
    }

    class LimitInfo
    {
        public DateTime Time { get; set; } = DateTime.Now;
        public ILimitingService limitingService { get; set; }
    }
    class AnnoMiddleware
    {
        private readonly RequestDelegate _next;
        private ViperConfig viperConfig = new ViperConfig();
        public AnnoMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            configuration.Bind(viperConfig);
            DefaultConfigManager
                .SetDefaultConfiguration(viperConfig.Target.AppName
                , viperConfig.Target.IpAddress
                , viperConfig.Target.Port
                , viperConfig.Target.TraceOnOff);
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var headers = httpContext.Response.Headers;

            try
            {
                //解析访问者IP地址和端口号
                if (headers != null)
                {
                    headers.TryAdd("Server", "Anno/1.0");
                }
            }
            finally
            {
                await _next(httpContext);
            }

        }
    }
    #region 扩展Newtonsoft.Json 序列化
    public class BigIntJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(long) || objectType == typeof(long?))
            {
                return true;
            }

            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (null != existingValue)
            {
                return long.Parse(existingValue.ToString());
            }
            else
            {
                return DBNull.Value;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteValue(value.ToString());
        }
    }

    public static class DicExt
    {
        public static byte[] ExecuteResult(this Dictionary<string, object> dic)
        {
            if (dic == null)
            {
                return default(byte[]);
            }
            MemoryStream ms = new MemoryStream();
            StreamWriter writer = new StreamWriter(ms);
            JsonSerializer jsonSerializer = new JsonSerializer();
            jsonSerializer.Converters.Add(new BigIntJsonConverter());

            jsonSerializer.Serialize(writer, dic);
            writer.Flush();
            writer.Close();
            return ms.ToArray();
        }
    }
    #endregion
}
