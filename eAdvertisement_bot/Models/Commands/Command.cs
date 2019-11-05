using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace eAdvertisement_bot.Models.Commands
{
    public abstract class Command
    {
        public abstract string Name { get; }
        public abstract Task Execute(Update update, TelegramBotClient botClient); // This method contains the main logic which should be executed if this command contains in update message
        public abstract bool Contains(Update update); // This method is used to check if this command contains in update message

    }
}
