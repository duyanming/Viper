using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace MvcCallAnno
{
    public static class IocManager
    {
        public static IContainer IoContainer;
        private static readonly ContainerBuilder Builder = new ContainerBuilder();

        public static ContainerBuilder GetContainerBuilder()
        {
            return Builder;
        }

        public static void Build(params Assembly[] assemblies)
        {
            if (assemblies != null)
            {
                assemblies.ToList().ForEach(assembly =>
                {
                    assembly.GetTypes().Where(x => x.GetTypeInfo().IsClass && !x.GetTypeInfo().IsAbstract && !x.GetTypeInfo().IsInterface).ToList().ForEach(
                        t =>
                        {
                            var interfaces = t.GetInterfaces();
                            if (interfaces.Length <= 0)
                            {
                                if (t.IsGenericType)
                                {
                                    Builder.RegisterGeneric(t);
                                }
                                else
                                {
                                    Builder.RegisterType(t);
                                }
                            }
                            else
                            {
                                if (t.IsGenericType)
                                {
                                    Builder.RegisterGeneric(t).As(t.GetInterfaces());
                                }
                                else
                                {
                                    Builder.RegisterType(t).As(t.GetInterfaces());
                                }
                            }
                        });
                });

            }
            IoContainer = Builder.Build();
        }
    }
}