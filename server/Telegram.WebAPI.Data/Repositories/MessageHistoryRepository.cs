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
    public class MessageHistoryRepository : Repository<MessageHistory>, IMessageHistoryRepository
    {
       
        public MessageHistoryRepository(TelegramContext _context) : base(_context) { }
      
        public async Task<MessageHistory[]> GetAllMessagesAsync()
        {
            try
            {
                IQueryable<MessageHistory> query = _context.Messages;
                query = query.AsNoTracking()
                             .OrderBy(a => a.MessageDate);

                return await query.ToArrayAsync();
            }
            catch (Exception)
            {
                throw;
            }           
        }

        public async Task<MessageHistory[]> GetAllMessagesByClienteAsync(int clienteId)
        {
            try
            {
                IQueryable<MessageHistory> query = _context.Messages;
                query = query.Where(u => u.TelegramUserId == clienteId);
                query = query.AsNoTracking()
                             .OrderBy(a => a.MessageDate);

                return await query.ToArrayAsync();
            }
            catch (Exception)
            {
                
                throw;
            }
         
        }
    }
}
