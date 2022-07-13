using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Telegram.WebAPI.Domain.Entities;

namespace Telegram.WebAPI.Domain.Entities
{
    public class MessageHistory
    {
        public MessageHistory(Guid userId, string textMessage, DateTime messageDate, bool messageSent)
        {
            this.UserId = userId;
            this.TextMessage = textMessage;
            this.MessageDate = messageDate;
            this.MessageSent = messageSent;
        }
        public string TextMessage { get; private set; }
        public DateTime MessageDate { get; private set; }
        public bool MessageSent { get; private set; }

        public Guid UserId { get; private set; }
    }
}
