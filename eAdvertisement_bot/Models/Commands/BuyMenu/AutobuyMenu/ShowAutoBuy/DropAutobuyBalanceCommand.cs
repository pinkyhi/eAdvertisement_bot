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
    public class DropAutobuyBalanceCommand : Command
    {
        public override string Name => "dropAutobuyBalance";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("drabbal");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                dbContext.SaveChanges();

                int t = dbContext.Autobuys.Find(Convert.ToInt32(user.Object_Id)).Balance;
                dbContext.Autobuys.Find(Convert.ToInt32(user.Object_Id)).Balance = 0;
                user.Balance += t;
                dbContext.SaveChanges();
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Баланс сброшен", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Показать обновленное меню", CallbackData = "sabN" + user.Object_Id }));


            }
            catch (Exception ex)
            {
                MainLogger.LogException(ex, addStr: "DropAutobuyBalance");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
