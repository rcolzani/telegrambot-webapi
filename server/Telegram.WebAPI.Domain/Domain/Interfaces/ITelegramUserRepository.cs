using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.WebAPI.Data;
using Telegram.WebAPI.Domain.Entities;

namespace Telegram.WebAPI.Domain.Interfaces
{
    public interface ITelegramUserRepository: IRepository<TelegramUser>
    {
        TelegramUser AddUser(int chatId, out bool isNewClient, string name);
        Task<TelegramUser> GetUserByIdAsync(int id, bool asNoTracking = false);
        TelegramUser GetUserById(int id, bool asNoTracking = false);
        Task<TelegramUser> GetUserByTelegramIdAsync(long id, bool asNoTracking = false);
        TelegramUser GetUserByTelegramId(long id, bool asNoTracking = false);
        Task<TelegramUser[]> GetAllUsersAsync();
        TelegramUser[] GetAllUsers();
        List<TelegramUser> GetAllUsersWithSendRiverActivate();
        List<TelegramUser> GetAllUsersWithReminderActivate();
    }
}
