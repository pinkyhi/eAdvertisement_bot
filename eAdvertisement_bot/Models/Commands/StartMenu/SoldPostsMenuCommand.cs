using eAdvertisement_bot.DAO;
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
                    new[] { new InlineKeyboardButton { Text = "Добавить собственную продажу", CallbackData = "osmP0" }},
                    new[] { new InlineKeyboardButton { Text = "Назад", CallbackData = "/backToStartMenu" } }, 
                   
                };

                List<Advertisement> ads = dbContext.Advertisements.Include("Channel").Include("Channel.User").Include("Advertisement_Status").Where(a => a.Channel.User_Id == update.CallbackQuery.From.Id).Where(a => a.Advertisement_Status_Id == 4 || a.Advertisement_Status_Id == 2 || a.Advertisement_Status_Id == 9).ToList();

                string text = "*Здесь находятся посты которые вы продали*\n\n";
                
                for(int i = 0; i < ads.Count; i++)
                {
                    text += "[" + ads[i].Channel.Name + "]" + "(" + ads[i].Channel.Link + ")" +
                        "\nЦена: " + ads[i].Price*ads[i].Channel.User.Commission +
                        "\nДолжно быть запощено в: " + ads[i].Date_Time +
                        "\nСтатус: " + ads[i].Advertisement_Status.Name+"\n";
                }

                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, text, replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, disableWebPagePreview: true);

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
                MainLogger.LogException(ex, "SoldPostsMenuCommand");
            }
            finally
            {
                dbContext.Dispose();
            }

        }
    }
}
