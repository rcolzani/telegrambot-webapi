using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.WebAPI.Domain.Enums;

namespace Telegram.WebAPI.Domain.Entities
{
    public class Reminder : Entity
    {
        public Reminder() { }
        public Reminder(int userId, string textMessage)
        {
            this.TextMessage = textMessage;
            this.TelegramUserId = userId;
            this.Status = ReminderStatus.WaitingForTextMessage;
            this.CreatedAt = DateTime.Now;
        }
        public void AddReminderText(string texto)
        {
            this.TextMessage = texto;
            this.Status = ReminderStatus.WaitingForTime;
        }
        public string TextMessage { get; private set; }
        public TimeSpan RemindTimeToSend { get; private set; }
        public DateTime RemindedAt { get; private set; }
        public ReminderStatus Status { get; private set; }

        public int TelegramUserId { get; private set; }
        public virtual TelegramUser TelegramUser { get; set; }

        public void SetReminded()
        {
            this.RemindedAt = DateTime.Now;
            this.UpdatedAt = DateTime.Now;
        }
        public void StopReminder()
        {
            this.Status = ReminderStatus.Disabled;
            this.UpdatedAt = DateTime.Now;
        }
        public void SetTimeToRemind(TimeSpan time)
        {
            this.RemindTimeToSend = time;
            this.Status = ReminderStatus.Activated;
            this.UpdatedAt = DateTime.Now;

            if (DateTime.Now.Date + this.RemindTimeToSend < DateTime.Now)
                this.RemindedAt = DateTime.Now; //Se o horário do lembrete é um horário que hoje já passou, registra como já enviado o lembrete. Se isso não for feito, será reenviado no próximo ciclo

        }
    }
}
