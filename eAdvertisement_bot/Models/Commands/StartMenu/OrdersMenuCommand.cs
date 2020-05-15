using eAdvertisement_bot.DAO;
using eAdvertisement_bot.Logger;
using eAdvertisement_bot.Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;


namespace eAdvertisement_bot.Models.Commands
{
    public class OrdersMenuCommand : Command
    {
        public override string Name => "/ordersMenuP";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("/ordersMenuP");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            int page = Convert.ToInt32(update.CallbackQuery.Data.Substring(update.CallbackQuery.Data.IndexOf('P')+1));
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User userEntity = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                List<Channel> channels = dbContext.Channels.Where(c => c.User_Id == userEntity.User_Id).ToList();
                List<long> channelIds = channels.Select(c => c.Channel_Id).ToList();
                List<Advertisement> ads = dbContext.Advertisements.Where(a => a.Is_Opened && (a.Date_Time>DateTime.Now && channelIds.Contains(a.Channel_Id) && a.Advertisement_Status_Id == 1)).OrderByDescending(a=>a.Price).OrderBy(a=>a.Date_Time).ToList();
                DateTime nowIs = DateTime.Now;

                for (int i = 0; i < ads.Count; i++)
                {
                    Advertisement adNow = ads[i];
                    DateTime tDT = adNow.Date_Time;


                    if (new DateTime(tDT.Year, tDT.Month, tDT.Day, tDT.Hour, tDT.Minute, tDT.Second) < DateTime.Now)
                    {
                        adNow.Advertisement_Status_Id = 3;
                        continue;

                    }

                    List<Advertisement> nearestAds = dbContext.Advertisements.Where(a => a.Is_Opened && ( a.Advertisement_Status_Id == 2 || a.Advertisement_Status_Id == 4 || a.Advertisement_Status_Id == 9)).Where(a => a.Channel_Id == adNow.Channel_Id && a.Date_Time <= new DateTime(tDT.Year, tDT.Month, tDT.Day, tDT.Hour, tDT.Minute, tDT.Second)).ToList();
                    Advertisement nearestAd = nearestAds.FirstOrDefault(a => a.Date_Time.Equals(nearestAds.Max(a => a.Date_Time)));

                    List<Advertisement> nearestTopAds = dbContext.Advertisements.Where(a => a.Is_Opened && (a.Advertisement_Status_Id == 2 || a.Advertisement_Status_Id == 4 || a.Advertisement_Status_Id == 9)).Where(a => a.Channel_Id == adNow.Channel_Id && a.Date_Time > new DateTime(tDT.Year, tDT.Month, tDT.Day, tDT.Hour, tDT.Minute, tDT.Second)).ToList();
                    Advertisement nearestTopAd = nearestTopAds.FirstOrDefault(a => a.Date_Time.Equals(nearestTopAds.Min(a => a.Date_Time)));

                    if (nearestAd != null)
                    {
                        if ((new DateTime(tDT.Year, tDT.Month, tDT.Day, tDT.Hour, tDT.Minute, tDT.Second)).Subtract(new TimeSpan(nearestAd.Top, 0, 0)) < nearestAd.Date_Time)
                        {
                            adNow.Advertisement_Status_Id = 3;
                            continue;
                        }
                    }
                    if (nearestTopAd != null)
                    {
                        if ((new DateTime(nowIs.Year, nowIs.Month, nowIs.Day, tDT.Hour, tDT.Minute, tDT.Second)).Add(new TimeSpan(1, 0, 0)) > nearestTopAd.Date_Time)
                        {
                            adNow.Advertisement_Status_Id = 3;
                            continue;
                        }
                    }
                }

                dbContext.SaveChanges();
                ads = ads.Where(a => a.Advertisement_Status_Id == 1).ToList();
                Advertisement ad = ads!=null && ads.Count!=0 && ads[page] != null ? ads[page] : null;
                if (ad != null)
                {
                    Publication post;
                    try
                    {
                        post = JsonSerializer.Deserialize<Publication>(ad.Publication_Snapshot);
                    }
                    catch (Exception ex)
                    {
                        post = null;
                        MainLogger.LogException(ex, $"OrdersMenuCommand_Serialization adId = {ad.Advertisement_Id}");
                    }

                    SendPost(post, update, botClient);
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id,"_____________Конец поста____________");

                    string text = "Всего заказов "+ads.Count()+"\n*Информация о заказе*\n" +
                    "Канал: " + "[" + ad.Channel.Name + "](" + ad.Channel.Link + ")" +
                    "\nЦена: " + Convert.ToInt32(Convert.ToDouble(ad.Price)*userEntity.Commission) +
                    "\nТоп: " + ad.Top +
                    "\nЛента: " + ad.Alive +
                    "\nВремя постинга: " + ad.Date_Time;
                    InlineKeyboardButton[][] controllKeyboard;
                    if (ads.Count > 1)
                    {
                        controllKeyboard = new InlineKeyboardButton[3][];
                        if (page == 0)
                        {
                            controllKeyboard[0] = new[]
                            {
                                new InlineKeyboardButton{CallbackData = "afaN"+ad.Advertisement_Id, Text = "Принять"},
                                new InlineKeyboardButton{CallbackData = "dfaN"+ad.Advertisement_Id, Text = "Отклонить"}
                            };
                            controllKeyboard[1] = new[] { new InlineKeyboardButton { CallbackData = "/ordersMenuP" + (page + 1), Text = "→→→ "+(page+2) } };
                            controllKeyboard[2] = new[] { new InlineKeyboardButton { CallbackData = "/backToStartMenu", Text = "Назад" } };
                        }
                        else if (page == ads.Count - 1)
                        {
                            controllKeyboard[0] = new[]
                            {
                                new InlineKeyboardButton{CallbackData = "afaN"+ad.Advertisement_Id, Text = "Принять"},
                                new InlineKeyboardButton{CallbackData = "dfaN"+ad.Advertisement_Id, Text = "Отклонить"}
                            };
                            controllKeyboard[1] = new[] { new InlineKeyboardButton { CallbackData = "/ordersMenuP" + (page - 1), Text = page+" ←←←" } };
                            controllKeyboard[2] = new[] { new InlineKeyboardButton { CallbackData = "/backToStartMenu", Text = "Назад" } };
                        }
                        else
                        {
                            controllKeyboard[0] = new[]
                            {
                                new InlineKeyboardButton{CallbackData = "afaN"+ad.Advertisement_Id, Text = "Принять"},
                                new InlineKeyboardButton{CallbackData = "dfaN"+ad.Advertisement_Id, Text = "Отклонить"}
                            };
                            controllKeyboard[1] = new[] { new InlineKeyboardButton { CallbackData = "/ordersMenuP" + (page - 1), Text = page+ " ←←←" }, new InlineKeyboardButton { CallbackData = "/ordersMenuP" + (page + 1), Text = "→→→ "+(page+2) } };
                            controllKeyboard[2] = new[] { new InlineKeyboardButton { CallbackData = "/backToStartMenu", Text = "Назад" } };
                        }
                    }
                    else
                    {
                        controllKeyboard = new InlineKeyboardButton[2][];
                        controllKeyboard[0] = new[]
                            {
                                new InlineKeyboardButton{CallbackData = "afaN"+ad.Advertisement_Id, Text = "Принять"},
                                new InlineKeyboardButton{CallbackData = "dfaN"+ad.Advertisement_Id, Text = "Отклонить"}
                            };
                        controllKeyboard[1] = new[] { new InlineKeyboardButton { CallbackData = "/backToStartMenu", Text = "Назад" } };
                    }


                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, disableWebPagePreview: true, replyMarkup: new InlineKeyboardMarkup(controllKeyboard));

                }
                else
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Пока у вас нет заказов.", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { CallbackData = "/backToStartMenu", Text = "Назад" }));
                }


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
                MainLogger.LogException(ex, "OrdersMenuCommand");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
