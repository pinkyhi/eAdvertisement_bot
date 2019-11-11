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
    public class AddChannelCommand : Command
    {
        public override string Name => "/addChannel";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.Equals("/addChannel");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Back to sell menu", CallbackData = "/backToSellMenu" });
                await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, "Send any post from a channel where this bot is administrator", replyMarkup: keyboard);
                dbContext.Users.First(u => u.User_Id == update.CallbackQuery.From.Id).User_State_Id = 1;
                dbContext.SaveChanges();
            }
            catch { }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
