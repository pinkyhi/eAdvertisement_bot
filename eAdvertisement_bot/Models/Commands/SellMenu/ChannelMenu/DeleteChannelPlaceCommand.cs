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
    public class DeleteChannelPlaceCommand : Command
    {
        public override string Name => "/deletePlace";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("/deletePlaceN");
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                int placeId = Convert.ToInt32(update.CallbackQuery.Data.Substring(13));
                List<DbEntities.Place> places = dbContext.Places.Where(p => p.Place_Id == placeId).ToList();
                long channelId = places[0].Channel_Id;
                dbContext.Places.RemoveRange(dbContext.Places.Where(p => p.Place_Id == placeId).ToList());
                dbContext.SaveChanges();
                places = dbContext.Places.Where(p => p.Channel_Id == channelId).ToList();

                InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[places.Count + 2][];
                keyboard[0] = new[]
                {
                    new InlineKeyboardButton { Text = "Change desc", CallbackData = "/howToChangeDescription"},
                    new InlineKeyboardButton { Text = "Change cpm", CallbackData = "/howToChangeCpm"},
                    new InlineKeyboardButton { Text = "Add adv place", CallbackData = "/howToAddAdvPlace"},
                };
                int indexToPaste = 1;
                while (indexToPaste < places.Count() + 1)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "Delete place " + places[indexToPaste - 1].Time, CallbackData = "/deletePlaceN" + places[indexToPaste - 1].Place_Id }, };
                    indexToPaste++;
                }
                keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "Back", CallbackData = "/sellMenuP0" }, };


                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Place is deleted", true);  // ...,...,alert    AnswerCallbackQuery is required to send to avoid clock animation ob the button   

                try
                {
                    await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                }
                catch { }
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.Text, replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, ex.Message);
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
