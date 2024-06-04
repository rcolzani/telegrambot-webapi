using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.WebAPI.Domain.Entities;
using Telegram.WebAPI.Domain.Enums;
using Telegram.WebAPI.Domain.Interfaces;

namespace Telegram.WebAPI.Domain.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _userCollection;

    public UserRepository(IMongoClient mongoClient)
    {
        var camelCaseConvention = new ConventionPack { new CamelCaseElementNameConvention() };
        ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);
        _userCollection = mongoClient.GetDatabase("telegrambotreminder").GetCollection<User>("users");
    }

    public void UpdateUser(User user)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
        var result = _userCollection.ReplaceOne(filter, user);
    }

    public User AddUser(int chatId, out bool isNewUser, string name)
    {
        isNewUser = false;
        var user = GetUserByTelegramIdAsync(chatId).Result;

        if (user == null)
        {
            user = new User(chatId, Enums.TelegramUserStatus.NewCliente, name);
            var result = _userCollection.InsertOneAsync(user);
            isNewUser = true;
        }
        return user;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        var result = await _userCollection.Find(Builders<User>.Filter.Empty).ToListAsync();
        return result;
    }

    public Task<List<User>> GetAllUsersWithSendRiverActivateAsync()
    {
        var result = _userCollection.Find(u => u.SendRiverLevel.Equals(true)).ToListAsync();
        return result;
    }

    public async Task<User> GetUserByIdAsync(Guid id)
    {
        return await _userCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<User> GetUserByTelegramIdAsync(long id)
    {
        return await _userCollection.Find(u => u.TelegramChatId == id).FirstOrDefaultAsync();
    }


    public List<User> GetAllRemindersActive()
    {
        var filter = Builders<User>.Filter.ElemMatch(x => x.Reminders, x => x.Status.Equals(ReminderStatus.Activated));
        var res = _userCollection.Find(filter);

        return res.ToList();
    }

    public User GetAllRemindersActiveByUser(Guid userId)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, userId); //.ElemMatch(x => x.Reminders, x => x.Status.Equals(ReminderStatus.Activated));

        var query = Builders<User>.Filter.And(
            Builders<User>.Filter.Eq(u => u.Id, userId),
            Builders<User>.Filter.ElemMatch(x => x.Reminders, x => x.Status.Equals(ReminderStatus.Activated))
                );

        //var projectionFilter = Builders<User>.Projection
        //   .Include(m => m.Id)
        //   .Include(m => m.Reminders.Where(a => a.Status.Equals(ReminderStatus.Activated)));

        var dados = _userCollection.Find(query);
        //_userCollection.FindOneById
        return dados.FirstOrDefault();
    }
}
