using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Telegram.WebAPI.Domain.Enums
{
    public enum TelegramUserStatus
    {
        NewCliente = 0,
        WaitingForTextMessage = 1,
        WaitingForTime = 2,
        Complete = 3
    }
}
