using HtmlAgilityPack;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.WebAPI.Domain.Interfaces;
using Telegram.WebAPI.Hubs;
using Telegram.WebAPI.Hubs.Clients;

namespace Telegram.WebAPI.Application
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
                foreach (var reminder in _unitOfWork.Reminders.GetAllRemindersActive())
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
                Functions.Generic.LogException(e);
                return false;
            }
        }
    }
}
