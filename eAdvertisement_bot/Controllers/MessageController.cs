using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eAdvertisement_bot.DAO;
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
            var botClient = await Bot.GetBotClientAsync();    // May be this will be better to make static
            //return Ok();
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
            catch(Exception ex) { await botClient.SendTextMessageAsync(update.Message == null ? update.CallbackQuery.From.Id : update.Message.From.Id, "Error: " + ex.Message); return Ok(); }
            return Ok();
        }
    }
}
