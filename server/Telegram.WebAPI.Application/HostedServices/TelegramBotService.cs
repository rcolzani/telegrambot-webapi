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
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Linq;
using Telegram.WebAPI.Shared.Extensions;
using Telegram.WebAPI.Application.Services;
using Telegram.WebAPI.Application.Hubs.Models;
using Telegram.WebAPI.Application.Hubs.Models.Interfaces;

namespace Telegram.WebAPI.HostedServices
{
    /// <summary>
    /// Service responsável por receber mensagens e executar as aplicações de Lembrete (reminder) e nível do rio ()
    /// </summary>
    public class TelegramBotService : IHostedService
    {
        private readonly string _token = Functions.Settings.TelegramToken;
        ILogger<TelegramBotService> _logger;
        private TelegramBotClient bot;

        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
        private readonly IHubContext<ChatHub, IChatClient> _chatHub;
        private bool telegramBotRunning = false;
        private TelegramBotApplication _telegramBotApplication;
        private ReminderApplication _reminderApplication;
        private RiverLevelApplication _riverLevelApp;

        private List<ReceivedMessage> receivedMessagesToProcess;
        public TelegramBotService(IHubContext<ChatHub, IChatClient> chatHub, TelegramBotApplication telegramBotApplication, ReminderApplication reminderApplication, RiverLevelApplication riverLevelApp, ILogger<TelegramBotService> logger)
        {
            _logger = logger;
            _chatHub = chatHub;

            _telegramBotApplication = telegramBotApplication;
            _telegramBotApplication.bot = new TelegramBotClient(_token);

            _reminderApplication = reminderApplication;
            _riverLevelApp = riverLevelApp;

            bot = new TelegramBotClient(_token);
            bot.OnMessage += botMessageReceiver;
            Settings.TelegramBotActivated = true;

            receivedMessagesToProcess = new List<ReceivedMessage>();
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _executingTask = ExecuteAsync(_stoppingCts.Token);
            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }
            return Task.CompletedTask;
        }
        protected virtual async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Entrou na thread");
                DateTime riverLevelNextSend = DateTime.Now;
                DateTime reminderNextSend = DateTime.Now;
                //Esta Task é apenas para o service ficar rodando como HostedService
                do
                {
                    foreach (var mensagem in receivedMessagesToProcess.Where(m => m.IsProcessed.Equals(false)).ToList())
                    {
                        await _telegramBotApplication.PrepareQuestionnaires(mensagem.Message);
                        mensagem.SetAsProcessed();
                    }

                    if (telegramBotRunning && Settings.TelegramBotActivated == false)
                    {
                        stopReceiving();
                        await HubSendMessage(new MessageSystem("Server status", "O server foi parado.", DateTime.Now), false);
                    }
                    else if (telegramBotRunning == false && Settings.TelegramBotActivated)
                    {
                        startReceiving();
                        await HubSendMessage(new MessageSystem("Server status", "O server foi iniciado.", DateTime.Now), false);
                    }

                    if (telegramBotRunning)
                    {
                        if (reminderNextSend < DateTime.Now)
                        {
                            await _reminderApplication.SendReminders();
                            reminderNextSend = DateTime.Now.AddMinutes(1);
                            await HubSendMessage(new MessageSystem("Server information", "Checando lembretes para enviar", DateTime.Now), false);
                        }
                        if (riverLevelNextSend< DateTime.Now)
                        {
                            await _riverLevelApp.SendRiverLevel();
                            riverLevelNextSend = DateTime.Now.AddMinutes(3);
                        }
                     }

                    await Task.Delay(500, stoppingToken);
                }
                while (!stoppingToken.IsCancellationRequested);
            }
            catch (Exception ex)
            {
                ex.LogExceptionToConsole();
                _logger.LogError(ex.ToString());
                throw;
            }
         
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void botMessageReceiver(object sender, MessageEventArgs e)
        {
            //Adiciona mensagem na lista de mensagens recebidas ao receber uma mensagem.
            //Esta lista é processada em outra task, que responserá a mensagem.
            //É feito desta forma para tornar o acesso ao banco thread safe
            receivedMessagesToProcess.Add(new ReceivedMessage(e));

            //Remove da fila as mensagens que já foram respondidadas
            receivedMessagesToProcess.RemoveAll(m => m.IsProcessed.Equals(true));
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
        private async Task HubSendMessage(MessageBase chatMessage, bool limitUsername)
        {
            if (limitUsername && chatMessage.User != null)
            {
                //Limitar o nome do usuário em 2 caracteres para não expor o nome completo
                chatMessage.User = chatMessage.User.Substring(0, 2);
            }
            await _chatHub.Clients.All.ReceiveMessage(chatMessage);
        }
        private class ReceivedMessage
        {
            public MessageEventArgs Message { get; private set; }
            public bool IsProcessed { get; private set; }

            public ReceivedMessage(MessageEventArgs message)
            {
                this.Message = message;
                IsProcessed = false;
            }

            public void SetAsProcessed()
            {
                this.IsProcessed = true;
            }
        }
    }
}