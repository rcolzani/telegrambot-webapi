using System.Collections.Generic;
using System.Threading.Tasks;
using server.Hubs.Models;

namespace Telegram.WebAPI.Hubs.Clients
{
    public interface IChatClient
    {
        Task ReceiveMessage(ChatMessage message);
        Task StatistcsUpdate(StatisticsMain statistics);
    }
}