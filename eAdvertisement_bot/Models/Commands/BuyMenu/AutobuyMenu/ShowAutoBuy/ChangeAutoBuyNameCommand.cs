﻿using eAdvertisement_bot.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace eAdvertisement_bot.Models.Commands
{
    public class ChangeAutoBuyNameCommand : Command
    {
        public override string Name => "/changeAutobuyName";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("cabn");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                user.User_State_Id = 401;
                dbContext.SaveChanges();

                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Write the name of this autobuy, it will be available only for you.", true);
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Write the name of this autobuy\n*My first autobuy*", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

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
