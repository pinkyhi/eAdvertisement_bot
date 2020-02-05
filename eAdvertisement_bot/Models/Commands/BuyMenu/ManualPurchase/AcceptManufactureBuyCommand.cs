using System;
using eAdvertisement_bot.DAO;
using eAdvertisement_bot.Models.DbEntities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Text.Json;

namespace eAdvertisement_bot.Models.Commands
{
    public class AcceptManufactureBuyCommand : Command
    {
        public override string Name => "/acceptManufactureBuyNDTP";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("ambN");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            long channelId = Convert.ToInt64(update.CallbackQuery.Data.Substring(4, update.CallbackQuery.Data.IndexOf('D') - 4));
            string dateStr = update.CallbackQuery.Data.Substring(update.CallbackQuery.Data.IndexOf('D') + 1, update.CallbackQuery.Data.IndexOf('T') - (update.CallbackQuery.Data.IndexOf('D') + 1));
            string tags = update.CallbackQuery.Data.Substring(update.CallbackQuery.Data.IndexOf('T') + 1, update.CallbackQuery.Data.IndexOf('P') - (update.CallbackQuery.Data.IndexOf('T') + 1));
            int postId = Convert.ToInt32(update.CallbackQuery.Data.Substring(update.CallbackQuery.Data.IndexOf('P') + 1));
            DateTime dateTime = DateTime.Parse(dateStr);
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                Channel channel = dbContext.Channels.Find(channelId);
                Publication post = dbContext.Publications.Find(postId);
                List<Button> buttons = dbContext.Buttons.Where(b => b.Publication_Id == post.Publication_Id).ToList();

                List<Media> media = dbContext.Medias.Where(m => m.Publication_Id == post.Publication_Id).ToList();
                InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[1][];

                string json = JsonSerializer.Serialize(post);
                

                keyboard[0] = new[] { new InlineKeyboardButton { Text = "Back to manual purchase menu", CallbackData = "/manualPurchaseMenuP"+tags } };

                if(user.Balance>= channel.Price)
                {
                    try
                    {
                        dbContext.Advertisements.Add(new Advertisement { Advertisement_Status_Id = 1, Alive = 24, Top = 1, Channel_Id = channelId, Publication_Snapshot = json, Date_Time = dateTime, User_Id = update.CallbackQuery.From.Id, Price = channel.Price });
                        user.Balance -= channel.Price;
                        dbContext.SaveChanges();
                        await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Post is sent, to back into manual purchase menu click the button below", replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, disableWebPagePreview: true);
                    }
                    catch
                    {
                        await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Sorry but this place is occupied now, to back into manual purchase menu click the button below", replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, disableWebPagePreview: true);
                    }

                }
                else
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Sorry, but you haven't enough money, to back into manual purchase menu click on the button below", replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, disableWebPagePreview: true);
                    
                }



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
