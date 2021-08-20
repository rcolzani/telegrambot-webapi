using System.Linq;
using System.Threading.Tasks;
using Telegram.WebAPI.Domain.DTO;
using Telegram.WebAPI.Domain.Interfaces;

namespace Telegram.WebAPI.Application.Services
{
    public class StatisticsApplication
    {
        private readonly IUnitOfWork _unitOfWork;

        public StatisticsApplication(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<StatisticsDto> GetStatistics()
        {
            var users = await _unitOfWork.TelegramUsers.GetAllUsersAsync();
            var messages = await _unitOfWork.MessageHistorys.GetAllMessagesAsync();

            var userQuantity = users.Length;
            var activeClientsQuantity = users.Where(c => c.Reminders?.Where(r => r.Status == Domain.Enums.ReminderStatus.Activated).Count() >= 1).Count();
            var messageReceivedQuantity = messages.Where(m => m.MessageSent == false).Count();
            var messageSentQuantity = messages.Where(m => m.MessageSent).Count();

            return new StatisticsDto(activeClientsQuantity, userQuantity, messageReceivedQuantity, messageSentQuantity);
        }
    }
}
