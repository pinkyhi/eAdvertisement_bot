﻿using eAdvertisement_bot.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace eAdvertisement_bot.Models.Commands
{
    public class ChangeChannelCpmCommandText : Command
    {
        public override string Name => "ChangeChannelCpmCommandText";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                return false;
            }
            else
            {
                try
                {
                    return update.Message.Text.StartsWith("cpm: ");    // If it command is in text of update method

                }
                catch
                {
                    return false;
                }
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.Message.From.Id));
                if (user.User_State_Id > -1)
                {
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, "You aren't in the channel info menu");
                }
                else
                {
                    int amount = Convert.ToInt32(update.Message.Text.Substring(5));
                    dbContext.Channels.Find(user.User_State_Id).Cpm = amount;
                    dbContext.SaveChanges();
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Cpm is changed succesfully :)\nYou can write next commands, or press button bellow to see an updated menu", replyMarkup: new InlineKeyboardMarkup(new[] { new[] { new InlineKeyboardButton { Text = "Show updated menu", CallbackData = "/showChannelForSellerN" + user.User_State_Id }, } })); 
                }
            }
            catch(Exception ex)
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