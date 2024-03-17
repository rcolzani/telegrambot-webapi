using HtmlAgilityPack;
using Microsoft.AspNetCore.SignalR;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.WebAPI.Domain.Interfaces;
using Telegram.WebAPI.Domain.Interfaces.Application;
using Telegram.WebAPI.Hubs;
using Telegram.WebAPI.Hubs.Clients;
using Telegram.WebAPI.Shared.Extensions;

namespace Telegram.WebAPI.Application.Services;

public class RiverLevelApplication : IRiverLevelApplication
{
    private string lastRiverLevel;
    public TelegramBotClient bot;
    private TelegramBotApplication _telegramBotApp;
    private readonly IUserRepository _userRepository;
    private readonly IHubContext<ChatHub, IChatClient> _chatHub;
    public RiverLevelApplication(IUserRepository userRepository, IHubContext<ChatHub, IChatClient> chatHub, TelegramBotApplication telegramBotApplication)
    {
        _chatHub = chatHub;
        _userRepository = userRepository;
        _telegramBotApp = telegramBotApplication;
    }
    public async Task SendRiverLevel()
    {
        try
        {
            string riverLevelHour = "", riverLevel = "";
            RiverLevelAlertaBlu(out riverLevel, out riverLevelHour);

            bool isValidRiverData = !string.IsNullOrEmpty(riverLevel) && !string.IsNullOrEmpty(riverLevelHour);
            if (!isValidRiverData)
                return;

            bool hasRiverLevelUpdated = lastRiverLevel != riverLevel + riverLevelHour;
            if (!hasRiverLevelUpdated)
                return;

            lastRiverLevel = riverLevel + riverLevelHour;

            foreach (var user in await _userRepository.GetAllUsersWithSendRiverActivateAsync())
            {
                await _telegramBotApp.sendMessageAsync(user.TelegramChatId, $"O nível do rio está {riverLevel} às {riverLevelHour}");
            }
        }
        catch (Exception e)
        {
            e.LogExceptionToConsole();
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
