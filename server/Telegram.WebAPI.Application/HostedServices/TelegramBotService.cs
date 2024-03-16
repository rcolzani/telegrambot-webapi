using Functions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.WebAPI.Application.Hubs.Models;
using Telegram.WebAPI.Application.Hubs.Models.Interfaces;
using Telegram.WebAPI.Application.Services;
using Telegram.WebAPI.Domain.Interfaces.Application;
using Telegram.WebAPI.Hubs;
using Telegram.WebAPI.Hubs.Clients;
using Telegram.WebAPI.Shared.Extensions;

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
        private IReminderApplication _reminderApplication;
        private IRiverLevelApplication _riverLevelApp;

        public TelegramBotService(IHubContext<ChatHub,
            IChatClient> chatHub,
            TelegramBotApplication telegramBotApplication,
            IReminderApplication reminderApplication,
            IRiverLevelApplication riverLevelApp,
            ILogger<TelegramBotService> logger)
        {
            _logger = logger;
            _chatHub = chatHub;

            _telegramBotApplication = telegramBotApplication;
            _telegramBotApplication.bot = new TelegramBotClient(_token);

            _reminderApplication = reminderApplication;
            _riverLevelApp = riverLevelApp;

            bot = new TelegramBotClient(_token);

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
            };

            using CancellationTokenSource cts = new();

            bot.StartReceiving(botMessageReceiver,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token);

            Settings.TelegramBotActivated = true;
        }
        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
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
                    if (telegramBotRunning && Settings.TelegramBotActivated == false)
                    {
                        //stopReceiving();
                        await HubSendMessage(new MessageSystem("Server status", "O server foi parado.", DateTime.Now), false);
                    }
                    else if (telegramBotRunning == false && Settings.TelegramBotActivated)
                    {
                        //startReceiving();
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
                        if (riverLevelNextSend < DateTime.Now)
                        {
                            await _riverLevelApp.SendRiverLevel();
                            riverLevelNextSend = DateTime.Now.AddMinutes(3);
                        }
                    }

                    //await Task.Delay(500, stoppingToken);
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
        async Task botMessageReceiver(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var receivedMessage = new TelegramMessageUpdate()
            {
                Message = new TelegramMessage()
                {
                    chatDateTime = update.Message.Date,
                    chatFirstName = update.Message.Chat.FirstName,
                    chatLastName = update.Message.Chat.LastName,
                    chatId = update.Message.Chat.Id,
                    Text = update.Message.Text
                }
            };
            await _telegramBotApplication.PrepareQuestionnaires(receivedMessage);
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
    }
}