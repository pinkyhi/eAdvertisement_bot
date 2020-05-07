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
    public class ChangeAutobuyStateCommand : Command
    {
        public override string Name => "/changeAutobuyState";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("cabst");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                dbContext.Autobuys.Find(Convert.ToInt32(user.Object_Id)).State = dbContext.Autobuys.Find(Convert.ToInt32(user.Object_Id)).State == 0 ? dbContext.Autobuys.Find(Convert.ToInt32(user.Object_Id)).State = 1 : dbContext.Autobuys.Find(Convert.ToInt32(user.Object_Id)).State = 0;
                dbContext.SaveChanges();

                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Состояние измененно. Автозакуп " + (dbContext.Autobuys.Find(Convert.ToInt32(user.Object_Id)).State == 0 ? "выключен" : "включен") + "!\n", true);
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Состояние измененно. Автозакуп " + (dbContext.Autobuys.Find(Convert.ToInt32(user.Object_Id)).State == 0 ? "выключен" : "включен") + "!\n", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Показать обновленное меню", CallbackData = "sabN" + user.Object_Id }));
            }
            catch (Exception ex)
            {
                MainLogger.LogException(ex, addStr: "ChangeAutobuyState");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
