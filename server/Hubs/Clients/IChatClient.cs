using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.WebAPI.Hubs.Models;

namespace Telegram.WebAPI.Hubs.Clients
{
    public interface IChatClient
    {
        Task ReceiveMessage(ChatMessage message);
        Task StatistcsUpdate(StatisticsMain statistics);
    }
}