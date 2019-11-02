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
    public class StartCommand : Command
    {
        public override string Name => @"/start";

        
        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
            {
                return false;
            }
            else
            {
                return message.Text.Equals(this.Name);    // If it command is in text of update method TODO: now true with /start and /starts 
            }
        }

        public override async Task Execute(Message message, TelegramBotClient botClient)
        {
            var chatId = message.From.Id;
            AppDbContext dbContext = new AppDbContext();

            InlineKeyboardButton statusBotIKB=new InlineKeyboardButton();
            InlineKeyboardButton buyIKB = new InlineKeyboardButton { Text = "Buy", CallbackData = "/buyMenu" };
            InlineKeyboardButton sellIKB = new InlineKeyboardButton { Text = "Sell", CallbackData = "/sellMenu" };
            InlineKeyboardButton soldPostsIKB = new InlineKeyboardButton { Text = "Sold posts", CallbackData = "/soldPostsMenu" };
            InlineKeyboardButton boughtPostsIKB = new InlineKeyboardButton { Text = "Bought posts", CallbackData = "/boughtPostsMenu" };
            InlineKeyboardButton infoIKB = new InlineKeyboardButton { Text = "Info", CallbackData = "/infoMenu" };



            try
            {

                dbContext.Users.Add(new DbEntities.User { User_Id = message.From.Id, Nickname = message.From.Username, FirstName = message.From.FirstName, LastName = message.From.LastName, Language = message.From.LanguageCode, Stopped = false });
                dbContext.SaveChanges();
                dbContext.Dispose();
                statusBotIKB = new InlineKeyboardButton { Text = "Stop Bot", CallbackData = "/stopBot" };

                InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(new[]
                {
                    new[] //first row
                    {
                        statusBotIKB,
                    },
                    new[] // second row
                    {
                        buyIKB,
                        sellIKB,
                    },
                    new[] // third row
                    {
                        soldPostsIKB,
                        boughtPostsIKB,
                    },
                    new[] // fourth row
                    {
                        infoIKB,
                    }
                });

                await botClient.SendTextMessageAsync(chatId, "Hi, "+message.From.FirstName+". Initialization is succesfull :)", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: keyboard);
            }
            catch (Exception ex)
            {
                if (ex.InnerException.ToString().Contains("Duplicate"))
                {
                    if ( dbContext.Users.First(u => u.User_Id == chatId).Stopped)
                    {
                        statusBotIKB = new InlineKeyboardButton { Text = "Launch Bot", CallbackData = "/launchBot" };
                    }
                    else
                    {
                        statusBotIKB = new InlineKeyboardButton { Text = "Stop Bot", CallbackData = "/stopBot" };
                    }

                    InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[] //first row
                        {
                            statusBotIKB,
                        },
                        new[] // second row
                        {
                            buyIKB,
                            sellIKB,
                        },
                        new[] // third row
                        {
                            soldPostsIKB,
                            boughtPostsIKB,
                        },
                        new[] // fourth row
                        {
                            infoIKB,
                        }
                    });

                    await botClient.SendTextMessageAsync(chatId, "You are already initialized.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: keyboard);

                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId, "Sorry, now are some troubles with initialization.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                }
                
            }
        }
    }
}
