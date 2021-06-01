using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Telegram.WebAPI.Domain.Entities;

namespace Telegram.WebAPI.Domain.Entities
{
    public class MessageHistory : Entity
    {
        /// <summary>
        /// Construtor para o ef
        /// </summary>
        public MessageHistory() { }
        public MessageHistory(int clienteId, string textMessage, DateTime messageDate, bool messageSent)
        {
            this.TelegramUserId = clienteId;
            this.TextMessage = textMessage;
            this.MessageDate = messageDate;
            this.MessageSent = messageSent;
        }
        public string TextMessage { get; set; }
        public DateTime MessageDate { get; set; }
        public bool MessageSent { get; set; }

        public int TelegramUserId { get; set; }
        public virtual TelegramUser TelegramUser { get; set; }
    }
}
