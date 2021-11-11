using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Anno.EngineData.SysInfo;
using Viper.GetWay.Hubs.Util;

namespace Viper.GetWay.Hubs
{
    public class MonitorHub : Hub
    {
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
            WatchDataUtil.WatchData.RemoveAll(w => w.ConnectionId == Context.ConnectionId);
            await Clients.Caller.SendAsync("OnDisconnected", $"{Context.ConnectionId} left");
        }
        #endregion
        /// <summary>
        /// 连接成功客户端推送附加信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task SetWatch(string name)
        {
            try
            {
                if (WatchDataUtil.WatchData == null)
                {
                    WatchDataUtil.WatchData = new List<WatchUser>();
                }
                if (!WatchDataUtil.WatchData.Exists(w => w.ConnectionId == Context.ConnectionId))
                {
                    WatchDataUtil.WatchData.Add(new WatchUser()
                    {
                        ConnectionId = Context.ConnectionId,
                        WatchServiceName = name
                    });
                }
                else
                {
                    WatchDataUtil.WatchData.RemoveAll(w => w.ConnectionId == Context.ConnectionId);
                    WatchDataUtil.WatchData.Add(new WatchUser()
                    {
                        ConnectionId = Context.ConnectionId,
                        WatchServiceName = name
                    });
                }
                await Clients.Caller.SendAsync("SetWatch", $"SetWatch {name} OK");
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("SetWatch", $"SetWatch {name} {ex.Message}");
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
}
