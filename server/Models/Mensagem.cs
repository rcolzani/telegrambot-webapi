using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Telegram.WebAPI.Models;

namespace Telegram.WebAPI.Models
{
    public class Mensagem
    {
        public Mensagem(int clienteId, string textMessage, DateTime messageDate, bool messageSent)
        {
            this.ClienteId = clienteId;
            this.TextMessage = textMessage;
            this.MessageDate = messageDate;
            this.MessageSent = messageSent;
        }
        [Key]
        public int Id { get; set; }
        public string TextMessage { get; set; }
        public DateTime MessageDate { get; set; }
        public bool MessageSent { get; set; }

        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
    }
}
