using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.WebAPI.Application.Hubs.Models.Interfaces;
using Telegram.WebAPI.Hubs.Models;

namespace Telegram.WebAPI.Hubs.Clients
{
    public interface IChatClient
    {
        Task ReceiveMessage(MessageBase message);
    }
}