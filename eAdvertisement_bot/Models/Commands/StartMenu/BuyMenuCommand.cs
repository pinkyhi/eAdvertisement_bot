﻿using eAdvertisement_bot.DAO;
using eAdvertisement_bot.Logger;
using eAdvertisement_bot.Models.DbEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace eAdvertisement_bot.Models.Commands
{
    public class BuyMenuCommand : Command
    {
        public override string Name => "/buyMenu";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.Equals("/buyMenu");
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User userEntity = dbContext.Users.Include("User_Status").FirstOrDefault(u=>u.User_Id == Convert.ToInt64(update.CallbackQuery.From.Id));
                userEntity.User_State_Id = 0;
                dbContext.SaveChanges();
                long hold = dbContext.Advertisements.Where(a => a.User_Id == userEntity.User_Id && (a.Advertisement_Status_Id == 1 || a.Advertisement_Status_Id == 2 || a.Advertisement_Status_Id == 4)).Sum(a => a.Price);
                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, null, false);  // ...,...,alert    AnswerCallbackQuery is required to send to avoid clock animation ob the button

                InlineKeyboardMarkup keyboard = buyMenuKeyboard;
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Баланс: " + userEntity.Balance + "\nХолд: " + hold + "\nСтатус аккаунта: *" + userEntity.User_Status.Name + ".*", replyMarkup: keyboard, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

                try
                {
                    await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                }
                catch
                {
                }

            }
            catch(Exception ex)
            {
                MainLogger.LogException(ex, "BuyMenuCommand");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
