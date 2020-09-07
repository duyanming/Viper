using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.CommandBus
{
    /// <summary>
    /// 命令Bus
    /// </summary>
    public interface ICommandBus
    {
        /// <summary>
        /// 命令发送器
        /// </summary>
        /// <typeparam name="TCommand">命令</typeparam>
        /// <param name="command">命令实例</param>
        void Send<TCommand>(TCommand command) where TCommand : ICommand;
        /// <summary>
        /// 命令发送器 异步
        /// </summary>
        /// <typeparam name="TCommand">命令</typeparam>
        /// <param name="command">命令实例</param>
        /// <param name="callback">回调函数</param>
        void SendAsync<TCommand>(TCommand command, Action<TCommand> callback = null) where TCommand : ICommand;
    }
}
