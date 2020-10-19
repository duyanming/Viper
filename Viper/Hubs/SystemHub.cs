using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.SignalR;
using Anno.CronNET;
using Anno.EngineData.SysInfo;
using Anno.EngineData;
using Anno.Rpc.Client;

namespace Viper.GetWay.Hubs
{
    public class SystemHub : Hub
    {
        #region 基础方法
        /// <summary>
        /// 建立连接时触发
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {

            await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} joined", "你连接成功了！");
        }

        /// <summary>
        /// 离开连接时触发
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception ex)
        {

            await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} left");
        }

        /// <summary>
        /// 向所有人推送消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task Send(string message)
        {
            return Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId}: {message}");
        }
        /// <summary>
        /// 向指定组推送消息
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task SendToGroup(string groupName, string message)
        {
            return Clients.Group(groupName).SendAsync("ReceiveMessage", $"{Context.ConnectionId}@{groupName}: {message}");
        }
        /// <summary>
        /// 加入指定组并向组推送消息
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task JoinGroup(string groupName)
        {

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("ReceiveMessage", $"{Context.ConnectionId} joined {groupName}");
        }
        /// <summary>
        /// 推出指定组并向组推送消息
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("ReceiveMessage", $"{Context.ConnectionId} left {groupName}");
        }
        /// <summary>
        /// 向指定Id推送消息
        /// </summary>
        /// <param name="userid">要推送消息的对象</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task Echo(string userid, string message)
        {
            return Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", $"{Context.ConnectionId}: {message}");
        }
        /// <summary>
        /// 向所有人推送消息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(string user, string message)
        {

            await Clients.All.SendAsync("ReceiveMessage", Context.ConnectionId, message);

        }
        #endregion
    }

    public class MonitorHub : Hub
    {
        public static readonly List<WatchUser> WatchData = new List<WatchUser>();
        public MonitorHub(TaskManager taskManager)
        {

        }

        #region 基础方法
        /// <summary>
        /// 建立连接时触发
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {

            await Clients.Caller.SendAsync("OnConnected", $"{Context.ConnectionId} joined", "你连接成功了！");
        }

        /// <summary>
        /// 离开连接时触发
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            WatchData.RemoveAll(w => w.ConnectionId == Context.ConnectionId);
            await Clients.Caller.SendAsync("OnDisconnected", $"{Context.ConnectionId} left");
        }
        #endregion
        /// <summary>
        /// 连接成功客户端推送附加信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public void SetWatch(string name)
        {
            if (!WatchData.Exists(w => w.ConnectionId == Context.ConnectionId))
            {
                WatchData.Add(new WatchUser()
                {
                    ConnectionId = Context.ConnectionId,
                    WatchServiceName = name
                });
            }
            else
            {
                WatchData.Find(w => w.ConnectionId == Context.ConnectionId).WatchServiceName = name;
            }
        }
        /// <summary>
        /// 接受Service推送的性能检测报告，并且推送给客户端
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task GetAndSendMonitorData(UseSysInfoWatch.ServerStatus data)
        {
            return Clients.Caller.SendAsync("SendMonitorData", data);
        }
    }

    public class TaskManager
    {/// <summary>
     /// 任务管理器
     /// </summary>
        private static readonly CronDaemon CronDaemon = new CronDaemon();
        private static readonly UseSysInfoWatch Usi = new UseSysInfoWatch();
        private static readonly object LockCron = new object();
        private static IHubContext<MonitorHub> _monitorContext;
        private static TraceConfig traceConfig = new TraceConfig();
        public TaskManager(IHubContext<MonitorHub> monitorHub, TraceConfig _traceConfig)
        {
            traceConfig = _traceConfig;
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
                var watchUsers = MonitorHub.WatchData.Select(wu => wu.WatchServiceName).ToList().Distinct();

                foreach (var watchUser in watchUsers)
                {
                    var connectionIds = MonitorHub.WatchData.Where(wu => wu.WatchServiceName == watchUser).Select(wu => wu.ConnectionId).ToList();
                    if (connectionIds.Count <= 0)
                    {
                        continue;
                    }
                    if (watchUser == traceConfig.Target.AppName)
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
                        }).Wait();
                    }
                }
            }
            catch(Exception ex)
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
            var actionResult = Newtonsoft.Json.JsonConvert.DeserializeObject<ActionResult<UseSysInfoWatch.ServerStatus>>(await Connector.BrokerDnsAsync(input, nickName));
            return actionResult.OutputData;
        }

        #endregion
    }
    public class WatchUser
    {
        public string ConnectionId { get; set; }
        public string WatchServiceName { get; set; }
    }
}
