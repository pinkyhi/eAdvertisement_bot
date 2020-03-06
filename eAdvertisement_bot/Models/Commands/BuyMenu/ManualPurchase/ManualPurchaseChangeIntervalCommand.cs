using eAdvertisement_bot.DAO;
using eAdvertisement_bot.Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace eAdvertisement_bot.Models.Commands
{
    public class ManualPurchaseChangeIntervalCommand : Command
    {
        public override string Name => "/manualPurchaseChangeIntervalT";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("mpciT");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            string tags = update.CallbackQuery.Data.Substring(5);
            AppDbContext dbContext = new AppDbContext();
            try
            {
                dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id)).User_State_Id = 301;
                dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id)).Tag = tags;
                dbContext.SaveChanges();

                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "", true);
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Отправьте интервал в целочисленном формате от-до\n*1-100000*", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);


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
