using System.Threading.Tasks;
using Telegram.WebAPI.Application.Hubs.Models.Interfaces;

namespace Telegram.WebAPI.Hubs.Clients;

public interface IChatClient
{
    Task ReceiveMessage(MessageBase message);
}
