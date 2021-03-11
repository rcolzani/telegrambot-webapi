using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using server.GeneralFunctions;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.WebAPI.Data;
using Telegram.WebAPI.Hubs;
using Telegram.WebAPI.Hubs.Clients;
using Telegram.WebAPI.Models;

namespace Telegram.WebAPI.services
{
    public class TelegramBotService : IHostedService
    {
        private readonly IRepository _repo;
        private readonly string _token = "1580927578:AAHMaEQ44RwBI2LdFihYr6Joe_Y84bMVwbA";
        private static TelegramBotClient bot;
        private static string lastRiverLevel;
        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
        private readonly IHubContext<ChatHub, IChatClient> _chatHub;
        public TelegramBotService(IRepository repo, IHubContext<ChatHub, IChatClient> chatHub)
        {
            _repo = repo;
            _chatHub = chatHub;
            bot = new TelegramBotClient(_token);
            bot.OnMessage += botMessageReceiver;
            bot.StartReceiving();
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
                //Delay de 50 segundos
                sendMessagesIfNeeded();
                await _chatHub.Clients.All.ReceiveMessage(new server.Hubs.Models.ChatMessage("teste", "checando mensagens", DateTime.Now));

                await Task.Delay(30000, stoppingToken);
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
        private void stopReceiving()
        {
            bot.StopReceiving();
        }
        private async void PrepareQuestionnaires(MessageEventArgs e)
        {
            int chatId = (int)e.Message.Chat.Id;
            await _chatHub.Clients.All.ReceiveMessage(new server.Hubs.Models.ChatMessage(chatId.ToString(), e.Message.Text, e.Message.Date));
            //string jsonString = JsonSerializer.Serialize(e);
            //Functions.LogEvent($"MessageEvent: {jsonString}");
            //Functions.LogEvent($"Mensagem recebida {e.Message.Text} - do chat {chatId}");
            bool isNewCliente = false;
            var clientChat = await _repo.GetClienteByTelegramIdAsync(chatId);

            if (clientChat == null)
            {
                _repo.Add(new Cliente(chatId, "", (int)clientStatus.newCliente, new TimeSpan(08, 00, 00), DateTime.Now.AddDays(-1), false, false, DateTime.Now));
                _repo.SaveChanges();
                clientChat = await _repo.GetClienteByTelegramIdAsync(chatId);
                isNewCliente = true;
            }

            _repo.Add(new Mensagem(clientChat.Id, e.Message.Text, e.Message.Date, false));
            _repo.SaveChanges();
            string messageReceived = Functions.RemoveAccents(e.Message.Text.ToLower());
            if (messageReceived == "ola" || messageReceived == "/start")
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
                            new KeyboardButton("Consultar")
                        },
                        new[]
                        {
                            new KeyboardButton("Parar")
                        },
                        new []{
                            new KeyboardButton("Nível do rio")
                        },
                        new[]
                        {
                            new KeyboardButton("Sair")
                        }
                    }
                };

                string texto = $"Olá {e.Message.From.FirstName}, {Environment.NewLine}{Environment.NewLine}";

                if (isNewCliente)
                {
                    texto += $"Vejo que é sua primeira vez aqui. Seja muito bem vindo!!!{Environment.NewLine}Sou um robô criado para lembrar você do que for preciso. Basta você criar um lembrete que eu" +
                        $" aviso você para tomar água, se medicar, tirar o lixo... Você só precisar seguir as instruções abaixo e não esquecerei de você hehe{Environment.NewLine}{Environment.NewLine}";
                }

                texto += $"Selecione uma das opções no teclado que apareceu para você ou digite:{Environment.NewLine}" +
                    $"*Iniciar* - para começar a cadastrar um lembrete{Environment.NewLine}" +
                    $"*Consultar* - para consultar os lembretes ativos{Environment.NewLine}" +
                    $"*Parar* - para não receber mais{Environment.NewLine}" +
                    $"*Nível do rio* - para saber em tempo real quando uma nova medição foi atualizada no site da defesa civil de Blumenau{Environment.NewLine}" +
                    "*Sair* - para sair do menu";

                await sendMessageAsync(e.Message.Chat.Id, texto, keyboard);
            }
            else if (messageReceived == "iniciar")
            {
                clientChat.Activated = true;
                clientChat.Status = (int)clientStatus.waitingForTextMessage;
                await sendMessageAsync(e.Message.Chat.Id, "Qual mensagem você deseja receber ao ser lembrado?");
            }
            else if (messageReceived == "nivel do rio")
            {
                clientChat.RiverLevel = true;
                await sendMessageAsync(e.Message.Chat.Id, "Feito!\n\nVocê começará a receber as medições do nível do rio a partir de agora.");
            }
            else if (messageReceived == "consultar")
            {
                string lembretes = "";
                foreach (var chat in await _repo.GetAllClientesAsync())
                {
                    if (chat.TelegramChatId == chatId && chat.TextMessage != "")
                    {
                        if (lembretes != "")
                        {
                            lembretes += Environment.NewLine;
                        }
                        lembretes += $"Lembrete {chat.TextMessage} às {chat.RemindTimeToSend}";
                    }
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
                    await sendMessageAsync(e.Message.Chat.Id, lembretes, keyboard);
                }
                else
                {
                    await sendMessageAsync(e.Message.Chat.Id, lembretes);
                }
            }
            else if (messageReceived == "parar")
            {
                clientChat.Activated = false;
                clientChat.RiverLevel = false;
                clientChat.TextMessage = "";
                clientChat.Status = (int)clientStatus.newCliente;
                clientChat.LastSend = new DateTime();
                await sendMessageAsync(e.Message.Chat.Id, "Removido da fila de envio.");
            }
            else if (messageReceived == "sair")
            {
                await sendMessageAsync(e.Message.Chat.Id, $"Feito :D{Environment.NewLine}{Environment.NewLine}Para voltar a conversar comigo diga Olá");
                clientChat.Activated = false;
            }
            else if (clientChat.Status == (int)clientStatus.waitingForTextMessage)
            {
                if (messageReceived != null)
                {
                    clientChat.TextMessage = e.Message.Text;
                    clientChat.Status = (int)clientStatus.waitingForTime;
                    await sendMessageAsync(e.Message.Chat.Id, "Qual horário você deseja ser lembrado? Precisa ser no formato HH:MM!");
                }
            }
            else if (clientChat.Status == (int)clientStatus.waitingForTime)
            {
                TimeSpan sendTime = new TimeSpan();
                if (TimeSpan.TryParse(messageReceived, out sendTime))
                {
                    clientChat.RemindTimeToSend = sendTime;
                    clientChat.Status = (int)clientStatus.complete;

                    if (DateTime.Now.Date + sendTime < DateTime.Now)
                    {
                        clientChat.LastSend = DateTime.Now; //Se o horário do lembrete é um horário que hoje já passou, registra como já enviado o lembrete. Se isso não for feito, será reenviado no próximo ciclo
                    }

                    await sendMessageAsync(e.Message.Chat.Id, $"Cadastro criado com sucesso!!!{Environment.NewLine}{Environment.NewLine}Você receberá a mensagem: {clientChat.TextMessage}{Environment.NewLine}Todos os dias as {clientChat.RemindTimeToSend.ToString()}");
                }
                else
                {
                    await sendMessageAsync(e.Message.Chat.Id, "Não reconheço este formato de horário. O horário precisa estar no formato HH:MM");
                }
            }
            else
            {
                await sendMessageAsync(e.Message.Chat.Id, $"Não consegui entender este comando :/ Os comandos disponíveis são:{Environment.NewLine}olá - para iniciar a conversa{Environment.NewLine}iniciar - para iniciar o cadastro de um lembrete{Environment.NewLine}parar - para parar o recebimento de lembretes");
            }
            //clientChat.MessageHistory.Add(new Message { dateTimeMessage = e.Message.Date, MessageText = e.Message.Text, MessageId = e.Message.MessageId });
            _repo.Update(clientChat);
            _repo.SaveChanges();
        }
        private async static Task<bool> sendMessageAsync(long chatId, string text, IReplyMarkup replyMarkup = null)
        {
            try
            {
                if (text == null)
                    return false;

                if (replyMarkup == null)
                    replyMarkup = new ReplyKeyboardRemove() { };

                var messageSent = await bot.SendTextMessageAsync(chatId, text, Telegram.Bot.Types.Enums.ParseMode.Markdown, false, false, 0, replyMarkup);
                return true;
            }
            catch (Exception e)
            {
                // Functions.LogException(e);
                return false;
            }
        }
        public async void sendMessagesIfNeeded()
        {
            try
            {
                bool enviarNivel = false;
                string riverLevelHour = "", riverLevel = "";
                RiverLevelAlertaBlu(out riverLevel, out riverLevelHour);
                //if (String.IsNullOrEmpty(lastRiverLevel) == false && lastRiverLevel != riverLevel + riverLevelHour)
                if (lastRiverLevel != riverLevel + riverLevelHour)
                {
                    enviarNivel = true;
                }
                lastRiverLevel = riverLevel + riverLevelHour;

                foreach (var client in await _repo.GetAllClientesAsync())
                {
                    //enviar nível do rio
                    if (enviarNivel && client.RiverLevel)
                    {
                        await sendMessageAsync(client.TelegramChatId, $"O nível do rio está {riverLevel} às {riverLevelHour}");
                    }
                    //enviar lembretes criados
                    if (client.TextMessage != "" && client.Status == (int)clientStatus.complete && client.Activated == true) //apenas clientes com o cadastro de um lembrete completo
                    {
                        if ((DateTime.Now.Date + client.RemindTimeToSend) <= DateTime.Now && client.LastSend.Date < DateTime.Now.Date) //considerar enviar 
                        {
                            if (client.LastSend == new DateTime())
                            {
                                await sendMessageAsync(client.TelegramChatId, client.TextMessage);
                            }
                            else if (client.LastSend.AddMinutes(-5) < DateTime.Now)
                            {
                                await sendMessageAsync(client.TelegramChatId, client.TextMessage);
                            }
                            client.LastSend = DateTime.Now;
                            _repo.Update(client);
                            _repo.SaveChanges();
                        }
                    }

                }
            }
            catch (Exception e)
            {
                // Functions.LogException(e);
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
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())  // Go query google
                using (Stream responseStream = response.GetResponseStream())               // Carrega a resposta
                using (StreamReader streamReader = new StreamReader(responseStream))       // Carrega o stream reader para ler a resposta
                {
                    siteContent = streamReader.ReadToEnd(); // Lê a resposta
                }
                var document = new HtmlDocument();
                document.LoadHtml(siteContent);
                var nodes = document.DocumentNode.SelectNodes("//*[@id='river-level-table']/tbody/tr/td");
                riverLevelHour = nodes[0].InnerText.Trim();
                riverLevel = nodes[1].InnerText.Trim();
            }
            catch (Exception e)
            {
                // Functions.LogException(e);
            }

        }
        public enum clientStatus
        {
            newCliente = 0,
            waitingForTextMessage = 1,
            waitingForTime = 2,
            complete = 3
        }
    }
}