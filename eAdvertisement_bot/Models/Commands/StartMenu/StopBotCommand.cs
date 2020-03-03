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
    public class StopBotCommand : Command
    {
        public override string Name => @"/stopBot";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.Equals("/stopBot");
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            dbContext.Users.First(u => u.User_Id == update.CallbackQuery.From.Id).Stopped = true;
            dbContext.SaveChanges();

            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Бот остановлен", true);  // ...,...,alert    AnswerCallbackQuery is required to send to avoid clock animation ob the button

            InlineKeyboardMarkup keyboard = entryStoppedBotKeyboard;

            await botClient.EditMessageReplyMarkupAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, keyboard);    //i don't know why here is editing and no deleting and sending, but i don't want to rework it

            dbContext.Dispose();
        }
    }
}
