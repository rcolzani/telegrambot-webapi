using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using Telegram.WebAPI.Hubs.Clients;

namespace Telegram.WebAPI.Hubs
{
    public class ChatHub : Hub<IChatClient>
    { }
}