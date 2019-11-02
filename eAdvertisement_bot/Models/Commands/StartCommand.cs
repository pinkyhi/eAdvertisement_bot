using eAdvertisement_bot.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace eAdvertisement_bot.Models.Commands
{
    public class StartCommand : Command
    {
        public override string Name => @"/start";

        
        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
            {
                return false;
            }
            else
            {
                return message.Text.Equals(this.Name);    // If it command is in text of update method TODO: now true with /start and /starts 
            }
        }

        public override async Task Execute(Message message, TelegramBotClient botClient)
        {
            var chatId = message.Chat.Id;
            try
            {
                AppDbContext dbContext = new AppDbContext();
                dbContext.Users.Add(new DbEntities.User { User_Id = message.From.Id, Nickname = message.From.Username, FirstName = message.From.FirstName, LastName = message.From.LastName, Language = message.From.LanguageCode });
                dbContext.SaveChanges();
                dbContext.Dispose();
                await botClient.SendTextMessageAsync(chatId, "Hi, "+message.From.FirstName+". Initialization is succesfull :)", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }
            catch (Exception ex)
            {
                if (ex.InnerException.ToString().Contains("Duplicate"))
                {
                    await botClient.SendTextMessageAsync(chatId, "You are already initialized.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId, "Sorry, now are some troubles with initialization.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                }
                
            }
        }
    }
}
