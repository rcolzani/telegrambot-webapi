using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Telegram.WebAPI.Models;

namespace Telegram.WebAPI.Data
{
    public class Repository : IRepository
    {
        private readonly TelegramContext _context;
        public Repository(TelegramContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Remove<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }
        public void Update<T>(T entity) where T : class
        {
            _context.Update(entity);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() > 0);
        }

        public async Task<Cliente> GetClienteAsync(int id, bool asNoTracking)
        {
            IQueryable<Cliente> query = _context.Clientes;

            query = query.Where(u => u.Id == id);
            return await query.FirstOrDefaultAsync();
        }
        public async Task<Cliente> GetClienteByTelegramIdAsync(long telegramId, bool asNoTracking)
        {
            IQueryable<Cliente> query = _context.Clientes;
            query = query.Where(u => u.TelegramChatId == telegramId);

            if (asNoTracking)
                query.AsNoTracking();

            return await query.FirstOrDefaultAsync();
        }
        public async Task<Cliente[]> GetAllClientesAsync()
        {
            IQueryable<Cliente> query = _context.Clientes;
            query = query.OrderBy(a => a.Id);
            return await query.ToArrayAsync();
        }

        public async Task<Mensagem[]> GetAllMessagesByClienteAsync(int clienteId)
        {
            IQueryable<Mensagem> query = _context.Mensagens;
            query = query.Where(u => u.ClienteId == clienteId);
            query = query.AsNoTracking()
                         .OrderBy(a => a.MessageDate);

            return await query.ToArrayAsync();
        }
    }
}