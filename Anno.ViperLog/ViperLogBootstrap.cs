using Anno.EngineData;
using Anno.Loader;
using Anno.Rpc.Client.DynamicProxy;
using Anno.ViperLog.Imp;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Anno.ViperLog
{
    public class ViperLogBootstrap : IPlugsConfigurationBootstrap
    {
        public void ConfigurationBootstrap()
        {
          
        }

        public void PreConfigurationBootstrap()
        {
            /*
           * Anno服务接口通过代理注册到IOC容器中去
           */
            try
            {
                var builder = IocLoader.GetAutoFacContainerBuilder();
                builder.RegisterInstance<ILogService>(AnnoProxyBuilder.GetService<ILogService>());
            }
            catch(Exception ex) {
                Anno.Log.Log.Warn(ex);
                var builder = IocLoader.GetServiceDescriptors(); 
                builder.AddSingleton<ILogService>(AnnoProxyBuilder.GetService<ILogService>());
            }
        }
    }
}
