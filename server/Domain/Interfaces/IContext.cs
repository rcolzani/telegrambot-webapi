using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.WebAPI.Domain.Entities;

namespace Telegram.WebAPI.Domain.Interfaces
{
    public interface IContext 
    {
        public DbSet<TelegramUser> TelegramUser { get; set; }
        public DbSet<MessageHistory> Messages { get; set; }
        public DbSet<Reminder> Reminder { get; set; }

    }
}
