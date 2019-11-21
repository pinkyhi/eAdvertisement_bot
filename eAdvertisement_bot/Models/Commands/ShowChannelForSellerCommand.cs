using eAdvertisement_bot.DAO;
using eAdvertisement_bot.Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace eAdvertisement_bot.Models.Commands
{
    public class ShowChannelForSellerCommand : Command
    {
        public override string Name => "/showChannelForSeller";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("/showChannelForSellerN");
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            long channelId = Convert.ToInt64(update.CallbackQuery.Data.Substring(22));
            try
            {
                Channel channel = dbContext.Channels.Find(channelId);
                dbContext.Users.Find(channel.User_Id).Object_Id = channel.Channel_Id;
                dbContext.Users.Find(channel.User_Id).User_State_Id = 2;
                dbContext.SaveChanges();
                string info = "[" + channel.Name + "](" + channel.Link + ")" +
                    "\nSubscribers: " + channel.Subscribers +
                    "\nCoverage: " + channel.Coverage +
                    "\nERR: " + Math.Round(Convert.ToDouble(channel.Coverage) / Convert.ToDouble(channel.Subscribers), 2) +
                    "\nPrice: " + Convert.ToInt32(channel.Coverage / 1000 * channel.Cpm) +
                    "\nCpm: " + channel.Cpm;
                if(channel.Description!=null && !channel.Description.Equals(""))
                {
                    info += "\n*Description*\n" + channel.Description;
                }

                List<Place> places = dbContext.Places.Where(p => p.Channel_Id == channelId).OrderBy(p=>p.Time).ToList();
                InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[places.Count+2][];

                keyboard[0] = new[]
                {
                    new InlineKeyboardButton { Text = "Change desc", CallbackData = "/howToChangeDescription"},
                    new InlineKeyboardButton { Text = "Change cpm", CallbackData = "/howToChangeCpm"},
                    new InlineKeyboardButton { Text = "Add adv place", CallbackData = "/howToAddAdvPlace"},
                };
                int indexToPaste = 1;
                while (indexToPaste < places.Count()+1)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "Delete place "+places[indexToPaste-1].Time, CallbackData = "/deletePlaceN" + places[indexToPaste-1].Place_Id }, };
                    indexToPaste++;
                }
                keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "Back", CallbackData = "/sellMenuP0" }, };

                try
                {
                    await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                }
                catch { }
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, info, replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }
            catch(Exception ex) { await botClient.SendTextMessageAsync(update.CallbackQuery.From.Id, ex.Message); }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
