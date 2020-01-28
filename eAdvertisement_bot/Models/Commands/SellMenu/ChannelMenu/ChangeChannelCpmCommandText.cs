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
    public class ChangeChannelCpmCommandText : Command
    {
        public override string Name => "ChangeChannelCpmCommandText";

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
                    return update.Message.Text.ToLower().StartsWith("cpm:");    // If it command is in text of update method

                }
                catch
                {
                    return false;
                }
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.Message.From.Id));
                if (user.User_State_Id != 2)
                {
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, "You aren't in the channel info menu");
                }
                else
                {
                    int amount = Convert.ToInt32(update.Message.Text.Substring(4).Trim());
                    if (amount < 10000)
                    {
                        DbEntities.Channel channel = dbContext.Channels.Find(user.Object_Id);
                        channel.Cpm = amount;
                        channel.Price = Convert.ToInt32(channel.Coverage * channel.Cpm / 1000);
                        dbContext.SaveChanges();
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Cpm is changed succesfully :)\nYou can write next commands, or press button bellow to see an updated menu", replyMarkup: new InlineKeyboardMarkup(new[] { new[] { new InlineKeyboardButton { Text = "Show updated menu", CallbackData = "/showChannelForSellerN" + user.Object_Id }, } }));
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "It's too much, bro:C Try again.\nOr go back, you can click on the buton below.", replyMarkup: new InlineKeyboardMarkup(new[] { new[] { new InlineKeyboardButton { Text = "Back", CallbackData = "/showChannelForSellerN" + user.Object_Id }, } }));

                    }

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
