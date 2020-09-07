using System;
using System.Collections.Generic;
using System.Text;
using Anno.CommandBus;
using Anno.Domain.Function;
using Anno.Domain.Repository;

namespace Anno.Command.Handler
{
    public class SysFuncHandler : ICommandHandler<SysFuncCommand>
    {
        private readonly ISysFuncRepository _sysFuncRepository;

        public SysFuncHandler(ISysFuncRepository sysFuncRepository)
        {
            _sysFuncRepository = sysFuncRepository;
        }

        public void Execute(SysFuncCommand command)
        {
            if (command.SysType == SysType.AddChild)
            {
                SysFunc sysFunc = new SysFunc(IdWorker.NextId());
                sysFunc.Mapp(command);
                sysFunc.Pid = command.Id;
                _sysFuncRepository.Add(sysFunc);
                command.Result.Status = true;
            }
            else if (command.SysType == SysType.AddFlatLevel)
            {
                SysFunc sysFunc = new SysFunc(IdWorker.NextId());
                sysFunc.Mapp(command);
                _sysFuncRepository.Add(sysFunc);
                command.Result.Status = true;
            }
            else if (command.SysType == SysType.Remove)
            {
                var rlt = _sysFuncRepository.Remove(command.Id);
                if (!rlt.success)
                {
                    command.Result.Status = false;
                    command.Result.Msg = rlt.msg;
                }
                command.Result.Status = true;
            }
            else if (command.SysType == SysType.Update)
            {
                SysFunc sysFunc = new SysFunc();
                sysFunc.Mapp(command);
                _sysFuncRepository.SaveChange(sysFunc);
                command.Result.Status = true;
            }
        }
    }
}
