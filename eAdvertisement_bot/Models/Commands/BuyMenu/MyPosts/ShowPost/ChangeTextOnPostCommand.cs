using eAdvertisement_bot.DAO;
using eAdvertisement_bot.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace eAdvertisement_bot.Models.Commands
{
    public class ChangeTextOnPostCommand : Command
    {
        public override string Name => "/changeTextOnPostN";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("/changeTextOnPostN");
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                user.User_State_Id = 103;
                user.Object_Id = Convert.ToInt64(update.CallbackQuery.Data.Substring(18));
                dbContext.SaveChanges();

                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Отправьте текст вашего поста\nФорматировние – markdown!", true);
            }
            catch (Exception ex)
            {
                MainLogger.LogException(ex, "ChangeTextOnPostCommand");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
