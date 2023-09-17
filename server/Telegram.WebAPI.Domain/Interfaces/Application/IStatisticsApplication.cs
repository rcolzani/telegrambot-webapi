using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.WebAPI.Domain.DTO;

namespace Telegram.WebAPI.Application.Interfaces
{
    public interface IStatisticsApplication
    {
        Task<StatisticsDto> GetStatistics();
    }
}
