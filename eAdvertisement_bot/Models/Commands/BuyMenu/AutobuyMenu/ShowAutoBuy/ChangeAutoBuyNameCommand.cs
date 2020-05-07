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
    public class ChangeAutoBuyNameCommand : Command
    {
        public override string Name => "/changeAutobuyName";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("cabn");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                user.User_State_Id = 401;
                dbContext.SaveChanges();

                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Напишите имя автозакупа, оно будет видно только вам.", true);
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Отправьте имя автозакупа\nПример\n*My first autobuy*", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

            }
            catch (Exception ex)
            {
                MainLogger.LogException(ex, addStr: "ChangeAutoBuyName");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
