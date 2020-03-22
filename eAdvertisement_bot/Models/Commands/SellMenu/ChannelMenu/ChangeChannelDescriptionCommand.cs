using eAdvertisement_bot.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace eAdvertisement_bot.Models.Commands
{
    public class ChangeChannelDescriptionCommand : Command
    {
        public override string Name => "/changeDescription";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("/changeDescription");
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                user.User_State_Id = 203;

                dbContext.SaveChanges();

                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Отправьте краткое описание ваего канала", true);
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Пример \n*This is the best channel!*", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
            
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, ex.StackTrace + "\n" + ex.Message +"\n");
            }
            finally
            {
                dbContext.Dispose();
            }


        }
    }
}
