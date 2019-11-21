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
                if (Convert.ToString(user.User_State_Id).StartsWith("101"))
                {
                    DbEntities.Publication post = dbContext.Publications.Find(Convert.ToInt32((Convert.ToString(user.User_State_Id)).Substring(3)));

                    if (post != null)
                    {
                        int countOfMediasOnPost = dbContext.Medias.Count(m => m.Publication_Id == post.Publication_Id);
                        if (countOfMediasOnPost==1 && dbContext.Buttons.Count(m => m.Publication_Id == post.Publication_Id) > 0)
                        {
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "You can't attach to this post more than 1 picture, because you use buttons\nYou can recreate post to avoid this problem");
                            await botClient.DeleteMessageAsync(update.Message.Chat.Id, update.Message.MessageId);
                            return;
                        }
                        if (countOfMediasOnPost < 10)
                        {
                            dbContext.Medias.Add(new DbEntities.Media { Publication_Id = post.Publication_Id, Path = update.Message.Photo[2].FileId });
                            user.User_State_Id = 0;
                            dbContext.SaveChanges();
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Image is added succesfully :)", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Show updated post", CallbackData = "/showPostN" + post.Publication_Id }));
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Limit of images is 10\nIf you added excess image you can recreate post");
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
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, ex.Message);
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
