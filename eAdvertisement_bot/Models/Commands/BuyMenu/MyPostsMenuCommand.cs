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
    public class MyPostsMenuCommand : Command
    {
        public override string Name => "/myPostsMenu";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("/myPostsMenu");
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id)).User_State_Id = 0;
                dbContext.SaveChanges();
                List<DbEntities.Publication> posts = dbContext.Publications.Where(p => p.User_Id == update.CallbackQuery.From.Id).ToList();
                InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[posts.Count + 2][];
                keyboard[0] = new[]
                {
                    new InlineKeyboardButton{Text = "Add new post", CallbackData = "/addPost"}
                };

                int indexToPaste = 1;
                while (indexToPaste < posts.Count + 1)
                {
                    keyboard[indexToPaste] = new[]
                    {
                        new InlineKeyboardButton{Text=posts[indexToPaste-1].Name, CallbackData="/showPostN"+posts[indexToPaste-1].Publication_Id}
                    };
                    indexToPaste++;
                }
                keyboard[indexToPaste] = new[]
                {
                    new InlineKeyboardButton{Text="Back", CallbackData = "/buyMenu"}
                };
                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, null, false);
                try
                {
                    await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                }
                catch { }
                finally
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Here you can create advertisement posts and look them up", replyMarkup: new InlineKeyboardMarkup(keyboard));
                }

            }
            catch(Exception ex)
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
