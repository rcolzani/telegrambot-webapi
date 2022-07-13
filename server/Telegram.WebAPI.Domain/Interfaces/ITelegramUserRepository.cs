using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.WebAPI.Data;
using Telegram.WebAPI.Domain.Entities;

namespace Telegram.WebAPI.Domain.Interfaces
{
    public interface ITelegramUserRepository: IRepository<User>
    {
        User AddUser(int chatId, out bool isNewClient, string name);
        Task<User> GetUserByIdAsync(int id, bool asNoTracking = false);
        User GetUserById(int id, bool asNoTracking = false);
        Task<User> GetUserByTelegramIdAsync(long id, bool asNoTracking = false);
        User GetUserByTelegramId(long id, bool asNoTracking = false);
        Task<User[]> GetAllUsersAsync();
        User[] GetAllUsers();
        List<User> GetAllUsersWithSendRiverActivate();
        List<User> GetAllUsersWithReminderActivate();
    }
}
