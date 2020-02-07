﻿using eAdvertisement_bot.DAO;
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
    public class SoldPostsMenuCommand : Command
    {
        public override string Name => "/soldPostsMenu";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.Equals("/soldPostsMenu");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                InlineKeyboardButton[][] keyboard = new[] {
                    new[] { new InlineKeyboardButton { Text = "Add my own solding", CallbackData = "osmP0" }},
                    new[] { new InlineKeyboardButton { Text = "Back", CallbackData = "/backToStartMenu" } }, 
                   
                };

                List<Advertisement_Status> advertisement_Statuses = dbContext.Advertisement_Statuses.ToList();
                List<Channel> channels = dbContext.Channels.ToList();
                List<Advertisement> ads = dbContext.Advertisements.Where(a => a.Channel.User_Id == update.CallbackQuery.From.Id).Where(a => a.Advertisement_Status_Id == 4 || a.Advertisement_Status_Id == 2 || a.Advertisement_Status_Id == 1).ToList();

                string text = "*Here a posts that you have sold*\n\n";
                
                for(int i = 0; i < ads.Count; i++)
                {
                    text += "[" + ads[i].Channel.Name + "]" + "(" + ads[i].Channel.Link + ")" +
                        "\nPrice: " + ads[i].Channel.Price +
                        "\nPosted at: " + ads[i].Date_Time +
                        "\nStatus: " + ads[i].Advertisement_Status.Name+"\n";
                }

                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, text, replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

                try
                {
                    await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                }
                catch (Exception ex)
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, ex.Message);
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally
            {
                dbContext.Dispose();
            }

        }
    }
}
