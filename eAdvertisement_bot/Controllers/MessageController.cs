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
        //AppDbContext dbContext = new AppDbContext();    // Initialization of dbContext that is used in exapmle below


        //GET: Method that hasn't any action because bot work throw post
        [HttpGet]
        public string Get()
        {
            /*
             * Small example how to get access to related tables, and how to add something.
             * More examples and with LINQ too is here https://metanit.com/sharp/entityframeworkcore/1.1.php
             * 
            List<Channel_Category> channelCategories = dbContext.Channel_Categories.ToList();
            List<Category> categories = dbContext.Categories.ToList();
            List<Channel> channels = dbContext.Channels.ToList();
            List<Advertisement> advs = dbContext.Advertisements.ToList();
            dbContext.Categories.Add(new Category { Name = "testCategoryThatAddedFromApp" });
            dbContext.SaveChanges();


            * If you would find a problem with part of working with dbContext, try to solve it with FluentAPI part in AppDbContext
            */

            return "GET method is unavailable";
        }


        
        //POST: Main post method in that updates from telegram become 
        [HttpPost]
        [Route("/df443335")]
        public async Task<OkResult> Post([FromBody]Update update)
        {
            if (update == null) { return Ok(); }    // Message can has no updates, but smth else
            else
            {
                var commands = Bot.Commands;
                var message = update.Message;
                var botClient = await Bot.GetBotClientAsync();

                
                foreach(var command in commands)
                {
                    if(command.Contains(message))
                    {
                        await command.Execute(message, botClient);
                        break;
                    }
                }
                return Ok();
            }
        }
    }
}
