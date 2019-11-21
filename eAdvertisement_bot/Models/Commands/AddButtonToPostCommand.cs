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
                user.User_State_Id = Convert.ToInt64("102" + update.CallbackQuery.Data.Substring(17));
                dbContext.SaveChanges();

                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Send buttons in format like in message below\nYou can't use symbols such as ()[] in text or url\nIf you attach buttons, you won't be able to add more than 1 image!", true);
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "You can add up to 5 buttons, each line for a new button\n\n(Button 1)[http://example1.com]\n(Button 2)[http://example2.com]");
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
