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
    public class BackFromBuyMenuCommand : Command
    {
        public override string Name => "/backFromBuyMenu";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.Equals("/backFromBuyMenu");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
            long userId = update.CallbackQuery.From.Id;
            DbEntities.User userEntity = dbContext.Users.Find(userId);

            InlineKeyboardMarkup keyboard;
            if (userEntity.Stopped)
            {
                keyboard = entryStoppedBotKeyboard;
            }
            else
            {
                keyboard = entryLaunchedBotKeyboard;
            }
            await botClient.SendTextMessageAsync(userId, "You are already initialized.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: keyboard);

            dbContext.Dispose();
        }
    }
}
