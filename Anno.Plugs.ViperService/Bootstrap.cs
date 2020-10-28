using Anno.EngineData;
using System;

namespace Anno.Plugs.ViperService
{
    /// <summary>
    /// 插件启动引导器
    /// DependsOn 依赖的类型程序集自动注入DI容器
    /// </summary>
    [DependsOn(
        //typeof(Domain.Bootstrap)
       //, typeof(QueryServices.Bootstrap)
       //, typeof(Repository.Bootstrap)
       //, typeof(Command.Handler.Bootstrap
        )]
    public class Bootstrap : IPlugsConfigurationBootstrap
    {
        /// <summary>
        /// Service 依赖注入构建之后调用
        /// </summary>
        public void ConfigurationBootstrap()
        {

        }
        /// <summary>
        /// Service 依赖注入构建之前调用
        /// </summary>
        public void PreConfigurationBootstrap()
        {

        }
    }
}
