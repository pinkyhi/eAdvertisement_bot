using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eAdvertisement_bot.DAO;
using eAdvertisement_bot.Logger;
using eAdvertisement_bot.Models.DbEntities;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace eAdvertisement_bot.Controllers
{
    
    [Route("/df443335")]
    [ApiController]
    public class MessageController : Controller
    {

        //GET: Method that hasn't any action because bot work throw post
        [HttpGet]
        public string Get()
        {
            return "GET method is unavailable";
        }




        //POST: Main post method in that updates from telegram become 
        [HttpPost]
        [Route("/df443335")]
        public async Task<StatusCodeResult> Post([FromBody]Update update)
        {
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery || update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var botClient = await Bot.GetBotClientAsync();    // Singleton
                try
                {
                    var commands = Bot.Commands;
                    if (update == null) { return Ok(); }    // Message can has no updates, but smth else ?
                    else
                    {
                        foreach (var command in commands)
                        {
                            if (command.Contains(update))
                            {
                                await command.Execute(update, botClient);

                                break;
                            }
                        }

                    }
                }
                catch (Exception ex) {
                    MainLogger.LogException(ex, "Controller");
                    return Ok();
                }
            }
            return Ok();
        }
    }
}
