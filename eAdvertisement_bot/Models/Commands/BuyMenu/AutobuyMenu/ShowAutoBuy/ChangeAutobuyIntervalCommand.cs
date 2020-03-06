using eAdvertisement_bot.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace eAdvertisement_bot.Models.Commands
{
    public class ChangeAutobuyIntervalCommand :Command
    {
        public override string Name => "/changeAutobuyBuyInterval";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("cabbi");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                user.User_State_Id = 404;
                dbContext.SaveChanges();

                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Напишите интервал в днях, бот не будет покупать рекламу в каналах чаще чем это кол-во дней. Не более 60 дней", true);
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Напишите интервал в днях\n*14*", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
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
