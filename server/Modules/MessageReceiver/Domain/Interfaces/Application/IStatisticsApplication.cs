using System.Threading.Tasks;
using Telegram.WebAPI.Domain.DTO;

namespace Telegram.WebAPI.Application.Interfaces;

public interface IStatisticsApplication
{
    Task<StatisticsDto> GetStatistics();
}

