using Anno.EngineData;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Command.Handler
{
    [DependsOn(typeof(Domain.Bootstrap))]
    public class Bootstrap : IPlugsConfigurationBootstrap
    {
        public void ConfigurationBootstrap()
        {
            //throw new NotImplementedException();
        }

        public void PreConfigurationBootstrap()
        {
            //throw new NotImplementedException();
        }
    }
}
