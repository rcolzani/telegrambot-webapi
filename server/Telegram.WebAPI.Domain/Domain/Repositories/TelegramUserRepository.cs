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
        public TelegramUser AddUser(int chatId, out bool isNewUser, string name)
        {
            isNewUser = false;
            var user = GetUserByTelegramId(chatId, false);

            if (user == null)
            {
                user = new TelegramUser(chatId, Enums.TelegramUserStatus.NewCliente, name);
                _context.Add(user);
                _context.SaveChanges();

                //clientChat = GetClienteByTelegramId(chatId, false);
                isNewUser = true;
            }
            return user;
        }

        public TelegramUser[] GetAllUsers()
        {
            IQueryable<TelegramUser> query;

            query = _context.TelegramUser;

            query = query.OrderBy(a => a.Id);
            return query.ToArray();
        }

        public async Task<TelegramUser[]> GetAllUsersAsync()
        {
            IQueryable<TelegramUser> query;
            query = _context.TelegramUser;
            query = query.OrderBy(a => a.Id);
            return await query.ToArrayAsync();
        }

        public List<TelegramUser> GetAllUsersWithReminderActivate()
        {
            IQueryable<TelegramUser> dados;

            dados = _context.TelegramUser.Where(u => u.Reminders.Where(r => r.Status == Enums.ReminderStatus.Activated).Count() > 0);

            dados = dados.AsNoTracking();
            return dados.ToList();
        }

        public List<TelegramUser> GetAllUsersWithSendRiverActivate()
        {
            IQueryable<TelegramUser> dados;

            dados = _context.TelegramUser.Where(c => c.SendRiverLevel.Equals(true));

            dados = dados.AsNoTracking();
            return dados.ToList();
        }

        public TelegramUser GetUserById(int id, bool asNoTracking = false)
        {
            IQueryable<TelegramUser> query;

            query = _context.TelegramUser;

            if (asNoTracking)
                query = query.AsNoTracking();

            query = query.Where(u => u.Id == id);
            return query.FirstOrDefault();
        }

        public async Task<TelegramUser> GetUserByIdAsync(int id, bool asNoTracking = false)
        {
            IQueryable<TelegramUser> query;

            query = _context.TelegramUser;

            if (asNoTracking)
                query = query.AsNoTracking();

            query = query.Where(u => u.Id == id);
            return await query.FirstOrDefaultAsync();
        }

        public TelegramUser GetUserByTelegramId(long id, bool asNoTracking = false)
        {
            IQueryable<TelegramUser> query;

            query = _context.TelegramUser;

            query = query.Where(u => u.TelegramChatId == id);

            if (asNoTracking)
                query = query.AsNoTracking();

            return query.FirstOrDefault();
        }

        public async Task<TelegramUser> GetUserByTelegramIdAsync(long id, bool asNoTracking = false)
        {
            IQueryable<TelegramUser> query;

            query = _context.TelegramUser;

            query = query.Where(u => u.TelegramChatId == id);

            if (asNoTracking)
                query.AsNoTracking();

            return await query.FirstOrDefaultAsync();
        }
    }
}
