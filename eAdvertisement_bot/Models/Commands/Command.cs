using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace eAdvertisement_bot.Models.Commands
{
    public abstract class Command
    {
        public abstract string Name { get; }
        public abstract Task Execute(Update update, TelegramBotClient botClient); // This method contains the main logic which should be executed if this command contains in update message
        public abstract bool Contains(Update update); // This method is used to check if this command contains in update message


        // Static elements


        protected InlineKeyboardMarkup entryStoppedBotKeyboard = new InlineKeyboardMarkup(new[]
        {
                        new[] //first row
                        {
                            new InlineKeyboardButton { Text = "Launch Bot", CallbackData = "/launchBot" },
                        },
                        new[] // second row
                        {
                            new InlineKeyboardButton { Text = "Buy", CallbackData = "/buyMenu" },
                            new InlineKeyboardButton { Text = "Sell", CallbackData = "/sellMenu" },
                        },
                        new[] // third row
                        {                            
                            new InlineKeyboardButton { Text = "Bought posts", CallbackData = "/boughtPostsMenu" },
                            new InlineKeyboardButton { Text = "Sold posts", CallbackData = "/soldPostsMenu" },
                        },
                        new[] // fourth row
                        {
                            new InlineKeyboardButton { Text = "Info", CallbackData = "/infoMenu" },
                        }
        });


        protected InlineKeyboardMarkup entryLaunchedBotKeyboard = new InlineKeyboardMarkup(new[]
        {
                        new[] //first row
                        {
                            new InlineKeyboardButton { Text = "Stop Bot", CallbackData = "/stopBot" },
                        },
                        new[] // second row
                        {
                            new InlineKeyboardButton { Text = "Buy", CallbackData = "/buyMenu" },
                            new InlineKeyboardButton { Text = "Sell", CallbackData = "/sellMenu" },
                        },
                        new[] // third row
                        {
                            new InlineKeyboardButton { Text = "Bought posts", CallbackData = "/boughtPostsMenu" },
                            new InlineKeyboardButton { Text = "Sold posts", CallbackData = "/soldPostsMenu" },
                        },
                        new[] // fourth row
                        {
                            new InlineKeyboardButton { Text = "Info", CallbackData = "/infoMenu" },
                        }
        });

        protected InlineKeyboardMarkup buyMenuKeyboard = new InlineKeyboardMarkup(new[]
       {
                        new[] //first row
                        {
                            new InlineKeyboardButton { Text = "Manual purchase", CallbackData = "/manualPurchaseMenu" },
                            new InlineKeyboardButton { Text = "Autobuy", CallbackData = "/autoBuyMenu" },
                        },
                        new[] // second row
                        {
                            new InlineKeyboardButton { Text = "My posts", CallbackData = "/myPostsMenu" },
                        },
                        new[] // third row
                        {
                            new InlineKeyboardButton { Text = "Back", CallbackData = "/backFromBuyMenu" },
                        }
        });

    }
}
