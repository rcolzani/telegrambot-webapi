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

        Cliente AddClient(int chatId, out bool isNewClient);
        Task<Cliente> GetClienteAsync(int id, bool asNoTracking = false);
        Cliente GetCliente(int id, bool asNoTracking = false);
        Task<Cliente> GetClienteByTelegramIdAsync(long id, bool asNoTracking = false);
        Cliente GetClienteByTelegramId(long id, bool asNoTracking = false);
        Task<Cliente[]> GetAllClientesAsync();
        Cliente[] GetAllClientes();
        Task<Mensagem[]> GetAllMessagesByClienteAsync(int clienteId);
        Task<Mensagem[]> GetAllMessagesAsync();

    }
}