using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MvcCallAnno
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        { 
            //1、将默认的xml格式化程序清除
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Formatters.JsonFormatter.SerializerSettings.DateFormatString = "MM/dd/yyy HH:mm:ss";
        }
    }
}
