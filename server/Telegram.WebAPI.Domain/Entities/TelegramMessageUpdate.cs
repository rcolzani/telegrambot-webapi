using System;

public class TelegramMessageUpdate
{
    public TelegramMessage Message { get; set; }
}

public class TelegramMessage
{
    public string Text { get; set; }
    public long chatId { get; set; }
    public string chatFirstName { get; set; }
    public string chatLastName { get; set; }
    public DateTime chatDateTime { get; set; }
}