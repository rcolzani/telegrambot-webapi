using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using server.Middlewares;
using Telegram.WebAPI.Hubs;
using Telegram.WebAPI.IoC;

namespace Telegram.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            webHostEnvironment = env;
        }

        public IWebHostEnvironment webHostEnvironment { get; set; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddInfrastructure(Configuration);

            services.AddControllers();

            services.AddSignalR();

            var origins = new string[2];
            origins.SetValue("https://telbot.rcolzani.com", 0);
            origins.SetValue("https://focused-borg-5cc3c6.netlify.app", 1);

            if (webHostEnvironment.IsDevelopment())
            {
                Array.Resize(ref origins, 4);
                origins.SetValue("http://localhost:3000", 2);
                origins.SetValue("https://localhost:3000", 3);
            }

            services.AddCors(options =>
           {
               options.AddPolicy("ClientPermission", policy =>
               {
                   policy.AllowAnyHeader()
                       .AllowAnyMethod()
                       .WithOrigins(origins)
                       .AllowCredentials();
               });
           });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseCors("ClientPermission");

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<MiddlewareTeste>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/hubs/chat");
            });
        }
    }
}
