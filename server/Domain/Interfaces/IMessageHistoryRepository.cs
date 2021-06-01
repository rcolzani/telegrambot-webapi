using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.WebAPI.Data;
using Telegram.WebAPI.Domain.Entities;

namespace Telegram.WebAPI.Domain.Interfaces
{
    public interface IMessageHistoryRepository : IRepository<MessageHistory>
    {
        Task<MessageHistory[]> GetAllMessagesByClienteAsync(int clienteId);
        Task<MessageHistory[]> GetAllMessagesAsync();
    }
}
