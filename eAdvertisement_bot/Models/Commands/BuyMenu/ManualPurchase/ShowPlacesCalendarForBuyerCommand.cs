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
    public class ShowPlacesCalendarForBuyerCommand : Command
    {
        public override string Name => "/showPlacesCalendarForBuyerNT";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("/showPlacesCalendarForBuyerN");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            long channelId = Convert.ToInt64(update.CallbackQuery.Data.Substring(28, update.CallbackQuery.Data.IndexOf('T') - 28));
            string tags = update.CallbackQuery.Data.Substring(update.CallbackQuery.Data.IndexOf('T') + 1);
            try
            {
                List<Place> places = dbContext.Places.Where(p => p.Channel_Id == channelId).ToList();
                Channel channel = dbContext.Channels.Find(channelId);


                InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[5][];

                DateTime nowIs = DateTime.Today;

                for(int i = 0; i < 3; i++)
                {
                    keyboard[i] = new InlineKeyboardButton[5];
                    for(int j = 0; j < 5; j++)
                    {
                        List<Place> placesShot = new List<Place>(places);
                        int freePlacesCount = placesShot.Count;
                        if (channel.Places != null)
                        {

                            foreach(Place p in placesShot)
                            {
                                if (new DateTime(nowIs.Year, nowIs.Month, nowIs.Day, p.Time.Hours, p.Time.Minutes, p.Time.Seconds) < DateTime.Now)
                                {
                                    freePlacesCount--;
                                    continue;
                                }
                                List<Advertisement> nearestAds = dbContext.Advertisements.Where(a => a.Advertisement_Status_Id == 2 || a.Advertisement_Status_Id == 4 || a.Advertisement_Status_Id == 9).Where(a => a.Channel_Id == channelId && a.Date_Time <= new DateTime(nowIs.Year,nowIs.Month,nowIs.Day,p.Time.Hours, p.Time.Minutes,p.Time.Seconds)).ToList();
                                Advertisement nearestAd = nearestAds.FirstOrDefault(a => a.Date_Time.Equals(nearestAds.Max(a => a.Date_Time)));

                                List<Advertisement> nearestTopAds = dbContext.Advertisements.Where(a => a.Advertisement_Status_Id == 2 || a.Advertisement_Status_Id == 4 || a.Advertisement_Status_Id == 9).Where(a => a.Channel_Id == channelId && a.Date_Time > new DateTime(nowIs.Year, nowIs.Month, nowIs.Day, p.Time.Hours, p.Time.Minutes, p.Time.Seconds)).ToList();
                                Advertisement nearestTopAd = nearestTopAds.FirstOrDefault(a => a.Date_Time.Equals(nearestTopAds.Min(a => a.Date_Time)));

                                if(nearestAd!= null)
                                {
                                    if( (new DateTime(nowIs.Year, nowIs.Month, nowIs.Day, p.Time.Hours, p.Time.Minutes, p.Time.Seconds)).Subtract(new TimeSpan(nearestAd.Top, 0, 0)) < nearestAd.Date_Time)
                                    {
                                        freePlacesCount--;
                                        continue;
                                    }
                                }
                                if(nearestTopAd!= null)
                                {
                                    if ((new DateTime(nowIs.Year, nowIs.Month, nowIs.Day, p.Time.Hours, p.Time.Minutes, p.Time.Seconds)).Add(new TimeSpan(1, 0, 0)) > nearestTopAd.Date_Time)
                                    {
                                        freePlacesCount--;
                                        continue;
                                    }
                                }
                            }
                        }
  

                        if(channel.Places==null || (freePlacesCount == 0))
                        {
                            keyboard[i][j] = new InlineKeyboardButton { Text = "X" + Convert.ToString(nowIs.Date).Substring(0, 5) + "X", CallbackData = "/showPlacesForBuyerN" + channelId + "D" + Convert.ToString(nowIs.Date).Substring(0, 10) + "T"+tags };
                        }
                        else
                        {
                            keyboard[i][j] = new InlineKeyboardButton { Text = Convert.ToString(nowIs.Date).Substring(0, 5), CallbackData = "/showPlacesForBuyerN" + channelId + "D" + Convert.ToString(nowIs.Date).Substring(0,10)+"T"+tags};

                        }
                        
                        nowIs=nowIs.AddDays(1);
                    }
                }

                keyboard[3] = new[]
                {
                    new InlineKeyboardButton { Text = "Назад", CallbackData = "/showChannelForBuyerN"+channelId+"T"+tags},
                };
                keyboard[4] = new[]
                {
                    new InlineKeyboardButton { Text = "Отмена", CallbackData = "/manualPurchaseMenuP" + tags },
                };

                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Здесь вы можете выбрать день для покупки рекламы", replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, disableWebPagePreview: true);

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
