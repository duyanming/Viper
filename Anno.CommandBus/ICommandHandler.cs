using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.CommandBus
{
    /// <summary>
    /// 处理Handler 每个Command 有且只有一个 Handler
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// 处理Command
        /// </summary>
        /// <param name="command"></param>
        void Execute(TCommand command);
    }
}
