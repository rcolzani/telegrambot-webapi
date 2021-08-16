using System;
using System.Collections.Generic;
using System.Text;
using Telegram.WebAPI.Application.Hubs.Models.Interfaces;

namespace Telegram.WebAPI.Application.Hubs.Models
{
    public class MessageSystem : MessageBase
    {
        public MessageSystem(string user, string message, DateTime dataHora)
        {
            this.User = user;
            this.Message = message;
            this.DataHora = dataHora;
        }
    }
}
