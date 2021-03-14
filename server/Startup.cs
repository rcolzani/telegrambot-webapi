using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.WebAPI.Data;
using Telegram.WebAPI.Hubs;
using Telegram.WebAPI.services;

namespace Telegram.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TelegramContext>(
                           context => context.UseMySql(Configuration.GetConnectionString("JawsDB")), ServiceLifetime.Singleton
                       );

            Functions.Settings.TelegramToken = Configuration["TelegramBotToken"];
            services.AddControllers().AddNewtonsoftJson(options =>
             options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
             );
            services.AddControllers();

            services.AddSignalR();

            services.AddSingleton<IRepository, Repository>();

            services.AddSingleton<IConfiguration>(Configuration);

            services.AddHostedService<TelegramBotService>();

            services.AddCors(options =>
           {
               options.AddPolicy("ClientPermission", policy =>
               {
                   policy.AllowAnyHeader()
                       .AllowAnyMethod()
                       .WithOrigins("http://localhost:5000")
                       .WithOrigins("https://localhost:5001")
                       .WithOrigins("http://telegram.rcolzani.com")
                       .WithOrigins("https://focused-borg-5cc3c6.netlify.app")
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/hubs/chat");
            });
        }
    }
}
