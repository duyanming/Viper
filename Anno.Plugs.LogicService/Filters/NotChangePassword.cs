using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Plugs.LogicService.Filters
{
    using Anno.EngineData;
    using EngineData.Filters;
    public class NotChangePassword : AuthorizationFilterAttribute
    {
        private string[] accounts;
        public NotChangePassword(params string[] pAccounts)
        {
            this.accounts = pAccounts;
        }
        public override void OnAuthorization(BaseModule context)
        {
            if (accounts != null && accounts.Length > 0)
            {
                var dto = context.Request<Domain.Dto.ChangePwdInputDto>("dto");
                var uAccount = Infrastructure.DbInstance.Db.Ado.GetString("SELECT  account FROM  sys_member  WHERE  id=@id;", new { id = dto.ID });
                foreach (var account in accounts)
                {
                    if (account.Equals(uAccount)) {
                        context.Authorized = false;
                        return;
                    }
                }
            }
            context.Authorized = true;
        }
    }
}
