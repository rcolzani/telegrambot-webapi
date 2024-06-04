using System;

namespace Telegram.WebAPI.Hubs.Models;

public class XChatMessage
{
    public XChatMessage(string user, string message, DateTime dataHora)
    {
        this.User = user.Substring(0, 2); ;
        this.Message = message.Replace(user, "<usu�rio>");
        this.DataHora = dataHora;
    }
    public string User { get; set; }

    public string Message { get; set; }

    public DateTime DataHora { get; set; }
}
