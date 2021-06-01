using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Telegram.WebAPI.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IMessageHistoryRepository MessageHistorys { get; }
        ITelegramUserRepository TelegramUsers { get; }
        int Complete();
    }
}
