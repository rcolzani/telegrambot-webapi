using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Telegram.WebAPI.Data.Memory
{
    public static class UserTemp
    {
        public static List<User> TempUsers { get; set; }

        public class User
        {
            public long TelegramId { get; set; }
            public bool HasReminder { get; set; }
        }
    }
}
