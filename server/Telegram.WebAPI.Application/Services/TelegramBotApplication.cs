using Functions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.WebAPI.Application.Hubs.Models;
using Telegram.WebAPI.Application.Hubs.Models.Interfaces;
using Telegram.WebAPI.Domain.Entities;
using Telegram.WebAPI.Domain.Interfaces;
using Telegram.WebAPI.Hubs;
using Telegram.WebAPI.Hubs.Clients;
using Telegram.WebAPI.Hubs.Models;
using Telegram.WebAPI.Shared.Extensions;

namespace Telegram.WebAPI.Application.Services
{
    public class TelegramBotApplication
    {
        public TelegramBotClient bot;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<ChatHub, IChatClient> _chatHub;
        ILogger<TelegramBotApplication> _logger;

        public TelegramBotApplication(IUnitOfWork unitOfWork, IHubContext<ChatHub, IChatClient> chatHub, ILogger<TelegramBotApplication> logger)
        {
            _chatHub = chatHub;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<bool> PrepareQuestionnaires(MessageEventArgs e)
        {
            try
            {
                int chatId = (int)e.Message.Chat.Id;
                bool isNewUser = false;

                _logger.LogInformation(DateTime.Now + ": Começou a adicionar o usuário");
                var user = _unitOfWork.TelegramUsers.AddUser(chatId, out isNewUser, $"{e.Message.Chat.FirstName.FirstCharToUpper()} {e.Message.Chat.LastName.FirstCharToUpper()}".Trim());
                _logger.LogInformation(DateTime.Now + ": Adicionou o usuário");

                var userLastReminder = user.Reminders?.OrderByDescending(r => r.CreatedAt).FirstOrDefault();

                string messageReceived = e.Message.Text.ToLower().RemoveAccents();

                if (messageReceived == "ola" || messageReceived == "/start")
                {
                    ReceivedMessageHello(user, isNewUser);
                }
                else if (userLastReminder != null && userLastReminder.Status == Domain.Enums.ReminderStatus.WaitingForTextMessage)
                {
                    ReceivedMessageReminderText(user, userLastReminder, messageReceived);
                }
                else if (userLastReminder != null && userLastReminder.Status == Domain.Enums.ReminderStatus.WaitingForTime)
                {
                    ReceivedMessageReminderTime(user, userLastReminder, messageReceived);
                }
                else if (messageReceived == "iniciar")
                {
                    ReceivedMessageStart(user);
                }
                else if (messageReceived == "lembrete")
                {
                    ReceivedMessageReminder(user);
                }
                else if (messageReceived == "nivel do rio")
                {
                    ReveivedMessageRiverLevel(user);
                }
                else if (messageReceived == "consultar")
                {
                    ReceivedMessageConsult(user);
                }
                else if (messageReceived == "parar")
                {
                    ReceivedMessageStopReceiver(user);
                }
                else
                {
                    ReceivedMessageCommandNotUnderstand(user);
                }

                //O envio de mensagens para o front e a gravação da mensagem recebida devem ser feitas após o processamento da mensagem e envio de resposta ao cliente
                //assim fica mais rápida a troca de mensagens com o usuário
                await HubSendMessage(new MessageClient(e.Message.Chat.FirstName, e.Message.Text, e.Message.Date), true);

                //Adiciona mensagem no banco de dados
                _unitOfWork.MessageHistorys.Add(new MessageHistory(user.Id, e.Message.Text, e.Message.Date, false));
                _unitOfWork.Complete();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now} : {ex.ToString()}");
                return false;
            }
        }
        private async void ReceivedMessageReminderTime(TelegramUser user, Reminder reminder, string messageReceived)
        {
            try
            {
                TimeSpan sendTime = new TimeSpan();
                if (TimeSpan.TryParse(messageReceived, out sendTime))
                {
                    if (reminder == null)
                    {
                        await sendMessageAsync(user.TelegramChatId, $"Cadastro falhou. Inicie novamente.");
                        return;
                    }
                    reminder.SetTimeToRemind(sendTime);
                    await sendMessageAsync(user.TelegramChatId, $"Cadastro criado com sucesso!!!{Environment.NewLine}{Environment.NewLine}Você receberá a mensagem: {reminder.TextMessage}{Environment.NewLine}Todos os dias as {reminder.RemindTimeToSend.ToString()}");
                    _unitOfWork.Reminders.Update(reminder);
                    _unitOfWork.TelegramUsers.Update(user);
                }
                else
                {
                    await sendMessageAsync(user.TelegramChatId, "Não reconheço este formato de horário. O horário precisa estar no formato HH:MM");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now} : {ex.ToString()}");
            }
        }

        private async void ReceivedMessageReminder(TelegramUser user)
        {
            try
            {
                var keyboard = new ReplyKeyboardMarkup
                {
                    Keyboard = new[]
                             {
                        new[]
                        {
                            new KeyboardButton("Iniciar")
                        },
                        new []{
                            new KeyboardButton("Consultar")
                        }
                    }
                };

                string texto = $"Selecione uma das opções no teclado que apareceu para você ou digite:{Environment.NewLine}" +
                    $"*Iniciar* - para iniciar o cadastro de um lembrete{Environment.NewLine}" +
                    $"*Consultar* - para consultar os lembretes ativos{Environment.NewLine}";

                await sendMessageAsync(user.TelegramChatId, texto, keyboard);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now} : {ex.ToString()}");
            }
        }
        private async void ReceivedMessageReminderText(TelegramUser user, Reminder reminder, string reminderTextMessage)
        {
            try
            {
                if (reminderTextMessage == null)
                    return;

                await sendMessageAsync(user.TelegramChatId, "Qual horário você deseja ser lembrado? Precisa ser no formato HH:MM!");
                reminder.AddReminderText(reminderTextMessage);
                _unitOfWork.Reminders.Update(reminder);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now} : {ex.ToString()}");
            }

        }
        private async void ReceivedMessageStopReceiver(TelegramUser user)
        {
            try
            {
                user.StopReminders();
                await sendMessageAsync(user.TelegramChatId, "Removido da fila de envio. Para voltar a receber lembretes ou alertas do nível do rio, envie Olá para iniciar o cadastro.");
                _unitOfWork.TelegramUsers.Update(user);
                //_unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now} : {ex.ToString()}");
            }
        }
        private async void ReceivedMessageConsult(TelegramUser user)
        {
            try
            {
                string remindersConcat = "";

                var reminders = _unitOfWork.Reminders.GetAllRemindersActiveByUser(user.Id);

                foreach (var reminder in reminders)
                {
                    remindersConcat += $"{reminder.TextMessage} às {reminder.RemindTimeToSend}\n\n";

                }
                if (remindersConcat == "")
                {
                    var keyboard = new ReplyKeyboardMarkup
                    {
                        Keyboard = new[]
                        {
                        new[]
                        {
                            new KeyboardButton("Iniciar")
                        }
                    }
                    };
                    remindersConcat = "Você ainda não tem lembretes cadastrados. Que tal iniciar o cadastro de um novo lembrete?";
                    await sendMessageAsync(user.TelegramChatId, remindersConcat, keyboard);
                }
                else
                {
                    await sendMessageAsync(user.TelegramChatId, remindersConcat);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now} : {ex.ToString()}");
            }

        }
        private async void ReceivedMessageCommandNotUnderstand(TelegramUser user)
        {
            await sendMessageAsync(user.TelegramChatId, $"Não consegui entender este comando :/ Os comandos disponíveis são:{Environment.NewLine}olá - para iniciar a conversa{Environment.NewLine}iniciar - para iniciar o cadastro de um lembrete{Environment.NewLine}parar - para parar o recebimento de lembretes");
        }
        private async void ReceivedMessageHello(TelegramUser user, bool isNewCliente)
        {
            try
            {
                var keyboard = new ReplyKeyboardMarkup
                {
                    Keyboard = new[]
                 {
                        new[]
                        {
                            new KeyboardButton("Lembrete")
                        },
                        new []{
                            new KeyboardButton("Nível do rio")
                        },
                        new[]
                        {
                            new KeyboardButton("Parar")
                        }
                    }
                };

                string texto = $"Olá {user.Name}, {Environment.NewLine}{Environment.NewLine}";

                if (isNewCliente)
                {
                    texto += $"Vejo que é sua primeira vez aqui. Seja muito bem vindo!!!{Environment.NewLine}Sou um robô criado para lembrar você do que for preciso. Basta você criar um lembrete que eu" +
                        $" aviso você para tomar água, se medicar, tirar o lixo... Você só precisar seguir as instruções abaixo e não esquecerei de você hehe{Environment.NewLine}{Environment.NewLine}";
                }

                texto += $"Selecione uma das opções no teclado que apareceu para você ou digite:{Environment.NewLine}" +
                    $"*Lembrete* - para acessar as opções de lembretes{Environment.NewLine}" +
                    $"*Nível do rio* - para saber em tempo real quando uma nova medição foi atualizada no site da defesa civil de Blumenau{Environment.NewLine}" +
                    $"*Parar* - para não receber mais lembretes e alertas sobre o nível do rio{Environment.NewLine}";

                await sendMessageAndHideUserAsync(user.TelegramChatId, texto, user.Name,keyboard );
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now} : {ex.ToString()}");
            }

        }
        private async void ReceivedMessageStart(TelegramUser user)
        {
            try
            {
                await sendMessageAsync(user.TelegramChatId, "Qual mensagem você deseja receber ao ser lembrado?");

                var reminder = new Reminder(user.Id, "");
                _unitOfWork.Reminders.Add(reminder);
                //_unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now} : {ex.ToString()}");

            }

        }
        private async void ReveivedMessageRiverLevel(TelegramUser user)
        {
            try
            {
                await sendMessageAsync(user.TelegramChatId, "Feito!\n\nVocê começará a receber as medições do nível do rio a partir de agora.");
                user.StartReceiveRiverLevel();
                _unitOfWork.TelegramUsers.Update(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now} : {ex.ToString()}");
            }
        }
        public async Task<bool> sendMessageAndHideUserAsync(long telegramClientId, string text, string userNameToHide,IReplyMarkup replyMarkup = null)
        {
            return await _sendMessageAsync(telegramClientId, text, replyMarkup, userNameToHide);
        }
        public async Task<bool> sendMessageAsync(long telegramClientId, string text, IReplyMarkup replyMarkup = null)
        {
            return await _sendMessageAsync(telegramClientId, text, replyMarkup);
        }
        private async Task<bool> _sendMessageAsync(long telegramClientId, string text, IReplyMarkup replyMarkup = null, string userNameToHide = "")
        {
            try
            {
                if (text == null) { return false; }

                if (replyMarkup == null) { replyMarkup = new ReplyKeyboardRemove() { }; }

                var messageSent = await bot.SendTextMessageAsync(telegramClientId, text, Telegram.Bot.Types.Enums.ParseMode.Markdown, false, false, 0, replyMarkup);

                if (string.IsNullOrWhiteSpace(userNameToHide) == false)
                {
                    text = text.Replace(userNameToHide, "[nome do usuário]");
                }

                await HubSendMessage(new MessageSystem("Sistema", text, DateTime.Now), false);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"{DateTime.Now} : {e.ToString()}");
                return false;
            }
        }
        private async Task HubSendMessage(MessageBase chatMessage, bool limitUsername)
        {
            try
            {
                if (limitUsername && chatMessage.User != null)
                {
                    //Limitar o nome do usuário em 2 caracteres para não expor o nome completo
                    chatMessage.User = chatMessage.User.Substring(0, 2);
                }

                await _chatHub.Clients.All.ReceiveMessage(chatMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now} : {ex.ToString()}");
            }
        }
    }
}
