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
    public class InfoMenuCommand : Command
    {
        public override string Name => "/infoMenu";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.Equals("/infoMenu");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            InlineKeyboardButton[][] keyboard = new[] { new[] { new InlineKeyboardButton {Text = "Назад", CallbackData = "/backToStartMenu" } } };

            await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Связь с администратором\n@olejchanskiy\nТакже читайте [FAQ](https://telegra.ph/eAdvertisement-03-07), оно будет дополнятья", replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

            try
            {
                await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, ex.Message);
            }
        }
    }
}
