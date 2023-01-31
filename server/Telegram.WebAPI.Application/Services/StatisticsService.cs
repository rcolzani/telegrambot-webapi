using System.Linq;
using System.Threading.Tasks;
using Telegram.WebAPI.Application.Interfaces;
using Telegram.WebAPI.Domain.DTO;
using Telegram.WebAPI.Domain.Interfaces;

namespace Telegram.WebAPI.Application.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IUserRepository _userRepository;

        public StatisticsService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<StatisticsDto> GetStatistics()
        {
            var users = await _userRepository.GetAllUsersAsync();
            //var messages = await _userRepository.GetAllMessagesAsync();

            var userQuantity = users.Count();
            var activeClientsQuantity = users.Where(c => c.Reminders.Where(r => r.Status == Domain.Enums.ReminderStatus.Activated).Count() >= 1).Count();
            var messageReceivedQuantity = 0;// messages.Where(m => m.MessageSent == false).Count();
            var messageSentQuantity = 0; // messages.Where(m => m.MessageSent).Count();

            return new StatisticsDto(activeClientsQuantity, userQuantity, messageReceivedQuantity, messageSentQuantity);
        }
    }
}
