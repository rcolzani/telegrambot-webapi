using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.WebAPI.Data;
using Telegram.WebAPI.Domain.Entities;
using Telegram.WebAPI.Domain.Enums;
using Telegram.WebAPI.Domain.Interfaces;

namespace Telegram.WebAPI.Domain.Repositories
{
    public class ReminderRepository : Repository<Reminder>, IReminderRepository
    {
        public ReminderRepository(TelegramContext _context) : base(_context) { }

        public List<Reminder> GetAllRemindersActive()
        {
            var dados = _context.Reminder
                .Where(r => r.Status.Equals(ReminderStatus.Activated))
                .OrderByDescending(u => u.CreatedAt)
                .Include(u => u.TelegramUser);

            return dados.ToList();
        }

        public List<Reminder> GetAllRemindersActiveByUser(int userId)
        {
            var dados = _context.Reminder
                .Where(r => r.TelegramUserId.Equals(userId) && r.Status.Equals(ReminderStatus.Activated))
                .OrderByDescending(u => u.CreatedAt)
                .Include(a => a.TelegramUser);

            return dados.ToList();
        }

        public List<Reminder> GetAllRemindersByUser(int userId)
        {
            var dados = _context.Reminder.Where(r => r.TelegramUserId.Equals(userId)).OrderByDescending(u => u.CreatedAt);

            return dados.ToList();
        }
    }
}
