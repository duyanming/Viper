using Anno.EngineData;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Plugs.LogicService
{
    [DependsOn( typeof(Domain.Bootstrap)
       //, typeof(QueryServices.Bootstrap)
       , typeof(Repository.Bootstrap)
       , typeof(Command.Handler.Bootstrap)
        )]
    public class LogicBootstrap : IPlugsConfigurationBootstrap
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
