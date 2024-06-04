using System;

namespace Telegram.WebAPI.Application.Hubs.Models.Interfaces;

public abstract class MessageBase
{
    public string User { get; set; }

    public string Message { get; set; }

    public DateTime DataHora { get; set; }
}

