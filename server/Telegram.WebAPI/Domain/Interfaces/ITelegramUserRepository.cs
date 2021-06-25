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
        TelegramUser AddClient(int chatId, out bool isNewClient, string name);
        Task<TelegramUser> GetClienteAsync(int id, bool asNoTracking = false);
        TelegramUser GetCliente(int id, bool asNoTracking = false);
        Task<TelegramUser> GetClienteByTelegramIdAsync(long id, bool asNoTracking = false);
        TelegramUser GetClienteByTelegramId(long id, bool asNoTracking = false);
        Task<TelegramUser[]> GetAllClientesAsync();
        TelegramUser[] GetAllClientes();
        List<TelegramUser> GetAllUsersWithSendRiverActivate();
        List<TelegramUser> GetAllUsersWithReminderActivate();
    }
}
