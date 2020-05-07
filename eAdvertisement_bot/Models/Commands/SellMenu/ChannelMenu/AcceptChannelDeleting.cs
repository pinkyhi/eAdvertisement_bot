using eAdvertisement_bot.DAO;
using eAdvertisement_bot.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
namespace eAdvertisement_bot.Models.Commands
{
    public class AcceptChannelDeleting : Command
    {
        public override string Name => "/acceptChannelDeleting";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("/acceptChannelDeletingN");
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            long channelId = Convert.ToInt64(update.CallbackQuery.Data.Substring(23));
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                if (user.User_State_Id == 204)
                {
                    List<DbEntities.Place> p = dbContext.Places.Where(p => p.Channel_Id == channelId).ToList();
                    List<DbEntities.Channel_Category> cg = dbContext.Channel_Categories.Where(p => p.Channel_Id == channelId).ToList();
                    List<DbEntities.Autobuy_Channel> ac = dbContext.Autobuy_Channels.Where(p => p.Channel_Id == channelId).ToList();

                    dbContext.Channels.Remove(dbContext.Channels.Find(channelId));

                    dbContext.SaveChanges();
                    await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Канал удален успешно", true);
                    update.CallbackQuery.Data = "/sellMenuP0";
                    SellMenuCommand scfsc = new SellMenuCommand();
                    await scfsc.Execute(update, botClient);
                }
                else
                {
                    await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Вы должны быть в меню удаления чтобы удалить канал", true);
                }

            }
            catch (Exception ex)
            {
                MainLogger.LogException(ex, "AcceptChannelDeleting");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
