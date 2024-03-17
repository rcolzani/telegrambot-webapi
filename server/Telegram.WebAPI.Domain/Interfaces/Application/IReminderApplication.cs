using System.Threading.Tasks;

namespace Telegram.WebAPI.Domain.Interfaces.Application;

public interface IReminderApplication
{
    Task<bool> SendReminders();
}

