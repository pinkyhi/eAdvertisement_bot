using eAdvertisement_bot.DAO;
using eAdvertisement_bot.Logger;
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
    public class ShowPostCommand : Command
    {
        public override string Name => "/showPostN";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("/showPostN");
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            int postId = Convert.ToInt32(update.CallbackQuery.Data.Substring(10));
            AppDbContext dbContext = new AppDbContext();
            try
            {

                DbEntities.Publication postToShow = dbContext.Publications.Include("Media").Include("Buttons").FirstOrDefault(p=>p.Publication_Id==postId);


                SendPost(postToShow,update, botClient);

                InlineKeyboardButton[][] keyboardControll = new[]
                {
                    new[]
                    {
                        new InlineKeyboardButton{Text = "Медиа", CallbackData = "/addImageToPostN"+postId},
                        new InlineKeyboardButton{Text = "Кнопки", CallbackData = "/addButtonToPostN"+postId}
                    },
                    new[]
                    {
                        new InlineKeyboardButton{Text = "Текст", CallbackData = "/changeTextOnPostN"+postId},
                        new InlineKeyboardButton{Text = "Удалить пост", CallbackData = "/deletePostN"+postId}
                    },
                    new[]
                    {
                        new InlineKeyboardButton{Text="Изменить имя",CallbackData = "/changePostNameN"+postId}
                    },
                    new[]
                    {
                        new InlineKeyboardButton{Text="Назад",CallbackData = "/myPostsMenu"}
                    }
                };
                
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Кнопки для изменения поста\n*Не забудьте выставить какой-то текст, иначе реклама не запостится!*", replyMarkup: new InlineKeyboardMarkup(keyboardControll), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                try
                {
                    await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                }
                catch { }
                

            }
            catch(Exception ex)
            {
                MainLogger.LogException(ex, "ShowPostCommand");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
