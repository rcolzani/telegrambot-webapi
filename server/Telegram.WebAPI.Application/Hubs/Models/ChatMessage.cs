using System;

namespace Telegram.WebAPI.Hubs.Models
{
    public class ChatMessage
    {
        public ChatMessage(string user, string message, DateTime dataHora)
        {
            this.User = user;
            this.Message = message.Replace(user, "<usuário>");
            this.DataHora = dataHora;
        }
        public string User { get; set; }

        public string Message { get; set; }

        public DateTime DataHora { get; set; }
    }
}