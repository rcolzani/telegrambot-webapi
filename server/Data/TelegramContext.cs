using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Telegram.WebAPI.Models;

namespace Telegram.WebAPI.Data
{
    public class TelegramContext : DbContext
    {
        public TelegramContext(DbContextOptions<TelegramContext> options) : base(options)
        {

        }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Mensagem> Mensagens { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {


        }
    }
}