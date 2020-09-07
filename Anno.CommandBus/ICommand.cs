using System;

namespace Anno.CommandBus
{
    public interface ICommand
    {
        /// <summary>
        /// 命令ID
        /// </summary>
        long Id { get; }
    }
}
