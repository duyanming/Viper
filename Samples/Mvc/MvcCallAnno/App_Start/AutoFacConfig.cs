using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;

using Anno.Rpc.Client.DynamicProxy;
using MvcCallAnno.Service;

namespace MvcCallAnno
{
    public class AutoFacConfig
    {
        public static void Register()
        {
            var builder = IocManager.GetContainerBuilder();
            var config = GlobalConfiguration.Configuration;

            builder.RegisterApiControllers(Assembly.GetCallingAssembly())
               .PropertiesAutowired();
            builder.RegisterWebApiFilterProvider(config);
            builder.RegisterWebApiModelBinderProvider();
            /*
             * Anno服务接口通过代理注册到IOC容器中去
             */
            builder.RegisterInstance<IHelloWorldViperService>(AnnoProxyBuilder.GetService<IHelloWorldViperService>());

            IocManager.Build(typeof(IocManager).Assembly);
            config.DependencyResolver = new AutofacWebApiDependencyResolver(IocManager.IoContainer);


        }
    }
}