using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Telegram.WebAPI.Data;
using Telegram.WebAPI.Dtos;

namespace Telegram.WebAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class TelegramBotController : ControllerBase
    {
        private readonly IRepository _repo;
        private readonly ILogger<TelegramBotController> _logger;
        public TelegramBotController(IRepository repo, ILogger<TelegramBotController> logger)
        {
            _repo = repo;
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
            Functions.Settings.TelegramBotActivated = statusChange.Activate;
            return Ok(Functions.Settings.TelegramBotActivated);
        }

        [HttpGet("getquantity")]
        public async Task<IActionResult> GetQuantity(bool activate)
        {
            try
            {
                var messages = await _repo.GetAllMessagesAsync();
                var clients = await _repo.GetAllClientesAsync();
                var quantity = new Hubs.Models.StatisticsMain(
                    clients.Length,
                    clients.Where(c => c.Activated).Count(), messages.Where(m => m.MessageSent == false).Count(),
                    messages.Where(m => m.MessageSent).Count()
                     );
                return Ok(quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro:");
            }
        }

    }
}