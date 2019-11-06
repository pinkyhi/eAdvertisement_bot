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
    public class BuyMenuCommand : Command
    {
        public override string Name => "/buyMenu";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.Equals("/buyMenu");
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            DbEntities.User userEntity = dbContext.Users.Find(update.Message.From.Id);
            long hold = dbContext.Advertisements.Where(a => a.User_Id==userEntity.User_Id && a.Advertisement_Status_Id == 1).Sum(a=>a.Price);
            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, null, false);  // ...,...,alert    AnswerCallbackQuery is required to send to avoid clock animation ob the button

            InlineKeyboardMarkup keyboard = buyMenuKeyboard;

            await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, "Balance: "+userEntity.Balance+"\nHold: "+hold);
            await botClient.EditMessageReplyMarkupAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, keyboard);
        }
    }
}
