﻿using eAdvertisement_bot.DAO;
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
    public class On9XXStateEvent : Command
    {
        public override string Name =>"/on9XXStateEvent";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                return false;
            }
            else
            {
                AppDbContext dbContext = new AppDbContext();
                try
                {
                    DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.Message.From.Id));
                    return Convert.ToString(user.User_State_Id).StartsWith("9") && (Convert.ToString(user.User_State_Id)).Length>2;
                }
                catch
                {
                    return false;
                }
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.Message.From.Id));
                long channelId = user.Object_Id;
                long tag = user.User_State_Id;
                //22-06-01 13:31
                if (tag == 901 && dbContext.Channels.Find(channelId)!=null)
                {
                    DateTime dateTime = DateTime.Parse(update.Message.Text);

                    List<Advertisement> ads = dbContext.Advertisements.Where(a => a.Channel_Id == channelId && a.Date_Time < dateTime).ToList();
                    Advertisement nearestAd = ads.Where(a => a.Date_Time == ads.Max(a => a.Date_Time)).FirstOrDefault();

                    if (dateTime.Subtract(nearestAd.Date_Time).Hours >= nearestAd.Top)
                    {
                        dbContext.Advertisements.Add(new Advertisement { Channel_Id = channelId, Date_Time = dateTime, Advertisement_Status_Id = 9, Price = 0 });
                        dbContext.SaveChanges();
                        InlineKeyboardButton[][] keyboard = new[] { new[] { new InlineKeyboardButton { Text = "Back into sold ads menu", CallbackData = "/soldPostsMenu" }, } };
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Ad is added succesfully", replyMarkup: new InlineKeyboardMarkup(keyboard));
                    }
                    else if(dateTime.Subtract(nearestAd.Date_Time).Hours < nearestAd.Top)
                    {
                        InlineKeyboardButton[][] keyboard = new[] { new[] { new InlineKeyboardButton { Text = "Back into sold ads menu", CallbackData = "/soldPostsMenu" }, } };
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Ad isn't added because u have already an ad sold thats time+top is bigger than time that you defined now. Try again or go back.", replyMarkup: new InlineKeyboardMarkup(keyboard));
                        return;
                    }
                    else if (dateTime<DateTime.Now)
                    {
                        InlineKeyboardButton[][] keyboard = new[] { new[] { new InlineKeyboardButton { Text = "Back into sold ads menu", CallbackData = "/soldPostsMenu" }, } };
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Ad isn't added because date is less than now. Try again or go back.", replyMarkup: new InlineKeyboardMarkup(keyboard));
                        return;
                    }
                }
                user.User_State_Id = 0;
                dbContext.SaveChanges();
            }
            catch (Exception ex)
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