using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Telegram.WebAPI.Domain.Interfaces;
using Telegram.WebAPI.Hubs;
using Telegram.WebAPI.Hubs.Clients;
using Telegram.WebAPI.Shared.Extensions;

namespace Telegram.WebAPI.Application.Services
{
    public class ReminderApplication
    {
        private TelegramBotApplication _telegramBotApp;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<ChatHub, IChatClient> _chatHub;
        public ReminderApplication(IUnitOfWork unitOfWork, IHubContext<ChatHub, IChatClient> chatHub, TelegramBotApplication telegramBotApplication)
        {
            _chatHub = chatHub;
            _unitOfWork = unitOfWork;
            _telegramBotApp = telegramBotApplication;
        }

        public async Task<bool> SendReminders()
        {
            try
            {
                var remindersToSend = _unitOfWork.Reminders.GetAllRemindersActive();
                foreach (var reminder in remindersToSend)
                {
                    if ((DateTime.Now.Date + reminder.RemindTimeToSend) <= DateTime.Now && reminder.RemindedAt.Date < DateTime.Now.Date) //considerar enviar 
                    {
                        if (reminder.RemindedAt == new DateTime())
                        {
                            await _telegramBotApp.sendMessageAsync(reminder.TelegramUser.TelegramChatId, reminder.TextMessage);
                        }
                        else if (reminder.RemindedAt.AddMinutes(-5) < DateTime.Now)
                        {
                            await _telegramBotApp.sendMessageAsync(reminder.TelegramUser.TelegramChatId, reminder.TextMessage);
                        }
                        reminder.SetReminded();
                        _unitOfWork.Reminders.Update(reminder);
                    }
                }
                _unitOfWork.Complete();
                return true;
            }
            catch (Exception e)
            {
                e.LogExceptionToConsole();
                return false;
            }
        }
    }
}
