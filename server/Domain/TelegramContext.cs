using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Telegram.WebAPI.Domain.Entities;

namespace Telegram.WebAPI.Data
{
    public class TelegramContext : DbContext
    {
        public TelegramContext(DbContextOptions<TelegramContext> options) : base(options) { }
        public DbSet<TelegramUser> TelegramUser { get; set; }
        public DbSet<MessageHistory> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {


        }
    }
}