using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.WebAPI.Domain.Entities;
using Telegram.WebAPI.Domain.Interfaces;
using Telegram.WebAPI.Hubs;
using Telegram.WebAPI.Hubs.Clients;
using Telegram.WebAPI.Hubs.Models;

namespace Telegram.WebAPI.Application
{
    public class TelegramBotApplication
    {
        public TelegramBotClient bot;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<ChatHub, IChatClient> _chatHub;
      
        public TelegramBotApplication(IUnitOfWork unitOfWork, IHubContext<ChatHub, IChatClient> chatHub)
        {
            _chatHub = chatHub;
            _unitOfWork = unitOfWork;
        }
        public async void PrepareQuestionnaires(MessageEventArgs e)
        {
            int chatId = (int)e.Message.Chat.Id;
            await HubSendMessage(new Telegram.WebAPI.Hubs.Models.ChatMessage(e.Message.Chat.FirstName, e.Message.Text, e.Message.Date), true);

            bool isNewCliente = false;
            var clientChat = _unitOfWork.TelegramUsers.AddClient(chatId, out isNewCliente,  $"{e.Message.Chat.FirstName} {e.Message.Chat.LastName}".Trim());

            //Adiciona mensagem no banco de dados
            _unitOfWork.MessageHistorys.Add(new MessageHistory(clientChat.Id, e.Message.Text, e.Message.Date, false));
            _unitOfWork.Complete();

            string messageReceived = Functions.Generic.RemoveAccents(e.Message.Text.ToLower());
            if (messageReceived == "ola" || messageReceived == "/start")
            {
                ReceivedMessageHello(clientChat.Id, e.Message.Chat.FirstName, isNewCliente);
            }
            else if (clientChat.Status == Domain.Enums.TelegramUserStatus.WaitingForTextMessage)
            {
                ReceivedMessageReminderText(clientChat.Id, messageReceived);
            }
            else if (clientChat.Status == Domain.Enums.TelegramUserStatus.WaitingForTime)
            {
                ReceivedMessageReminderTime(clientChat.Id, messageReceived);
            }
            else if (messageReceived == "iniciar")
            {
                ReceivedMessageStart(clientChat.Id);
            }
            else if (messageReceived == "lembrete")
            {
                ReceivedMessageReminder(clientChat.Id);
            }
            else if (messageReceived == "sair")
            {
                ReceivedMessageExitMenu(clientChat.Id);
            }
            else if (messageReceived == "nivel do rio")
            {
                ReveivedMessageRiverLevel(clientChat.Id);
            }
            else if (messageReceived == "consultar")
            {
                ReceivedMessageConsult(clientChat.Id);
            }
            else if (messageReceived == "parar")
            {
                ReceivedMessageStopReceiver(clientChat.Id);
            }
            else
            {
                ReceivedMessageCommandNotUnderstand(clientChat.Id);
            }
        }
        private async void ReceivedMessageReminderTime(int clientId, string messageReceived)
        {
            var clientToSave = _unitOfWork.TelegramUsers.GetCliente(clientId);
            TimeSpan sendTime = new TimeSpan();
            if (TimeSpan.TryParse(messageReceived, out sendTime))
            {
                var reminder = clientToSave.Reminders.Where(r => r.Status == Domain.Enums.ReminderStatus.WaitingForTime).FirstOrDefault();
                
                if(reminder == null)
                {
                    await sendMessageAsync(clientToSave.TelegramChatId, $"Cadastro falhou. Inicie novamente.");
                    return;
                }
                reminder.SetTimeToRemind(sendTime);
                clientToSave.Status = Domain.Enums.TelegramUserStatus.Complete;
                await sendMessageAsync(clientToSave.TelegramChatId, $"Cadastro criado com sucesso!!!{Environment.NewLine}{Environment.NewLine}Você receberá a mensagem: {reminder.TextMessage}{Environment.NewLine}Todos os dias as {reminder.RemindTimeToSend.ToString()}");
                _unitOfWork.Reminders.Update(reminder);
                _unitOfWork.TelegramUsers.Update(clientToSave);
                _unitOfWork.Complete();
            }
            else
            {
                await sendMessageAsync(clientToSave.TelegramChatId, "Não reconheço este formato de horário. O horário precisa estar no formato HH:MM");
            }
        }
        public async Task<bool> sendMessageAsync(int userId, string text, IReplyMarkup replyMarkup = null)
        {
            var client = _unitOfWork.TelegramUsers.GetCliente(userId, true);
            if (client == null)
            {
                return false;
            }
            return await sendMessageAsync(client.TelegramChatId, text, replyMarkup);
        }
        private async Task<bool> sendMessageAsync(long telegramClientId, string text, IReplyMarkup replyMarkup = null)
        {
            try
            {
                if (text == null)
                    return false;

                if (replyMarkup == null)
                    replyMarkup = new ReplyKeyboardRemove() { };

                var messageSent = await bot.SendTextMessageAsync(telegramClientId, text, Telegram.Bot.Types.Enums.ParseMode.Markdown, false, false, 0, replyMarkup);
                await HubSendMessage(new Telegram.WebAPI.Hubs.Models.ChatMessage("Sistema", text, DateTime.Now), false);
                return true;
            }
            catch (Exception e)
            {
                Functions.Generic.LogException(e);
                return false;
            }
        }
        private async void ReceivedMessageReminder(int clientId)
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
                        },
                        new[]
                        {
                            new KeyboardButton("Sair")
                        }
                    }
            };

            string texto = $"Selecione uma das opções no teclado que apareceu para você ou digite:{Environment.NewLine}" +
                $"*Iniciar* - para iniciar o cadastro de um lembrete{Environment.NewLine}" +
                $"*Consultar* - para consultar os lembretes ativos{Environment.NewLine}" +
                "*Sair* - para sair do menu";

            await sendMessageAsync(clientId, texto, keyboard);
        }
        private async void ReceivedMessageReminderText(int clientId, string reminderTextMessage)
        {
            if (reminderTextMessage == null)
                return;

            var clientToSave = _unitOfWork.TelegramUsers.GetCliente(clientId);
            var reminder = new Reminder(reminderTextMessage);
            await sendMessageAsync(clientToSave.TelegramChatId, "Qual horário você deseja ser lembrado? Precisa ser no formato HH:MM!");
            _unitOfWork.Reminders.Update(reminder);
            _unitOfWork.Complete();
        }
        private async void ReceivedMessageStopReceiver(int clientId)
        {
            var clientToSave = _unitOfWork.TelegramUsers.GetCliente(clientId);
            clientToSave.StopReminders();
            await sendMessageAsync(clientToSave.TelegramChatId, "Removido da fila de envio. Para voltar a receber lembretes ou alertas do nível do rio, envie Olá para iniciar o cadastro.");
            _unitOfWork.TelegramUsers.Update(clientToSave);
            _unitOfWork.Complete();
        }
        private async void ReceivedMessageExitMenu(int clientId)
        {
            await sendMessageAsync(clientId, $"Você saiu do menu.{Environment.NewLine}{Environment.NewLine}Para voltar a conversar comigo diga Olá");
        }
        private async void ReceivedMessageConsult(int clientId)
        {
            string lembretes = "";
            var clientToSave = _unitOfWork.TelegramUsers.GetCliente(clientId);
            if (clientToSave != null)
            {
                //if (clientToSave.TextMessage != "")
                //    lembretes = $"Lembrete {clientToSave.TextMessage} às {clientToSave.RemindTimeToSend}";
            }
            if (lembretes == "")
            {
                var keyboard = new ReplyKeyboardMarkup
                {
                    Keyboard = new[]
                  {
                        new[]
                        {
                            new KeyboardButton("Iniciar")
                        },
                        new[]
                        {
                            new KeyboardButton("Sair")
                        }
                    }
                };
                lembretes = "Você ainda não tem lembretes cadastrados. Que tal iniciar o cadastro de um novo lembrete?";
                await sendMessageAsync(clientToSave.TelegramChatId, lembretes, keyboard);
            }
            else
            {
                await sendMessageAsync(clientToSave.TelegramChatId, lembretes);
            }
            _unitOfWork.TelegramUsers.Update(clientToSave);
            _unitOfWork.Complete();
        }
        private async void ReceivedMessageCommandNotUnderstand(int clientId)
        {
            await sendMessageAsync(clientId, $"Não consegui entender este comando :/ Os comandos disponíveis são:{Environment.NewLine}olá - para iniciar a conversa{Environment.NewLine}iniciar - para iniciar o cadastro de um lembrete{Environment.NewLine}parar - para parar o recebimento de lembretes");
        }
        private async void ReceivedMessageHello(int clientId, string clientFirstName, bool isNewCliente)
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
                        },
                        new[]
                        {
                            new KeyboardButton("Sair")
                        }
                    }
            };

            string texto = $"Olá {clientFirstName}, {Environment.NewLine}{Environment.NewLine}";

            if (isNewCliente)
            {
                texto += $"Vejo que é sua primeira vez aqui. Seja muito bem vindo!!!{Environment.NewLine}Sou um robô criado para lembrar você do que for preciso. Basta você criar um lembrete que eu" +
                    $" aviso você para tomar água, se medicar, tirar o lixo... Você só precisar seguir as instruções abaixo e não esquecerei de você hehe{Environment.NewLine}{Environment.NewLine}";
            }

            texto += $"Selecione uma das opções no teclado que apareceu para você ou digite:{Environment.NewLine}" +
                $"*Lembrete* - para acessar as opções de lembretes{Environment.NewLine}" +
                $"*Nível do rio* - para saber em tempo real quando uma nova medição foi atualizada no site da defesa civil de Blumenau{Environment.NewLine}" +
                $"*Parar* - para não receber mais lembretes e alertas sobre o nível do rio{Environment.NewLine}" +
                "*Sair* - para sair do menu";

            await sendMessageAsync(clientId, texto, keyboard);
        }
        private async void ReceivedMessageStart(int clientId)
        {
            await sendMessageAsync(clientId, "Qual mensagem você deseja receber ao ser lembrado?");
            var clientChatToSave = _unitOfWork.TelegramUsers.GetCliente(clientId);
            // clientChatToSave.Activated = true;
            clientChatToSave.Status = Domain.Enums.TelegramUserStatus.WaitingForTextMessage;
            _unitOfWork.TelegramUsers.Update(clientChatToSave);
            _unitOfWork.Complete();
        }
        private async void ReveivedMessageRiverLevel(int clientId)
        {
            var clientChatToSave = _unitOfWork.TelegramUsers.GetCliente(clientId);
            clientChatToSave.SendRiverLevel = true;
            await sendMessageAsync(clientChatToSave.TelegramChatId, "Feito!\n\nVocê começará a receber as medições do nível do rio a partir de agora.");
            _unitOfWork.TelegramUsers.Update(clientChatToSave);
            _unitOfWork.Complete();
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
