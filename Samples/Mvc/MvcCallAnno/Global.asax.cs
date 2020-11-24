using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace MvcCallAnno
{
    using Anno.Rpc.Client;
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            /*
             * 配置注册中心地址
             * 客户端名称：MvcCallAnno
             * 注册中心IP：127.0.0.1
             * 注册中心端口：7010
             * 调用链追踪：false（true开启，false关闭）
             */
            DefaultConfigManager.SetDefaultConfiguration("MvcCallAnno", "127.0.0.1", 7010, true);
            /*
             * Autofac Ioc 初始化
             */
            AutoFacConfig.Register();
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
