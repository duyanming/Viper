using Anno.EngineData;
using System;

namespace Anno.Plugs.HelloWorldService
{
    /// <summary>
    /// 插件启动引导器
    /// DependsOn 依赖的类型程序集自动注入DI容器
    /// </summary>
    [DependsOn(
        typeof(ViperLog.ViperLogBootstrap)
        //, typeof(QueryServices.Bootstrap)
        //, typeof(Repository.Bootstrap)
        //, typeof(Command.Handler.Bootstrap
        )]
    public class HelloWorldBootStrap : IPlugsConfigurationBootstrap
    {
        /// <summary>
        /// Service 依赖注入构建之后调用
        /// </summary>
        public void ConfigurationBootstrap()
        {
            //throw new NotImplementedException();
        }
        /// <summary>
        /// Service 依赖注入构建之前调用
        /// </summary>
        /// </summary>
        public void PreConfigurationBootstrap()
        {
            //throw new NotImplementedException();
        }
    }
}
