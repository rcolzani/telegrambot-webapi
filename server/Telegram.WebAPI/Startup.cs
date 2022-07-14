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
using server.Middlewares;
using Telegram.WebAPI.Application;
using Telegram.WebAPI.Application.Services;
using Telegram.WebAPI.Data;
using Telegram.WebAPI.Domain;
using Telegram.WebAPI.Domain.Interfaces;
using Telegram.WebAPI.Domain.Repositories;
using Telegram.WebAPI.Hubs;
using Telegram.WebAPI.HostedServices;
using Telegram.WebAPI.Data.Cache;
using Telegram.WebAPI.Domain.Interfaces.Application;

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
            services.RegisterMongoDbRepositories();
            
            
            services.AddDistributedMemoryCache();
            services.AddMemoryCache();
            services.AddSingleton<IUserRepositoryCache, UserRepositoryCache>();


            Functions.Settings.TelegramToken = Configuration["TelegramBotToken"];
            Functions.Settings.ControllerActionsPassword = Configuration["ControllerActionsPassword"];
            Functions.Settings.DatabaseName = "telegrambotreminder";


            services.AddControllers().AddNewtonsoftJson(options =>
             options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
             );
            services.AddControllers();

            services.AddSignalR();

            //services.AddSingleton<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<TelegramBotApplication>();
            services.AddSingleton<IReminderApplication, ReminderApplication>();
            services.AddSingleton<IRiverLevelApplication, RiverLevelApplication>();
            services.AddSingleton<StatisticsApplication>();

            services.AddSingleton<IConfiguration>(Configuration);

            services.AddHostedService<TelegramBotService>();

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
