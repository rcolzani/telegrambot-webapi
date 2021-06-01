using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.WebAPI.Data;
using Telegram.WebAPI.Domain.Entities;
using Telegram.WebAPI.Domain.Interfaces;

namespace Telegram.WebAPI.Domain.Repositories
{
    public class TelegramUserRepository : Repository<TelegramUser>, ITelegramUserRepository
    {
        public TelegramUserRepository(TelegramContext _context) : base(_context) { }
        public TelegramUser AddClient(int chatId, out bool isNewClient)
        {
            isNewClient = false;
            var clientChat = GetClienteByTelegramId(chatId, true);

            if (clientChat == null)
            {
                Add(new TelegramUser(chatId, Enums.TelegramUserStatus.NewCliente));
                _context.SaveChanges();
                clientChat = GetClienteByTelegramId(chatId, true);
                isNewClient = true;
            }
            return clientChat;
        }

        public TelegramUser[] GetAllClientes()
        {
            IQueryable<TelegramUser> query = _context.TelegramUser;
            query = query.OrderBy(a => a.Id);
            return query.ToArray();
        }

        public async Task<TelegramUser[]> GetAllClientesAsync()
        {
            IQueryable<TelegramUser> query = _context.TelegramUser;
            query = query.OrderBy(a => a.Id);
            return await query.ToArrayAsync();
        }

        public TelegramUser GetCliente(int id, bool asNoTracking = false)
        {
            IQueryable<TelegramUser> query = _context.TelegramUser;

            query = query.Where(u => u.Id == id);
            return query.FirstOrDefault();
        }

        public async Task<TelegramUser> GetClienteAsync(int id, bool asNoTracking = false)
        {
            IQueryable<TelegramUser> query = _context.TelegramUser;

            query = query.Where(u => u.Id == id);
            return await query.FirstOrDefaultAsync();
        }

        public TelegramUser GetClienteByTelegramId(long id, bool asNoTracking = false)
        {
            IQueryable<TelegramUser> query = _context.TelegramUser;
            query = query.Where(u => u.TelegramChatId == id);

            if (asNoTracking)
                query.AsNoTracking();

            return query.FirstOrDefault();
        }

        public async Task<TelegramUser> GetClienteByTelegramIdAsync(long id, bool asNoTracking = false)
        {
            IQueryable<TelegramUser> query = _context.TelegramUser;
            query = query.Where(u => u.TelegramChatId == id);

            if (asNoTracking)
                query.AsNoTracking();

            return await query.FirstOrDefaultAsync();
        }
    }
}
