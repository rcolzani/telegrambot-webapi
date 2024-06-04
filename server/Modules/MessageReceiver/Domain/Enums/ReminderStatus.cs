namespace Telegram.WebAPI.Domain.Enums;

public enum ReminderStatus
{
    WaitingForTextMessage = 1,
    WaitingForTime = 2,
    Activated = 3,
    Disabled = 4
}
