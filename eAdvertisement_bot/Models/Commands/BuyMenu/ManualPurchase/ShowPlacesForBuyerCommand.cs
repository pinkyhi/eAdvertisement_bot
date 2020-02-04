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
    public class ShowPlacesForBuyerCommand : Command
    {
        public override string Name => "/showPlacesForBuyerNP";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("/showPlacesForBuyerN");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            long channelId = Convert.ToInt64(update.CallbackQuery.Data.Substring(20, update.CallbackQuery.Data.IndexOf('D')-20));
            string dateStr = update.CallbackQuery.Data.Substring(update.CallbackQuery.Data.IndexOf('D') + 1, update.CallbackQuery.Data.IndexOf('T') - (update.CallbackQuery.Data.IndexOf('D') + 1));
            string tags = update.CallbackQuery.Data.Substring(update.CallbackQuery.Data.IndexOf('T') + 1);
            DateTime dateTime = DateTime.Parse(dateStr);
            try
            {
                List<Place> places = dbContext.Places.Where(p => p.Channel_Id == channelId).OrderBy(p => p.Time).ToList();
                Channel channel = dbContext.Channels.Find(channelId);
                List<DateTime> occupiedAds = dbContext.Advertisements.Where(a=> a.Date_Time.Date.Equals(dateTime.Date) ).Where(a => a.Channel_Id == channelId && (a.Advertisement_Status_Id == 2 || a.Advertisement_Status_Id == 4) && a.Date_Time < DateTime.Now).Select(a=>a.Date_Time).ToList();
                List<TimeSpan> occupiedTimes = new List<TimeSpan>(occupiedAds.Count);

                for(int i = 0; i < occupiedAds.Count; i++)
                {
                    occupiedTimes.Add(occupiedAds[i].TimeOfDay);
                }
                
                InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[places.Count+1][];

                for(int i = 0; i < places.Count; i++)
                {
                    if (occupiedTimes.Contains(places[i].Time))
                    {
                        keyboard[i] = new[] { new InlineKeyboardButton { Text = "X"+Convert.ToString(places[i].Time)+"X", CallbackData = "/" } };
                    }
                    else
                    {
                        keyboard[i] = new[] { new InlineKeyboardButton { Text = Convert.ToString(places[i].Time), CallbackData = "/" } };
                    }
                }
                keyboard[keyboard.Length - 1] = new[] { new InlineKeyboardButton { Text = "Back", CallbackData = "/showPlacesCalendarForBuyerN" + channelId +"T"+tags} };


                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Choose time", replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, disableWebPagePreview: true);

                try
                {
                    await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                }
                catch (Exception ex)
                {
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, ex.Message);
                }
            }
            catch (Exception ex) { await botClient.SendTextMessageAsync(update.CallbackQuery.From.Id, ex.Message); }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
