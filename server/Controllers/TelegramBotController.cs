using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Telegram.WebAPI.Data;
using Telegram.WebAPI.Domain.Interfaces;
using Telegram.WebAPI.Dtos;

namespace Telegram.WebAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class TelegramBotController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TelegramBotController> _logger;
        public TelegramBotController(IUnitOfWork unitOfWork, ILogger<TelegramBotController> logger)
        {
            _unitOfWork = unitOfWork;
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
                var clients = await _unitOfWork.TelegramUsers.GetAllClientesAsync();
                int resultado;

                if (clients.Count() == 0)
                {
                    _unitOfWork.TelegramUsers.Add(new Domain.Entities.TelegramUser { Name = "teste", TelegramChatId = 852455});
                    resultado = _unitOfWork.Complete();
                }

                var messages = await _unitOfWork.MessageHistorys.GetAllMessagesAsync(); 
                var quantity = new Hubs.Models.StatisticsMain(
                    clients.Length,
                    clients.Count(), messages.Where(m => m.MessageSent == false).Count(),
                    messages.Where(m => m.MessageSent).Count()
                     );
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