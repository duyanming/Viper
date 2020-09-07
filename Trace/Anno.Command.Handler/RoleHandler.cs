using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anno.CommandBus;
using Anno.Domain.Repository;
using Anno.Domain.Role;

namespace Anno.Command.Handler
{
    public class RoleHandler : ICommandHandler<AddRoleCommand>
    , ICommandHandler<DelRoleCommand>
    ,ICommandHandler<ModifyRoleFuncLinkCommand>
    {
        private readonly IRoleRepository _roleRepository;

        public RoleHandler(IRoleRepository roleRepository)
        {
            this._roleRepository = roleRepository;
        }

        public void Execute(AddRoleCommand command)
        {
            Role role = new Role(IdWorker.NextId());
            var rlt = role.Add(command.Name);
            if (!rlt.success)
            {
                command.Result.Status = false;
                command.Result.Msg = rlt.msg;
                return;
            }

            var rltR = _roleRepository.Add(role);

            if (!rltR.success)
            {
                command.Result.Status = false;
                command.Result.Msg = rltR.msg;
                return;
            }
            command.Id = role.ID;
            command.Result.Status = true;
        }

        public void Execute(DelRoleCommand command)
        {
            var rlt = _roleRepository.Remove(command.Id);
            if (rlt.success)
            {
                command.Result.Status = true;
                return;
            }

            command.Result.Status = false;
            command.Result.Msg = rlt.msg;
        }

        public void Execute(ModifyRoleFuncLinkCommand command)
        {
            Role role = _roleRepository.GetById(command.Id);
            role.RemoveAllFunc();
           command.FidList.ForEach(f =>
           {
               if (!role.Funcs.Exists(ff=>ff.Fid==f))
               {
                   role.AddFunc(f);
               }
           });
            _roleRepository.SaveChange(role);
            command.Result.Status = true;
        }
    }
}
