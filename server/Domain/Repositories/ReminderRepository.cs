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
    public class ReminderRepository : Repository<Reminder>, IReminderRepository
    {
        public ReminderRepository(TelegramContext _context) : base(_context) { }

        public Task<List<Reminder>> GetAllRemindersByUserAsync(int userId)
        {
            var dados = _context.Reminder.Where(r => r.TelegramUserId.Equals(userId));

            return dados.ToListAsync();
        }
    }
}
