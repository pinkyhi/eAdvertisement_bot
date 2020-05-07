using eAdvertisement_bot.DAO;
using eAdvertisement_bot.Logger;
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
        public override string Name => "/showPlacesForBuyerNDT";

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
                DateTime nowIs = DateTime.Now;

                InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[places.Count + 2][];
                DateTime tDT = DateTime.Parse(dateStr);
                int i = 0;

                if (channel.Places != null)
                {
                    foreach (Place p in places)
                    {
                        if (new DateTime(tDT.Year, tDT.Month, tDT.Day, p.Time.Hours, p.Time.Minutes, p.Time.Seconds) < DateTime.Now)
                        {
                            keyboard[i] = new[] { new InlineKeyboardButton { Text = "X" + Convert.ToString(places[i].Time) + "X", CallbackData = "/miq" } };
                            i++;
                            continue;
                        }

                        List<Advertisement> nearestAds = dbContext.Advertisements.Where(a => a.Is_Opened && ( a.Advertisement_Status_Id == 2 || a.Advertisement_Status_Id == 4 || a.Advertisement_Status_Id == 9)).Where(a => a.Channel_Id == channelId && a.Date_Time <= new DateTime(tDT.Year, tDT.Month, tDT.Day, p.Time.Hours, p.Time.Minutes, p.Time.Seconds)).ToList();
                        Advertisement nearestAd = nearestAds.FirstOrDefault(a => a.Date_Time.Equals(nearestAds.Max(a => a.Date_Time)));

                        List<Advertisement> nearestTopAds = dbContext.Advertisements.Where(a => a.Is_Opened && ( a.Advertisement_Status_Id == 2 || a.Advertisement_Status_Id == 4 || a.Advertisement_Status_Id == 9)).Where(a => a.Channel_Id == channelId && a.Date_Time > new DateTime(tDT.Year, tDT.Month, tDT.Day, p.Time.Hours, p.Time.Minutes, p.Time.Seconds)).ToList();
                        Advertisement nearestTopAd = nearestTopAds.FirstOrDefault(a => a.Date_Time.Equals(nearestTopAds.Min(a => a.Date_Time)));

                        if (nearestAd != null)
                        {
                            if ((new DateTime(tDT.Year, tDT.Month, tDT.Day, p.Time.Hours, p.Time.Minutes, p.Time.Seconds)).Subtract(new TimeSpan(nearestAd.Top, 0, 0)) < nearestAd.Date_Time)
                            {
                                keyboard[i] = new[] { new InlineKeyboardButton { Text = "X" + Convert.ToString(places[i].Time) + "X", CallbackData = "/miq" } };
                                i++;
                                continue;
                            }
                        }
                        if (nearestTopAd != null)
                        {
                            if ((new DateTime(nowIs.Year, nowIs.Month, nowIs.Day, p.Time.Hours, p.Time.Minutes, p.Time.Seconds)).Add(new TimeSpan(1, 0, 0)) > nearestTopAd.Date_Time)
                            {
                                keyboard[i] = new[] { new InlineKeyboardButton { Text = "X" + Convert.ToString(places[i].Time) + "X", CallbackData = "/miq" } };
                                i++;
                                continue;
                            }
                        }
                        keyboard[i] = new[] { new InlineKeyboardButton { Text = Convert.ToString(places[i].Time), CallbackData = "cpfaN" + channelId + "D" + Convert.ToString(tDT.AddHours(places[i].Time.Hours).AddMinutes(places[i].Time.Minutes)) + "T" + tags } };
                        i++;
                    }

                }

                keyboard[keyboard.Length - 2] = new[] { new InlineKeyboardButton { Text = "Назад", CallbackData = "/showPlacesCalendarForBuyerN" + channelId + "T" + tags } };
                keyboard[keyboard.Length - 1] = new[] { new InlineKeyboardButton { Text = "Отмена", CallbackData = "/manualPurchaseMenuP" + tags } };

                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Выберите время", replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, disableWebPagePreview: true);

                try
                {
                    await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                }
                catch
                {
                }
            }
            catch (Exception ex)
            {
                MainLogger.LogException(ex, "ShowPlacesForBuyerCommand");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
