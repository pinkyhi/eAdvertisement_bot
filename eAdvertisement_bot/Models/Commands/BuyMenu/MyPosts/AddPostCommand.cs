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
    public class AddPostCommand : Command
    {
        public override string Name => "/addPost";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("/addPost");
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                if (dbContext.Publications.Count(p => p.User_Id == update.CallbackQuery.From.Id) < 8)
                {
                    DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                    DbEntities.Publication newPost = new DbEntities.Publication { User_Id = user.User_Id, Name = "newPost",Text=null };
                    dbContext.Publications.Add(newPost);
                    dbContext.SaveChanges();

                    await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, null, false);

                    MyPostsMenuCommand x = new MyPostsMenuCommand();
                    await x.Execute(update, botClient);
                    
                }
                else
                {
                    await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Limit of posts is 8\nDelete one to add another", true);
                    return;
                }

            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat,ex.Message);
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
