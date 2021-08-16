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
        public string TextMessage { get; private set; }
        public DateTime MessageDate { get; private set; }
        public bool MessageSent { get; private set; }

        public int TelegramUserId { get; private set; }
        public virtual TelegramUser TelegramUser { get; set; }
    }
}
