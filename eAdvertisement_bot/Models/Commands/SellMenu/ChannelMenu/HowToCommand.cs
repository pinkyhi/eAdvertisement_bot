using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace eAdvertisement_bot.Models.Commands
{
    public class HowToCommand : Command // OLD VERSION
    {
        public override string Name => "/howTo";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("/howTo");
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            string action = update.CallbackQuery.Data.Substring(6);
            if (action.Equals("ChangeCpm"))
            {
                long x = update.CallbackQuery.Message.Chat.Id;
                await botClient.SendTextMessageAsync(x, "To change cpm write: *cpm: 'your integer cpm'*\n*Example*\ncpm: 100", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, null, false);
            }
            else if(action.Equals("ChangeDescription"))
            {
                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, null, false);
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "To change description write: *description: 'your description'*\n*Example*\ndescription: My channel is the best!", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }
            else if(action.Equals("AddAdvPlace"))
            {
                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, null, false);
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "To add advertisement place write: *place: 'time in format hh:mm'*\n*Example*\nplace: 09:00", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }
        }
    }
}
