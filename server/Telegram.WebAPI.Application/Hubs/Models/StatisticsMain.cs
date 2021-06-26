namespace Telegram.WebAPI.Hubs.Models
{
    public class StatisticsMain
    {
        public StatisticsMain(int clientsRegisteredTotal, int clientsActiveQuantity, int messagesReceivedQuantity, int messagesSentQuantity)
        {
            this.ClientsRegisteredTotal = clientsRegisteredTotal;
            this.ClientsActiveQuantity = clientsActiveQuantity;
            this.MessagesReceivedQuantity = messagesReceivedQuantity;
            this.MessagesSentQuantity = messagesSentQuantity;

        }
        public int ClientsRegisteredTotal { get; set; }
        public int ClientsActiveQuantity { get; set; }
        public int MessagesReceivedQuantity { get; set; }
        public int MessagesSentQuantity { get; set; }
    }
}