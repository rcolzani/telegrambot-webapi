using Microsoft.EntityFrameworkCore;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.WebAPI.Data;
using Telegram.WebAPI.Domain.Entities;
using Telegram.WebAPI.Domain.Interfaces;

namespace Telegram.WebAPI.Domain.Repositories
{
    public class MessageHistoryRepository 
    {

        private readonly IMongoCollection<MessageHistory> _messageCollection;

        public MessageHistoryRepository(IMongoClient mongoClient)
        {
            var camelCaseConvention = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);

            _messageCollection = mongoClient.GetDatabase("telegrambotreminder").GetCollection<MessageHistory>("Messages");

        }

        public async Task<List<MessageHistory>> GetAllMessagesAsync()
        {
            try
            {
                var filter = Builders<MessageHistory>.Filter.Empty;

                return await _messageCollection.Find(filter)
                    .SortBy(a => a.MessageDate).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task Add(MessageHistory message)
        {
            await _messageCollection.InsertOneAsync(message);
        }
    }
}
