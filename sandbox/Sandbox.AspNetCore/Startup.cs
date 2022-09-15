using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sandbox.AspNetCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // 此方法由运行时调用。 使用此方法向容器添加服务。
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddGrpc(); // MagicOnion 依赖于 ASP.NET Core gRPC 服务。
            services.AddMagicOnion();
                //.UseRedisGroupRepository(options =>
                //{
                //    options.ConnectionMultiplexer = StackExchange.Redis.ConnectionMultiplexer.Connect("localhost:6379");
                //});
        }

        //此方法由运行时调用。 使用此方法配置 HTTP 请求管道。
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // 默认 HSTS 值为 30 天. 您可能希望针对生产方案更改此设置, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapMagicOnionHttpGateway("_", app.ApplicationServices.GetService<MagicOnion.Server.MagicOnionServiceDefinition>().MethodHandlers, GrpcChannel.ForAddress("https://localhost:5001"));

                endpoints.MapMagicOnionSwagger("swagger", app.ApplicationServices.GetService<MagicOnion.Server.MagicOnionServiceDefinition>().MethodHandlers, "/_/");

                endpoints.MapMagicOnionService();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
