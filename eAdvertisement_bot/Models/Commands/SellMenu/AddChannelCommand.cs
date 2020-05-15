﻿using eAdvertisement_bot.DAO;
using eAdvertisement_bot.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace eAdvertisement_bot.Models.Commands
{
    public class AddChannelCommand : Command
    {
        public override string Name => "/addChannel";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.Equals("/addChannel");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Назад в меню продажи", CallbackData = "/sellMenuP0" });
                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, null, false);
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Перешлите любой пост из канала в котором бот администратор", replyMarkup: keyboard);
                try
                {
                    await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                }
                catch { }
                dbContext.Users.First(u => u.User_Id == update.CallbackQuery.From.Id).User_State_Id = 1;
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                MainLogger.LogException(ex, "AddChannelCommand");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
