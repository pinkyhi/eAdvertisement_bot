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
    public class InputCommand : Command
    {
        public override string Name => "/inputCommand";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.Equals("input");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            try
            {
                InlineKeyboardButton[][] keyboard = new[] { new[] { new InlineKeyboardButton { Text = "Назад", CallbackData = "/backToStartMenu" } } };

                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"Отправьте деньги на киви кошелек `380958374332` с комментарием:\n\n `{update.CallbackQuery.From}`\n\nПоддержка @olejchanskiy", replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

                try
                {
                    await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                }
                catch
                { }
            }
            catch (Exception ex)
            {
                MainLogger.LogException(ex, "InputMenuCommand");
            }
        }
    }
}
