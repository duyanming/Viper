using Anno.EngineData;
using Anno.EngineData.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Plugs.LogicService
{
    /// <summary>
    /// 服务权限过滤器
    /// </summary>
    public class Authorization : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(BaseModule context)
        {
            /*
             * 只有admin用户可以操作
             */
            if (context.RequestString("uname") != "admin")
            {
                context.Authorized = false;
                return;
            }
            context.Authorized = true;          
        }
    }
}
