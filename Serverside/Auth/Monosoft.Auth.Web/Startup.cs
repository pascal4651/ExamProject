using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Monosoft.Web.AuthApi.Hubs;

namespace AuthWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            authApiHub.Log("Startup - was called");
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            authApiHub.Log("Startup - configure services was called");
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSignalR();

           // var provider = services.BuildServiceProvider();
            //signalRHub = provider.GetService(typeof(IHubContext<authApiHub>)) as IHubContext<authApiHub>;


            services.AddCors();
        }
        public static IHubContext<authApiHub> signalRHub;
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            authApiHub.Log("Startup - configure called");
            app.Use(async (context, next) =>
            {
                await next();
                signalRHub = context.RequestServices.GetRequiredService<IHubContext<authApiHub>>();
                authApiHub.Log("Startup - hub context was set");
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseSignalR(routes =>
            {
                routes.MapHub<authApiHub>("/authApiHub");
            });
        }
    }
}
