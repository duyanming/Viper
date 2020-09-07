using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anno.CommandBus
{
    public class CommandBus : ICommandBus
    {
        /// <summary>
        /// 线程锁
        /// </summary>
        private static object _lock = new object();

        private static CommandBus _instance;
        /// <summary>
        /// 采用单例模式
        /// </summary>
        public static ICommandBus Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new CommandBus();
                    }
                }
                return _instance;
            }
        }
        public void Send<TCommand>(TCommand command)where TCommand:ICommand
        {
           var handler = Loader.IocLoader.Resolve<ICommandHandler<TCommand>>();
            if (handler != null)
            {
                handler.Execute(command);
            }
            else
            {
                throw new ArgumentNullException($"ICommandHandler<{typeof(TCommand).Name}> 未注册！");
            }
        }

        public void SendAsync<TCommand>(TCommand command, Action<TCommand> callback = null) where TCommand : ICommand
        {
            var handler = Loader.IocLoader.Resolve<ICommandHandler<TCommand>>();
            if (handler != null)
            {
                Task.Run(()=> {
                    handler.Execute(command);
                    callback?.Invoke(command);
                });
            }
            else
            {
                throw new ArgumentNullException($"ICommandHandler<{typeof(TCommand).Name}> 未注册！");
            }
        }
    }
}
