using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Telegram.WebAPI.Domain.Repositories;

namespace Telegram.WebAPI.Domain.Repositories
{
    public static class RepositoryExtensions
    {
        public static void RegisterMongoDbRepositories(this IServiceCollection servicesBuilder)
        {
            servicesBuilder.AddSingleton<IMongoClient, MongoClient>(s =>
            {
                var uri = s.GetRequiredService<IConfiguration>()["ConnectionStrings:MongoUri"];
                return new MongoClient(uri);
            });
            servicesBuilder.AddSingleton<UserRepository>();
            servicesBuilder.AddSingleton<MessageHistoryRepository>();
        }
    }
}
