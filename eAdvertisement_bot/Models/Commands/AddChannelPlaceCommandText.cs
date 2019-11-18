using eAdvertisement_bot.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace eAdvertisement_bot.Models.Commands
{
    public class AddChannelPlaceCommandText : Command
    {
        public override string Name => "AddChannelPlaceCommandText";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                return false;
            }
            else
            {
                try
                {
                    return update.Message.Text.ToLower().StartsWith("place:");    // If it command is in text of update method

                }
                catch
                {
                    return false;
                }
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            string timeStr = update.Message.Text.Substring(6).Trim();
            if (timeStr.Length > 5)
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Incorrect format of time. Try again.");
                return;
            }
            else
            {
                AppDbContext dbContext = new AppDbContext();
                try
                {
                    DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.Message.From.Id));
                    if (dbContext.Places.Count(p => p.Channel_Id == user.User_State_Id) < 8)
                    {
                        TimeSpan ts = TimeSpan.Parse(timeStr);
                        dbContext.Places.Add(new DbEntities.Place { Channel_Id = user.User_State_Id, Time = ts });
                        dbContext.SaveChanges();
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Place is added succesfully :)\nYou can write next commands, or press button bellow to see an updated menu", replyMarkup: new InlineKeyboardMarkup(new[] { new[] { new InlineKeyboardButton { Text = "Show updated menu", CallbackData = "/showChannelForSellerN" + user.User_State_Id }, } }));
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Your count has to be less than 8");
                        return;
                    }
                    
                }
                catch(Exception ex)
                {
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, ex.Message);
                }
                finally
                {
                    dbContext.Dispose();
                }
            }
        }
    }
}
