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
    public class ChangeAutobuyMaxCpmCommand : Command
    {
        public override string Name => "/changeAutobuyMaxCpm";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("cabmac");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                user.User_State_Id = 402;
                dbContext.SaveChanges();

                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Отправьте целочисленный максимальный cpm, бот не будет покупать рекламу с cpm больше этого.", true);
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Отправьте целочисленный максимальный cpm\n*100*", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }
            catch (Exception ex)
            {
                MainLogger.LogException(ex, addStr: "ChangeAutobuyMaxCpm");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}

