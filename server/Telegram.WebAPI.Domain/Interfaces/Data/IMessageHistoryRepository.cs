using System.Threading.Tasks;
using Telegram.WebAPI.Domain.Entities;

namespace Telegram.WebAPI.Domain.Interfaces.Data;

public interface IMessageHistoryRepository
{
    Task Add(MessageHistory message);
}

