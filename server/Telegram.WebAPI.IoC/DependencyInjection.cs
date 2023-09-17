using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.WebAPI.Application.Interfaces;
using Telegram.WebAPI.Application.Services;
using Telegram.WebAPI.Data.Cache;
using Telegram.WebAPI.Domain.Interfaces;
using Telegram.WebAPI.Domain.Interfaces.Application;
using Telegram.WebAPI.Domain.Repositories;
using Telegram.WebAPI.HostedServices;

namespace Telegram.WebAPI.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.RegisterMongoDbRepositories();

            Functions.Settings.TelegramToken = configuration["TELEGRAM_BOT_TOKEN"];
            Functions.Settings.ControllerActionsPassword = configuration["CONTROLLER_ACTION_PASSWORD"];
            Functions.Settings.DatabaseName = "telegrambotreminder";

            services.AddMemoryCache();
            services.AddSingleton<IUserRepositoryCache, UserRepositoryCache>();

            services.AddSingleton<TelegramBotApplication>();
            services.AddSingleton<IReminderApplication, ReminderApplication>();
            services.AddSingleton<IRiverLevelApplication, RiverLevelApplication>();
            services.AddSingleton<IStatisticsApplication, StatisticsApplication>();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddHostedService<TelegramBotService>();

            return services;
        }
    }
}
