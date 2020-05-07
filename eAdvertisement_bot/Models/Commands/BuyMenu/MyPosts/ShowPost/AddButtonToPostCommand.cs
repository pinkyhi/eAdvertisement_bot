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
    public class AddButtonToPostCommand : Command
    {
        public override string Name => "/addButtonToPostN";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("/addButtonToPostN");
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                user.User_State_Id = 102;
                user.Object_Id = Convert.ToInt64(update.CallbackQuery.Data.Substring(17));
                dbContext.SaveChanges();

                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Отправьте кнопки как в формате ниже\nВы не можете использовать символы: ()[]\nЕсли вы добавите кнопки, вы не сможете добавить более одной картинки!", true);
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Вы можете прикрепить до 5-ти кнопок, каждая кнопка с новой строчки\nПример\n\n(Button 1)[http://example1.com]\n(Button 2)[http://example2.com]");
            }
            catch (Exception ex)
            {
                MainLogger.LogException(ex, "AddButtonToPostCommand");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
