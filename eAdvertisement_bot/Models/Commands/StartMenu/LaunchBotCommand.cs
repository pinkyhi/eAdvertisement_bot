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
    public class LaunchBotCommand : Command
    {
        public override string Name => @"/launchBot";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.Equals("/launchBot");
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            dbContext.Users.First(u => u.User_Id == update.CallbackQuery.From.Id).Stopped = false;
            dbContext.SaveChanges();

            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Бот включен", true);  // ...,...,alert    AnswerCallbackQuery is required to send to avoid clock animation ob the button

            InlineKeyboardMarkup keyboard = entryLaunchedBotKeyboard;

            await botClient.EditMessageReplyMarkupAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, keyboard);

            dbContext.Dispose();
        }
    }
}
