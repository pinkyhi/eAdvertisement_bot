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
    public class ChangeChannelCpmCommand : Command
    {
        public override string Name => "/changeCpm";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("/changeCpm");
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                user.User_State_Id = 202;

                dbContext.SaveChanges();

                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Отправьте CPM (целое число)", true);
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Пример \n*220*", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }
            catch(Exception ex)
            {
                MainLogger.LogException(ex, "ChangeChannelCpmCommand");
            }
            finally
            {
                dbContext.Dispose();
            }


        }
    }
}
