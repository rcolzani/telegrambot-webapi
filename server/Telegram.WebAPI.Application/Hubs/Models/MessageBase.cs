using System;
using System.Collections.Generic;
using System.Text;

namespace Telegram.WebAPI.Application.Hubs.Models.Interfaces
{
    public class MessageBase
    {
        public string User { get; set; }

        public string Message { get; set; }

        public DateTime DataHora { get; set; }
    }
}
