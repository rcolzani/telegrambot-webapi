using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.WebAPI.Domain.Interfaces;
using Telegram.WebAPI.Domain.Repositories;

namespace Telegram.WebAPI.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TelegramContext _context;
        public UnitOfWork(TelegramContext context)
        {
            _context = context;
            MessageHistorys = new MessageHistoryRepository(_context);
            TelegramUsers = new TelegramUserRepository(_context);
            Reminders = new ReminderRepository(_context);
        }
        public IMessageHistoryRepository MessageHistorys { get; private set; }

        public ITelegramUserRepository TelegramUsers { get; private set; }

        public IReminderRepository Reminders { get; private set; }

        public int Complete()
        {
            return _context.SaveChanges();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}