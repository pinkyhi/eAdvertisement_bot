using eAdvertisement_bot.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace eAdvertisement_bot.Models.Commands
{
    public class OnPhotoMessageEvent : Command
    {
        public override string Name => "/onPhotoMessageEvent";

        public override bool Contains(Update update)
        {
            try
            {
                if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
                {
                    if (update.Message.Type == Telegram.Bot.Types.Enums.MessageType.Photo)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {

            }
            return false;
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.Message.From.Id));
                if (user.User_State_Id==101)
                {
                    DbEntities.Publication post = dbContext.Publications.Find(Convert.ToInt32(user.Object_Id));

                    if (post != null)
                    {
                        int countOfMediasOnPost = dbContext.Medias.Count(m => m.Publication_Id == post.Publication_Id);
                        if (countOfMediasOnPost==1 && dbContext.Buttons.Count(m => m.Publication_Id == post.Publication_Id) > 0)
                        {
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Вы не можете прикрепить более одной картинки так как вы используете кнопки\nПересоздайте пост чтобы избежать этой проблемы");
                            await botClient.DeleteMessageAsync(update.Message.Chat.Id, update.Message.MessageId);
                            return;
                        }
                        if (countOfMediasOnPost < 10)
                        {

                            dbContext.Medias.Add(new DbEntities.Media { Publication_Id = post.Publication_Id, Path = update.Message.Photo[update.Message.Photo.Length - 1].FileId });

                            user.User_State_Id = 0;
                            dbContext.SaveChanges();
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Картинка добавлена успешно :)", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Показать обновленный пост ", CallbackData = "/showPostN" + post.Publication_Id }));
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Лимит картинок – 10 шт.\nЕсли вы добавили лишние картинки – пересоздайте пост");
                        }
                    }
                    else
                    {
                        try
                        {
                            await botClient.DeleteMessageAsync(update.Message.Chat.Id, update.Message.MessageId);
                        }
                        catch { }
                        return;
                    }
                }  

            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, ex.StackTrace + "\n" + ex.Message +"\n");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
