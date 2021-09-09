using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Anno.CronNET;
using Anno.EngineData.SysInfo;
using Anno.EngineData;
using Anno.Rpc.Client;
using Viper.GetWay.Hubs.Util;

namespace Viper.GetWay.Hubs
{

    public class TaskManager
    {/// <summary>
     /// 任务管理器
     /// </summary>
        private static readonly CronDaemon CronDaemon = new CronDaemon();
        private static readonly UseSysInfoWatch Usi = new UseSysInfoWatch();
        private static readonly object LockCron = new object();
        private static IHubContext<MonitorHub> _monitorContext;
        private static ViperConfig viperConfig = new ViperConfig();
        public TaskManager(IHubContext<MonitorHub> monitorHub, ViperConfig _viperConfig)
        {
            viperConfig = _viperConfig;
            if (CronDaemon.Status == DaemonStatus.Stop)
            {
                lock (LockCron)
                {
                    if (CronDaemon.Status == DaemonStatus.Stop)
                    {
                        _monitorContext = monitorHub;
                        CronDaemon.AddJob("*/2 * * * * ? *", SendMonitorData);
                        CronDaemon.Start();
                    }
                }
            }
        }
        #region CPU监控

        static void SendMonitorData()
        {
            try
            {
                //被监控的APP
                var watchUsers = WatchDataUtil.WatchData.Select(wu => wu.WatchServiceName).ToList().Distinct();

                foreach (var watchUser in watchUsers)
                {
                    var connectionIds = WatchDataUtil.WatchData.Where(wu => wu.WatchServiceName == watchUser).Select(wu => wu.ConnectionId).ToList();
                    if (connectionIds.Count <= 0)
                    {
                        continue;
                    }
                    if (watchUser == viperConfig.Target.AppName)
                    {
                        var info = Usi.GetServerStatus();
                        info.Tag = watchUser;
                        _monitorContext.Clients.Clients(connectionIds.ToArray()).SendAsync("SendMonitorData", info);
                    }
                    else
                    {
                        GetServerStatus(watchUser).ContinueWith(rlt =>
                        {
                            var info = rlt.Result;
                            if (info != null)
                            {
                                info.Tag = watchUser;
                                _monitorContext.Clients.Clients(connectionIds.ToArray()).SendAsync("SendMonitorData", info);
                            }
                        });
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
        public static async Task<UseSysInfoWatch.ServerStatus> GetServerStatus(string nickName)
        {
            Dictionary<string, string> input = new Dictionary<string, string>();
            input.Add("channel", "Anno.Plugs.Monitor");
            input.Add("router", "Resource");
            input.Add("method", "GetServerStatus");
            var watchData = await Connector.BrokerDnsAsync(input, nickName);
            ActionResult<UseSysInfoWatch.ServerStatus> actionResult = null;
            try
            {
                if (watchData.IndexOf("tatus\":true") > 0)
                {
                    actionResult = Newtonsoft.Json.JsonConvert.DeserializeObject<ActionResult<UseSysInfoWatch.ServerStatus>>(watchData);
                }
                else
                {
                    return new UseSysInfoWatch.ServerStatus() { RunTime = "00:00:00:00", CurrentTime = DateTime.Now, Tag = nickName };
                }
            }
            catch
            {
                return new UseSysInfoWatch.ServerStatus() { RunTime = "00:00:00:00", CurrentTime = DateTime.Now, Tag = nickName };
            }
            return actionResult.OutputData;
        }

        #endregion
    }
}
