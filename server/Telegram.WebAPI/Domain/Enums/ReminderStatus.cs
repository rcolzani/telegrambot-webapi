using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Telegram.WebAPI.Domain.Enums
{
    public enum ReminderStatus
    {
        WaitingForTextMessage = 1,
        WaitingForTime = 2,
        Activated = 3,
        Disabled = 4
    }
}
