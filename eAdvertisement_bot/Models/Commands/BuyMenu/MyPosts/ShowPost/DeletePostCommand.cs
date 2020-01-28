using eAdvertisement_bot.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace eAdvertisement_bot.Models.Commands
{
    public class DeletePostCommand : Command
    {
        public override string Name => "/deletePostN";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("/deletePostN");
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                List<DbEntities.Button> buttons = dbContext.Buttons.ToList();
                List<DbEntities.Media> medias = dbContext.Medias.ToList();
                dbContext.Publications.Remove(dbContext.Publications.Find(Convert.ToInt32(update.CallbackQuery.Data.Substring(12))));
                dbContext.SaveChanges();

                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Post is deleted", true);
                Command c = new MyPostsMenuCommand();
                await c.Execute(update, botClient);

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
