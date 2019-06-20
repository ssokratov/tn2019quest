using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewCellBot.Application;
using Telegram.Bot.Types;

namespace NewCellBot.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelegramController : ControllerBase
    {
        private readonly UpdateService _updateService;

        public TelegramController(UpdateService updateService)
        {
            _updateService = updateService;
        }

        [HttpPost]
        public async Task<ActionResult> Get(Update update)
        {
            await _updateService.HandleAsync(update);

            return new StatusCodeResult(200);
        }
    }
}
