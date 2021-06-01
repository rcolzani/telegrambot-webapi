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

namespace Telegram.WebAPI.services
{
    public class TelegramBotService : IHostedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _token = Functions.Settings.TelegramToken;
        private TelegramBotClient bot;
        private string lastRiverLevel;
        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
        private readonly IHubContext<ChatHub, IChatClient> _chatHub;
        private bool telegramBotRunning = false;
        public TelegramBotService(IUnitOfWork unitOfWork, IHubContext<ChatHub, IChatClient> chatHub)
        {
            _unitOfWork = unitOfWork;
            _chatHub = chatHub;
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
                    sendRemindersAndRiverLevel();
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
                PrepareQuestionnaires(e);
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
        private async void PrepareQuestionnaires(MessageEventArgs e)
        {
            int chatId = (int)e.Message.Chat.Id;
            await HubSendMessage(new Telegram.WebAPI.Hubs.Models.ChatMessage(e.Message.Chat.FirstName, e.Message.Text, e.Message.Date), true);

            bool isNewCliente = false;
            var clientChat = _unitOfWork.TelegramUsers.AddClient(chatId, out isNewCliente);

            //Adiciona mensagem no banco de dados
            _unitOfWork.MessageHistorys.Add(new MessageHistory(clientChat.Id, e.Message.Text, e.Message.Date, false));
            _unitOfWork.Complete();

            string messageReceived = Functions.Generic.RemoveAccents(e.Message.Text.ToLower());
            if (messageReceived == "ola" || messageReceived == "/start")
            {
                ReceivedMessageHello(clientChat.Id, e.Message.Chat.FirstName, isNewCliente);
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
            else if (clientChat.Status == Domain.Enums.TelegramUserStatus.WaitingForTextMessage)
            {
                ReceivedMessageReminderText(clientChat.Id, messageReceived);
            }
            else if (clientChat.Status == Domain.Enums.TelegramUserStatus.WaitingForTime)
            {
                ReceivedMessageReminderTime(clientChat.Id, messageReceived);
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
                //clientToSave.RemindTimeToSend = sendTime;
                //clientToSave.Status = (int)clientStatus.complete;
                //if (DateTime.Now.Date + sendTime < DateTime.Now)
                //    clientToSave.LastSend = DateTime.Now; //Se o horário do lembrete é um horário que hoje já passou, registra como já enviado o lembrete. Se isso não for feito, será reenviado no próximo ciclo
                //await sendMessageAsync(clientToSave.TelegramChatId, $"Cadastro criado com sucesso!!!{Environment.NewLine}{Environment.NewLine}Você receberá a mensagem: {clientToSave.TextMessage}{Environment.NewLine}Todos os dias as {clientToSave.RemindTimeToSend.ToString()}");
            }
            else
            {
                await sendMessageAsync(clientToSave.TelegramChatId, "Não reconheço este formato de horário. O horário precisa estar no formato HH:MM");
            }
            _unitOfWork.TelegramUsers.Update(clientToSave);
            _unitOfWork.Complete();
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
            //clientToSave.TextMessage = reminderTextMessage;
            clientToSave.Status = Domain.Enums.TelegramUserStatus.WaitingForTime;
            await sendMessageAsync(clientToSave.TelegramChatId, "Qual horário você deseja ser lembrado? Precisa ser no formato HH:MM!");

            _unitOfWork.TelegramUsers.Update(clientToSave);
            _unitOfWork.Complete();
        }
        private async void ReceivedMessageStopReceiver(int clientId)
        {
            var clientToSave = _unitOfWork.TelegramUsers.GetCliente(clientId);
            //clientToSave.Activated = false;
            //clientToSave.RiverLevel = false;
            //clientToSave.TextMessage = "";
            clientToSave.Status = Domain.Enums.TelegramUserStatus.NewCliente;
            //clientToSave.LastSend = new DateTime();
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

        private async Task<bool> sendMessageAsync(int clientId, string text, IReplyMarkup replyMarkup = null)
        {
            var client = _unitOfWork.TelegramUsers.GetCliente(clientId, true);
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
        public async void sendRemindersAndRiverLevel()
        {
            try
            {
                bool enviarNivel = false;
                string riverLevelHour = "", riverLevel = "";
                RiverLevelAlertaBlu(out riverLevel, out riverLevelHour);

                if (string.IsNullOrEmpty(riverLevel) || string.IsNullOrEmpty(riverLevelHour))
                {
                    //As vezes acontece de o site da prefeitura estar com algum dado vazio e não pode ser enviado nesses casos
                }
                else if (lastRiverLevel != riverLevel + riverLevelHour)
                {
                    enviarNivel = true;
                    lastRiverLevel = riverLevel + riverLevelHour;
                }

                foreach (var client in _unitOfWork.TelegramUsers.GetAllClientes())
                {
                    ////enviar nível do rio
                    //if (enviarNivel && client.RiverLevel)
                    //{
                    //    await sendMessageAsync(client.TelegramChatId, $"O nível do rio está {riverLevel} às {riverLevelHour}");
                    //}
                    ////enviar lembretes criados
                    //if (client.TextMessage != "" && client.Status == (int)clientStatus.complete && client.Activated == true) //apenas clientes com o cadastro de um lembrete completo
                    //{
                    //    if ((DateTime.Now.Date + client.RemindTimeToSend) <= DateTime.Now && client.LastSend.Date < DateTime.Now.Date) //considerar enviar 
                    //    {
                    //        if (client.LastSend == new DateTime())
                    //        {
                    //            await sendMessageAsync(client.TelegramChatId, client.TextMessage);
                    //        }
                    //        else if (client.LastSend.AddMinutes(-5) < DateTime.Now)
                    //        {
                    //            await sendMessageAsync(client.TelegramChatId, client.TextMessage);
                    //        }
                    //        client.LastSend = DateTime.Now;
                    //        _repo.Update(client);
                    //        _repo.SaveChanges();
                    //    }
                    //}

                }
            }
            catch (Exception e)
            {
                Functions.Generic.LogException(e);
            }
        }
        private void RiverLevelAlertaBlu(out string riverLevel, out string riverLevelHour)
        {
            riverLevel = "";
            riverLevelHour = "";
            try
            {
                string siteContent = string.Empty;
                string url = "http://alertablu.cob.sc.gov.br/d/nivel-do-rio";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader streamReader = new StreamReader(responseStream))
                {
                    siteContent = streamReader.ReadToEnd();
                }
                var document = new HtmlDocument();
                document.LoadHtml(siteContent);
                var nodes = document.DocumentNode.SelectNodes("//*[@id='river-level-table']/tbody/tr/td");

                if (nodes.Count < 2)
                {
                    return;
                }

                riverLevelHour = nodes[0].InnerText.Trim();
                riverLevel = nodes[1].InnerText.Trim();
            }
            catch (Exception e)
            {
                Functions.Generic.LogException(e);
            }

        }
     
    }
}