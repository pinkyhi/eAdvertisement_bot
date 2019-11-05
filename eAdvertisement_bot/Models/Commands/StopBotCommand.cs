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

            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Bot is stopped", true);  // ...,...,alert    AnswerCallbackQuery is required to send to avoid clock animation ob the button


            InlineKeyboardButton statusBotIKB = new InlineKeyboardButton { Text = "Launch Bot", CallbackData = "/launchBot" }; ;
            InlineKeyboardButton buyIKB = new InlineKeyboardButton { Text = "Buy", CallbackData = "/buyMenu" };
            InlineKeyboardButton sellIKB = new InlineKeyboardButton { Text = "Sell", CallbackData = "/sellMenu" };
            InlineKeyboardButton soldPostsIKB = new InlineKeyboardButton { Text = "Sold posts", CallbackData = "/soldPostsMenu" };
            InlineKeyboardButton boughtPostsIKB = new InlineKeyboardButton { Text = "Bought posts", CallbackData = "/boughtPostsMenu" };
            InlineKeyboardButton infoIKB = new InlineKeyboardButton { Text = "Info", CallbackData = "/infoMenu" };


            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[] //first row
                        {
                            statusBotIKB,
                        },
                        new[] // second row
                        {
                            buyIKB,
                            sellIKB,
                        },
                        new[] // third row
                        {
                            soldPostsIKB,
                            boughtPostsIKB,
                        },
                        new[] // fourth row
                        {
                            infoIKB,
                        }
                    });


            await botClient.EditMessageReplyMarkupAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, keyboard);

            dbContext.Dispose();
        }
    }
}
