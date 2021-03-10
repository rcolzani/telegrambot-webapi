using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Telegram.WebAPI.Models
{
    public class Cliente
    {
        public Cliente(long telegramChatId, string textMessage, int status, TimeSpan remindTimeToSend, DateTime lastSend, bool activated, bool riverLevel, DateTime startChat)
        {
            this.TelegramChatId = telegramChatId;
            this.TextMessage = textMessage;
            this.Status = status;
            this.RemindTimeToSend = remindTimeToSend;
            this.LastSend = lastSend;
            this.Activated = activated;
            this.RiverLevel = riverLevel;
            this.StartChat = startChat;

        }
        [Key]
        public int Id { get; set; }
        public long TelegramChatId { get; set; }
        public string TextMessage { get; set; }
        public int Status { get; set; }
        public IEnumerable<Mensagem> MessageHistory { get; set; }
        public TimeSpan RemindTimeToSend { get; set; }
        public DateTime LastSend { get; set; }
        public bool Activated { get; set; }
        public bool RiverLevel { get; set; }
        public DateTime StartChat { get; set; }


    }
}