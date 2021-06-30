using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Telegram.WebAPI.Application.Services;
using Telegram.WebAPI.Data;
using Telegram.WebAPI.Domain.DTO;
using Telegram.WebAPI.Domain.Interfaces;

namespace Telegram.WebAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class TelegramBotController : ControllerBase
    {
        private StatisticsApplication _statisticsApp;
        private readonly ILogger<TelegramBotController> _logger;
        public TelegramBotController(IUnitOfWork unitOfWork, ILogger<TelegramBotController> logger, StatisticsApplication statisticsApp)
        {
            _statisticsApp = statisticsApp;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(Functions.Settings.TelegramBotActivated);
        }

        [HttpPost]
        public IActionResult Post(StatusChangeDto statusChange)
        {
            if (statusChange.passwd != Functions.Settings.ControllerActionsPassword)
            {
                return BadRequest("Senha incorreta ou não informada");
            }
            else if (statusChange.Activate == null)
            {
                return BadRequest("Status não informado");
            }

            Functions.Settings.TelegramBotActivated = (bool)statusChange.Activate;
            return Ok(Functions.Settings.TelegramBotActivated);
        }

        [HttpGet("quantity")]
        public async Task<IActionResult> GetQuantity(bool activate)
        {
            try
            {
                var statistics = await _statisticsApp.GetStatistics();

                var quantity = new Hubs.Models.StatisticsMain(statistics.UsersQuantity, 
                    statistics.ActiveUsersQuantity, 
                    statistics.MessagesReceivedQuantity, 
                    statistics.MessagesSentQuantity);

                return Ok(quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro:");
            }
            return Problem("Falhou");
        }

    }
}