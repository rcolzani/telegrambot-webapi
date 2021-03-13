using Microsoft.AspNetCore.Mvc;

namespace Telegram.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TelegramBotController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(Functions.Settings.TelegramBotActivated);
        }

        [HttpPost("{activate}")]
        public IActionResult Post(bool activate)
        {
            string responseText = "";
            if ((activate && Functions.Settings.TelegramBotActivated) || (activate == false && Functions.Settings.TelegramBotActivated == false))
            {
                responseText = "Already with this status";
            }
            Functions.Settings.TelegramBotActivated = activate;
            return Ok(Functions.Settings.TelegramBotActivated);
        }

    }
}