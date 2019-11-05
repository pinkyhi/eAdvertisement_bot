using eAdvertisement_bot.DAO;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace eAdvertisement_bot.Models.Commands
{
    public class StartCommand : Command
    {
        public override string Name => @"/start";

        
        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                return false;
            }
            else
            {
                return update.Message.Text.Equals(this.Name);    // If it command is in text of update method
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            var message = update.Message;

            long userId = message.From.Id;
            AppDbContext dbContext = new AppDbContext();

            DbEntities.User userEntity = dbContext.Users.Find(userId);

            try
            {
                if (userEntity == null)
                {
                    dbContext.Users.Add(new DbEntities.User { User_Id = message.From.Id, Nickname = message.From.Username, FirstName = message.From.FirstName, LastName = message.From.LastName, Language = message.From.LanguageCode, Stopped = false });
                    dbContext.SaveChanges();
                    dbContext.Dispose();
                    InlineKeyboardMarkup keyboard = entryLaunchedBotKeyboard;
                    await botClient.SendTextMessageAsync(userId, "Hi, " + message.From.FirstName + ". Initialization is succesfull :)", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: keyboard);
                }
                else
                {
                    InlineKeyboardMarkup keyboard;
                    if (userEntity.Stopped)
                    {
                        keyboard = entryStoppedBotKeyboard;
                    }
                    else
                    {
                        keyboard = entryLaunchedBotKeyboard;
                    }
                    await botClient.SendTextMessageAsync(userId, "You are already initialized.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: keyboard);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                await botClient.SendTextMessageAsync(userId, "Sorry, now are some troubles with initialization.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }
                
            
        }
    }
}
