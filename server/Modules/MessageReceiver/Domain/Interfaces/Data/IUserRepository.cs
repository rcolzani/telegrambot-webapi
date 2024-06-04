using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.WebAPI.Domain.Entities;

namespace Telegram.WebAPI.Domain.Interfaces;

public interface IUserRepository
{
    void UpdateUser(User user);
    User AddUser(int chatId, out bool isNewUser, string name);

    Task<List<User>> GetAllUsersAsync();

    Task<List<User>> GetAllUsersWithSendRiverActivateAsync();

    Task<User> GetUserByIdAsync(Guid id);

    Task<User> GetUserByTelegramIdAsync(long id);

    List<User> GetAllRemindersActive();

    User GetAllRemindersActiveByUser(Guid userId);
}
