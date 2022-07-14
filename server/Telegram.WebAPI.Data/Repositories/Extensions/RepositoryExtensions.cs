using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Diagnostics;
using Telegram.WebAPI.Domain.Interfaces;
using Telegram.WebAPI.Domain.Interfaces.Data;
using Telegram.WebAPI.Domain.Repositories;

namespace Telegram.WebAPI.Domain.Repositories
{
    public static class RepositoryExtensions
    {
        public static void RegisterMongoDbRepositories(this IServiceCollection servicesBuilder)
        {
            servicesBuilder.AddSingleton<IMongoClient, MongoClient>(s =>
            {
                string dataBaseUri = System.Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

                if(Debugger.IsAttached)
                    dataBaseUri = s.GetRequiredService<IConfiguration>().GetSection("DB_CONNECTION_STRING").Value;

                return new MongoClient(dataBaseUri);
            });

            servicesBuilder.AddSingleton<IUserRepository, UserRepository>();
            servicesBuilder.AddSingleton<IMessageHistoryRepository, MessageHistoryRepository>();
        }
    }
}
