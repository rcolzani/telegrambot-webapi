using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.WebAPI.Application.Services;
using Telegram.WebAPI.Domain.DTO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Telegram.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private StatisticsApplication _statisticsApp;
        private readonly ILogger<TelegramBotController> _logger;
        public StatisticsController(ILogger<TelegramBotController> logger, StatisticsApplication statisticsApp)
        {
            _statisticsApp = statisticsApp;
            _logger = logger;
        }

        // GET api/<ValuesController>
        [HttpGet]
        public async Task<StatisticsDto> Get()
        {
            try
            {
                var statistics = await _statisticsApp.GetStatistics();

                var quantity = new StatisticsDto(statistics.ActiveUsersQuantity,
                    statistics.UsersQuantity,
                    statistics.MessagesReceivedQuantity,
                    statistics.MessagesSentQuantity);

                return quantity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro:");
                return new StatisticsDto(0,0,0,0);
            }
        }
    }
}
