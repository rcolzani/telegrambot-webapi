using System;
using System.Collections.Generic;
using System.Text;
using Telegram.WebAPI.Application.Hubs.Models.Interfaces;

namespace Telegram.WebAPI.Application.Hubs.Models
{
    public class MessageClient : MessageBase
    {
        public MessageClient(string user, string message, DateTime dataHora)
        {
            this.User = user.Substring(0, 2);
            this.Message = message.Replace(user, "<usuário>");
            this.DataHora = dataHora;
        }
    }
}
