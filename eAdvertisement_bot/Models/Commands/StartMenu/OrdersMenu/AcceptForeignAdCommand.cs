using eAdvertisement_bot.DAO;
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
    public class AcceptForeignAdCommand : Command
    {
        public override string Name => "acceptForeignAdN";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("afaN");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            int adId = Convert.ToInt32(update.CallbackQuery.Data.Substring(update.CallbackQuery.Data.IndexOf('N') + 1));
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User userEntity = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                Advertisement ad = dbContext.Advertisements.Find(adId);

                DateTime nowIs = DateTime.Now;
                DateTime tDT = ad.Date_Time;


                if (new DateTime(tDT.Year, tDT.Month, tDT.Day, tDT.Hour, tDT.Minute, tDT.Second) < DateTime.Now)
                {
                    await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Время этой рекламы уже прошло.", true);

                    ad.Advertisement_Status_Id = 3;
                    dbContext.SaveChanges();

                    update.CallbackQuery.Data = "/ordersMenuP0";

                    OrdersMenuCommand omc = new OrdersMenuCommand();
                    await omc.Execute(update, botClient);
                    return;

                }

                List<Advertisement> nearestAds = dbContext.Advertisements.Where(a => a.Advertisement_Status_Id == 2 || a.Advertisement_Status_Id == 4 || a.Advertisement_Status_Id == 9).Where(a => a.Channel_Id == ad.Channel_Id && a.Date_Time <= new DateTime(tDT.Year, tDT.Month, tDT.Day, tDT.Hour, tDT.Minute, tDT.Second)).ToList();
                Advertisement nearestAd = nearestAds.FirstOrDefault(a => a.Date_Time.Equals(nearestAds.Max(a => a.Date_Time)));

                List<Advertisement> nearestTopAds = dbContext.Advertisements.Where(a => a.Advertisement_Status_Id == 2 || a.Advertisement_Status_Id == 4 || a.Advertisement_Status_Id == 9).Where(a => a.Channel_Id == ad.Channel_Id && a.Date_Time > new DateTime(tDT.Year, tDT.Month, tDT.Day, tDT.Hour, tDT.Minute, tDT.Second)).ToList();
                Advertisement nearestTopAd = nearestTopAds.FirstOrDefault(a => a.Date_Time.Equals(nearestTopAds.Min(a => a.Date_Time)));

                if (nearestAd != null)
                {
                    if ((new DateTime(tDT.Year, tDT.Month, tDT.Day, tDT.Hour, tDT.Minute, tDT.Second)).Subtract(new TimeSpan(nearestAd.Top, 0, 0)) < nearestAd.Date_Time)
                    {
                        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Это место уже занято", true);
                        ad.Advertisement_Status_Id = 3;
                        dbContext.SaveChanges();

                        update.CallbackQuery.Data = "/ordersMenuP0";

                        OrdersMenuCommand omc = new OrdersMenuCommand();
                        await omc.Execute(update, botClient);
                        return;
                    }
                }
                if (nearestTopAd != null)
                {
                    if ((new DateTime(nowIs.Year, nowIs.Month, nowIs.Day, tDT.Hour, tDT.Minute, tDT.Second)).Add(new TimeSpan(1, 0, 0)) > nearestTopAd.Date_Time)
                    {
                        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Это место уже занято", true);
                        ad.Advertisement_Status_Id = 3;
                        dbContext.SaveChanges();

                        update.CallbackQuery.Data = "/ordersMenuP0";

                        OrdersMenuCommand omc = new OrdersMenuCommand();
                        await omc.Execute(update, botClient);
                        return;
                    }
                }

                ad.Advertisement_Status_Id = 2;
                dbContext.SaveChanges();
                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Реклама принята!", true);
                update.CallbackQuery.Data = "/ordersMenuP0";

                OrdersMenuCommand omcc = new OrdersMenuCommand();
                await omcc.Execute(update, botClient);

            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, ex.Message);
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
