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
    public class AddBalanceToAutobuyCommand : Command
    {
        public override string Name => "addBalanceToAutobuy";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("abaltab");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                user.User_State_Id = 405;
                dbContext.SaveChanges();

                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Напишите целочисленную сумму, она будет перенесена с вашего баланса", true);
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Напишите целочисленную сумму\n*1000*", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }
            catch (Exception ex)
            {
                MainLogger.LogException(ex, addStr: "AddBalanceToAutobuy");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
