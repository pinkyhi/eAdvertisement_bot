using System;
using eAdvertisement_bot.DAO;
using eAdvertisement_bot.Models.DbEntities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Text.Json;

namespace eAdvertisement_bot.Models.Commands
{
    public class AcceptManufactureBuyCommand : Command
    {
        public override string Name => "/acceptManufactureBuyNDTP";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("ambN");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            long channelId = Convert.ToInt64(update.CallbackQuery.Data.Substring(4, update.CallbackQuery.Data.IndexOf('D') - 4));
            string dateStr = update.CallbackQuery.Data.Substring(update.CallbackQuery.Data.IndexOf('D') + 1, update.CallbackQuery.Data.IndexOf('T') - (update.CallbackQuery.Data.IndexOf('D') + 1));
            string tags = update.CallbackQuery.Data.Substring(update.CallbackQuery.Data.IndexOf('T') + 1, update.CallbackQuery.Data.IndexOf('P') - (update.CallbackQuery.Data.IndexOf('T') + 1));
            int postId = Convert.ToInt32(update.CallbackQuery.Data.Substring(update.CallbackQuery.Data.IndexOf('P') + 1));
            DateTime dateTime = DateTime.Parse(dateStr);
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                Channel channel = dbContext.Channels.Find(channelId);
                Publication post = dbContext.Publications.Find(postId);
                List<Button> buttons = dbContext.Buttons.Where(b => b.Publication_Id == post.Publication_Id).ToList();

                List<Media> media = dbContext.Medias.Where(m => m.Publication_Id == post.Publication_Id).ToList();
                InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[1][];

                string json = JsonSerializer.Serialize(post);
                

                keyboard[0] = new[] { new InlineKeyboardButton { Text = "Назад в меню ручной покупки", CallbackData = "/manualPurchaseMenuP"+tags } };

                if(user.Balance>= channel.Price)
                {
                    try
                    {
                        // TODO: Check time

                        DateTime nowIs = DateTime.Now;
                        DateTime tDT = DateTime.Parse(dateStr);


                        if (new DateTime(tDT.Year, tDT.Month, tDT.Day, tDT.Hour, tDT.Minute, tDT.Second) < DateTime.Now)
                        {
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Это место уже занято :С, попробуйте выбрать другое");
                            update.CallbackQuery.Data = "/showPlacesCalendarForBuyerN" + channelId + "T" + tags;
                            ShowPlacesCalendarForBuyerCommand spcfbc = new ShowPlacesCalendarForBuyerCommand();
                            await spcfbc.Execute(update, botClient);
                            return;

                        }

                        List<Advertisement> nearestAds = dbContext.Advertisements.Where(a => a.Is_Opened && (a.Advertisement_Status_Id == 2 || a.Advertisement_Status_Id == 4 || a.Advertisement_Status_Id == 9)).Where(a => a.Channel_Id == channelId && a.Date_Time <= new DateTime(tDT.Year, tDT.Month, tDT.Day, tDT.Hour, tDT.Minute, tDT.Second)).ToList();
                        Advertisement nearestAd = nearestAds.FirstOrDefault(a => a.Date_Time.Equals(nearestAds.Max(a => a.Date_Time)));

                        List<Advertisement> nearestTopAds = dbContext.Advertisements.Where(a => a.Is_Opened && ( a.Advertisement_Status_Id == 2 || a.Advertisement_Status_Id == 4 || a.Advertisement_Status_Id == 9)).Where(a => a.Channel_Id == channelId && a.Date_Time > new DateTime(tDT.Year, tDT.Month, tDT.Day, tDT.Hour, tDT.Minute, tDT.Second)).ToList();
                        Advertisement nearestTopAd = nearestTopAds.FirstOrDefault(a => a.Date_Time.Equals(nearestTopAds.Min(a => a.Date_Time)));

                        if (nearestAd != null)
                        {
                            if ((new DateTime(tDT.Year, tDT.Month, tDT.Day, tDT.Hour, tDT.Minute, tDT.Second)).Subtract(new TimeSpan(nearestAd.Top, 0, 0)) < nearestAd.Date_Time)
                            {
                                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Это место уже занято :С, попробуйте выбрать другое");
                                update.CallbackQuery.Data = "/showPlacesCalendarForBuyerN" + channelId + "T" + tags;
                                ShowPlacesCalendarForBuyerCommand spcfbc = new ShowPlacesCalendarForBuyerCommand();
                                await spcfbc.Execute(update, botClient);
                                return;
                            }
                        }
                        if (nearestTopAd != null)
                        {
                            if ((new DateTime(nowIs.Year, nowIs.Month, nowIs.Day, tDT.Hour, tDT.Minute, tDT.Second)).Add(new TimeSpan(1, 0, 0)) > nearestTopAd.Date_Time)
                            {
                                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Это место уже занято :С, попробуйте выбрать другое");
                                update.CallbackQuery.Data = "/showPlacesCalendarForBuyerN" + channelId + "T" + tags;
                                ShowPlacesCalendarForBuyerCommand spcfbc = new ShowPlacesCalendarForBuyerCommand();
                                await spcfbc.Execute(update, botClient);
                                return;
                            }
                        }

                        Advertisement newAd = new Advertisement { Is_Opened = true, Advertisement_Status_Id = 1, Alive = 24, Top = 1, Channel_Id = channelId, Publication_Snapshot = json, Date_Time = dateTime, User_Id = update.CallbackQuery.From.Id, Price = channel.Price };
                        dbContext.Advertisements.Add(newAd);
                        user.Balance -= channel.Price;
                        dbContext.SaveChanges();
                        try
                        {
                            await botClient.SendTextMessageAsync(channel.User_Id, "У вас новый заказ на рекламу:)", disableNotification: true, replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Заказы", CallbackData = "/ordersMenuP0" }));
                        }
                        catch { }
                        await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Пост отправлен", replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, disableWebPagePreview: true);
                    }
                    catch
                    {
                        await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Это место уже занято", replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, disableWebPagePreview: true);
                    }

                }
                else
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Извините, но у вас недостаточный баланс", replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, disableWebPagePreview: true);
                    
                }



                try
                {
                    await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                }
                catch (Exception ex)
                {
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, ex.StackTrace + "\n" + ex.Message +"\n");
                }
            }
            catch (Exception ex) { await botClient.SendTextMessageAsync(update.CallbackQuery.From.Id, ex.StackTrace + "\n" + ex.Message +"\n"); }
            finally
            {
                dbContext.Dispose();
            }
        }

    }
}
