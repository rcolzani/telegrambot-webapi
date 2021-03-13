using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.WebAPI.Models;

namespace Telegram.WebAPI.Data
{
    public interface IRepository
    {
        void Add<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Remove<T>(T entity) where T : class;
        bool SaveChanges();

        Task<Cliente> GetClienteAsync(int id, bool asNoTracking = false);
        Task<Cliente> GetClienteByTelegramIdAsync(long id, bool asNoTracking = false);
        Task<Cliente[]> GetAllClientesAsync();
        Task<Mensagem[]> GetAllMessagesByClienteAsync(int clienteId);

    }
}