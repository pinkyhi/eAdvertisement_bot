using System;
using eAdvertisement_bot.DAO;
using eAdvertisement_bot.Models.DbEntities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace eAdvertisement_bot.Models.Commands
{
    public class ChoosePostForAddCommandcs : Command
    {
        public override string Name => "/choosePostForAdNDT";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("cpfaN");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            long channelId = Convert.ToInt64(update.CallbackQuery.Data.Substring(5, update.CallbackQuery.Data.IndexOf('D') - 5));
            string dateStr = update.CallbackQuery.Data.Substring(update.CallbackQuery.Data.IndexOf('D') + 1, update.CallbackQuery.Data.IndexOf('T') - (update.CallbackQuery.Data.IndexOf('D') + 1));
            string tags = update.CallbackQuery.Data.Substring(update.CallbackQuery.Data.IndexOf('T') + 1);
            DateTime dateTime = DateTime.Parse(dateStr);
            try
            {
                Channel channel = dbContext.Channels.Find(channelId);
                List<Publication> posts = dbContext.Publications.Where(p => p.User_Id == update.CallbackQuery.From.Id).ToList();
                InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[posts.Count + 1][];

                for(int i = 0; i < posts.Count; i++)
                {
                    keyboard[i] = new[] { new InlineKeyboardButton { Text = posts[i].Name, CallbackData = "bpicN" + channelId + "D" + dateStr + "T" + tags + "P" + posts[i].Publication_Id } };
                }

                keyboard[keyboard.Length - 1] = new[] { new InlineKeyboardButton { Text = "Back", CallbackData = "/showPlacesForBuyerN" + channelId + "D" + Convert.ToString(dateTime).Substring(0, 10) + "T" + tags } };


                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Choose post", replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, disableWebPagePreview: true);

                try
                {
                    await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                }
                catch (Exception ex)
                {
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, ex.Message);
                }
            }
            catch (Exception ex) { await botClient.SendTextMessageAsync(update.CallbackQuery.From.Id, ex.Message); }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
