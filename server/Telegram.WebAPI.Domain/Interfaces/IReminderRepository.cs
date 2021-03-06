using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.WebAPI.Data;
using Telegram.WebAPI.Domain.Entities;

namespace Telegram.WebAPI.Domain.Interfaces
{
    public interface IReminderRepository : IRepository<Reminder>
    {
        List<Reminder> GetAllRemindersByUser(int clienteId);
        List<Reminder> GetAllRemindersActiveByUser(int clienteId);
        List<Reminder> GetAllRemindersActive();
    }
}
