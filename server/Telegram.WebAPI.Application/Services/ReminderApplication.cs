using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Telegram.WebAPI.Domain.Interfaces;
using Telegram.WebAPI.Domain.Interfaces.Application;
using Telegram.WebAPI.Hubs;
using Telegram.WebAPI.Hubs.Clients;
using Telegram.WebAPI.Shared.Extensions;

namespace Telegram.WebAPI.Application.Services;

public class ReminderApplication : IReminderApplication
{
    private TelegramBotApplication _telegramBotApp;
    private readonly IUserRepository _userRepository;
    private readonly IHubContext<ChatHub, IChatClient> _chatHub;
    public ReminderApplication(IUserRepository userRepository, IHubContext<ChatHub, IChatClient> chatHub, TelegramBotApplication telegramBotApplication)
    {
        _chatHub = chatHub;
        _userRepository = userRepository;
        _telegramBotApp = telegramBotApplication;
    }

    public async Task<bool> SendReminders()
    {
        try
        {
            var users = _userRepository.GetAllRemindersActive();
            foreach (var user in users)
            {
                foreach (var reminder in user.Reminders)
                {
                    if ((DateTime.Now.Date + reminder.RemindTimeToSend) <= DateTime.Now && reminder.RemindedAt.Date < DateTime.Now.Date) //considerar enviar 
                    {
                        if (reminder.RemindedAt == new DateTime())
                        {
                            await _telegramBotApp.sendMessageAsync(user.TelegramChatId, reminder.TextMessage);
                        }
                        else if (reminder.RemindedAt.AddMinutes(-5) < DateTime.Now)
                        {
                            await _telegramBotApp.sendMessageAsync(user.TelegramChatId, reminder.TextMessage);
                        }
                        reminder.SetReminded();
                    }
                }
                _userRepository.UpdateUser(user);
            }
            return true;
        }
        catch (Exception e)
        {
            e.LogExceptionToConsole();
            return false;
        }
    }
}
