using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Viper.GetWay
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Hubs.TaskManager, Hubs.TaskManager>();
            services.AddMvc();
            services.AddSignalR();
            services.AddResponseCompression(options =>
            {
                //options.Providers.Add<BrotliCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error/Index/500");
                app.UseStatusCodePagesWithReExecute("/error/Index/404");
            }
            app.UseResponseCompression();

            app.UseStaticFiles();//使用静态文件默认的文件夹为wwwroot
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<Hubs.SystemHub>("/SystemHub");
                endpoints.MapHub<Hubs.MonitorHub>("/MonitorHub");
                endpoints.MapControllerRoute("default", "{Controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
