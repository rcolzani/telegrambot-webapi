using System.Collections.Generic;

namespace Functions
{
    public static class Settings
    {
        public static string ConnectionString { get; set; }
        public static string TelegramToken { get; set; }
        public static bool TelegramBotActivated { get; set; }
        public static string ControllerActionsPassword { get; set; }

        public static bool TelegramBotReceivingMessage { get; private set; }

        public static void ReceivingMessageStart()
        {
            TelegramBotReceivingMessage = true;
        }
        public static void ReceivingMessageStop()
        {
            TelegramBotReceivingMessage = false;
        }

        public static bool TelegramBotSendingMessages { get; private set; }
        public static void SendingMessageStart()
        {
            TelegramBotSendingMessages = true;
        }
        public static void SendingMessageStop()
        {
            TelegramBotSendingMessages = false;
        }
    }
}