using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.WebAPI.Data;
using Telegram.WebAPI.Hubs;
using Telegram.WebAPI.Hubs.Clients;
using Telegram.WebAPI.Hubs.Models;
using Functions;
using Telegram.WebAPI.Domain.Interfaces;
using Telegram.WebAPI.Domain.Entities;
using Telegram.WebAPI.Application;

namespace Telegram.WebAPI.services
{
    /// <summary>
    /// Service responsável por receber mensagens e executar as aplicações de Lembrete (reminder) e nível do rio ()
    /// </summary>
    public class TelegramBotService : IHostedService
    {
         private readonly string _token = Functions.Settings.TelegramToken;
        private TelegramBotClient bot;
      
        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
        private readonly IHubContext<ChatHub, IChatClient> _chatHub;
        private bool telegramBotRunning = false;
        private TelegramBotApplication _telegramBotApplication;
        private ReminderApplication _reminderApplication;
        private RiverLevelApplication _riverLevelApp;
        public TelegramBotService(IHubContext<ChatHub, IChatClient> chatHub, TelegramBotApplication telegramBotApplication, ReminderApplication reminderApplication, RiverLevelApplication riverLevelApp )
        {
            _chatHub = chatHub;

            _telegramBotApplication = telegramBotApplication;
            _telegramBotApplication.bot = new TelegramBotClient(_token);

            _reminderApplication = reminderApplication;
            _riverLevelApp = riverLevelApp;

            bot = new TelegramBotClient(_token);
            bot.OnMessage += botMessageReceiver;
            Settings.TelegramBotActivated = true;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _executingTask = ExecuteAsync(_stoppingCts.Token);
            // se a tarefa foi concluida então retorna,
            // isto causa o cancelamento e a falha do chamados
            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }
            // de outra forma ela esta rodando
            return Task.CompletedTask;
        }
        protected virtual async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //Esta Task é apenas para o service ficar rodando como HostedService
            do
            {
                if (telegramBotRunning && Settings.TelegramBotActivated == false)
                {
                    stopReceiving();
                    await HubSendMessage(new Telegram.WebAPI.Hubs.Models.ChatMessage("Server status", "O server foi parado.", DateTime.Now), false);
                }
                else if (telegramBotRunning == false && Settings.TelegramBotActivated)
                {
                    startReceiving();
                    await HubSendMessage(new Telegram.WebAPI.Hubs.Models.ChatMessage("Server status", "O server foi iniciado.", DateTime.Now), false);
                }

                if (telegramBotRunning)
                {
                    await _reminderApplication.SendReminders();
                    await _riverLevelApp.SendRiverLevel();
                    await HubSendMessage(new Telegram.WebAPI.Hubs.Models.ChatMessage("Server info", "checando mensagens", DateTime.Now), false);
                }

                await Task.Delay(60000, stoppingToken);
            }
            while (!stoppingToken.IsCancellationRequested);
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void botMessageReceiver(object sender, MessageEventArgs e)
        {
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                _telegramBotApplication.PrepareQuestionnaires(e);
            }   
        }
        private void startReceiving()
        {
            bot.StartReceiving();
            telegramBotRunning = true;
        }
        private void stopReceiving()
        {
            bot.StopReceiving();
            telegramBotRunning = false;
        }
        private async Task HubSendMessage(ChatMessage chatMessage, bool limitUsername)
        {
            if (limitUsername && chatMessage.User != null)
            {
                //Limitar o nome do usuário em 2 caracteres para não expor o nome completo
                chatMessage.User = chatMessage.User.Substring(0, 2);
            }
            await _chatHub.Clients.All.ReceiveMessage(chatMessage);
        }
    }
}