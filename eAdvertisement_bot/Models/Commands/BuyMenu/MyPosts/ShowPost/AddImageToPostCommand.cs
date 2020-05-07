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
    public class AddImageToPostCommand : Command
    {
        public override string Name => "/addImageToPostN";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("/addImageToPostN");
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                user.User_State_Id = 101;
                user.Object_Id = Convert.ToInt64(update.CallbackQuery.Data.Substring(16));
                dbContext.SaveChanges();

                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Отправьте картинку, она будет прикреплена к посту\nЕсли вы прикрепите более одной картинки то вы не сможете использовать кнопки!", true);
            }
            catch(Exception ex)
            {
                MainLogger.LogException(ex, "AddImageToPostCommand");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
