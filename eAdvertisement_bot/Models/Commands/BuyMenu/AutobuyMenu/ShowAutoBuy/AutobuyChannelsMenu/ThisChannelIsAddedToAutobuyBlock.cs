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
    public class ThisChannelIsAddedToAutobuyBlock : Command
    {
        public override string Name => "/thisChannelIsAddedToThisAutobuy";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.Equals("tciatabBlock");
            }
        }
        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Этот канал уже прикреплен к автозакупу", true);
        }
    }
}
