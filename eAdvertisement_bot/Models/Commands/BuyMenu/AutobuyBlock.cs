using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace eAdvertisement_bot.Models.Commands
{
    public class AutobuyBlock : Command
    {
        public override string Name => "abblock";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("abblock");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Эта функция будет в следующей версии релиза", false);  // ...,...,alert    AnswerCallbackQuery is required to send to avoid clock animation ob the button
        }
    }
}
