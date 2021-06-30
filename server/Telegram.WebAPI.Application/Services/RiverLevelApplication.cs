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
using Telegram.WebAPI.Shared.Extensions;

namespace Telegram.WebAPI.Application.Services
{
    public class RiverLevelApplication
    {
        private string lastRiverLevel;
        public TelegramBotClient bot;
        private TelegramBotApplication _telegramBotApp;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<ChatHub, IChatClient> _chatHub;
        public RiverLevelApplication(IUnitOfWork unitOfWork, IHubContext<ChatHub, IChatClient> chatHub, TelegramBotApplication telegramBotApplication)
        {
            _chatHub = chatHub;
            _unitOfWork = unitOfWork;
            _telegramBotApp = telegramBotApplication;
        }
        public async Task<bool> SendRiverLevel()
        {
            try
            {
                string riverLevelHour = "", riverLevel = "";
                RiverLevelAlertaBlu(out riverLevel, out riverLevelHour);

                if (string.IsNullOrEmpty(riverLevel) || string.IsNullOrEmpty(riverLevelHour))
                {
                    //As vezes acontece de o site da prefeitura estar com algum dado vazio e não pode ser enviado nesses casos
                    return true;
                }
                if (lastRiverLevel == riverLevel + riverLevelHour)
                {
                    //Se o nível e o horário da medição continuam o mesmo, não deve enviar o lembrete
                    return true;                   
                }

                lastRiverLevel = riverLevel + riverLevelHour;

                foreach (var user in _unitOfWork.TelegramUsers.GetAllUsersWithSendRiverActivate())
                {
                    await _telegramBotApp.sendMessageAsync(user.TelegramChatId, $"O nível do rio está {riverLevel} às {riverLevelHour}");
                }
                return true;
            }
            catch (Exception e)
            {
                e.LogExceptionToConsole();
                return false;
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
                e.LogExceptionToConsole();
            }

        }
    }
}
