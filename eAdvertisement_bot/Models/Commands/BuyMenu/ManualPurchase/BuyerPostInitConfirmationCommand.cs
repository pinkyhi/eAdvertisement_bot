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
    public class BuyerPostInitConfirmationCommand : Command
    {
        public override string Name => "/buyerPostInitConfirmationNDTP";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("bpicN");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            long channelId = Convert.ToInt64(update.CallbackQuery.Data.Substring(5, update.CallbackQuery.Data.IndexOf('D') - 5));
            string dateStr = update.CallbackQuery.Data.Substring(update.CallbackQuery.Data.IndexOf('D') + 1, update.CallbackQuery.Data.IndexOf('T') - (update.CallbackQuery.Data.IndexOf('D') + 1));
            string tags = update.CallbackQuery.Data.Substring(update.CallbackQuery.Data.IndexOf('T') + 1, update.CallbackQuery.Data.IndexOf('P') - (update.CallbackQuery.Data.IndexOf('T') + 1));
            int postId = Convert.ToInt32(update.CallbackQuery.Data.Substring(update.CallbackQuery.Data.IndexOf('P') + 1));
            DateTime dateTime = DateTime.Parse(dateStr);
            try
            {
                Channel channel = dbContext.Channels.Find(channelId);
                Publication post = dbContext.Publications.Find(postId);
                List<Button> buttons = dbContext.Buttons.Where(b => b.Publication_Id == post.Publication_Id).ToList();

                List<Media> media = dbContext.Medias.Where(m => m.Publication_Id == post.Publication_Id).ToList();
                InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[3][];

                //string json = JsonSerializer.Serialize(post);

                SendPost(post, update, botClient);

                keyboard[0] = new[] { new InlineKeyboardButton { Text = "Принять", CallbackData = "ambN" + channelId + "D" + Convert.ToString(dateTime) + "T" + tags + "P" + postId} };
                keyboard[1] = new[] { new InlineKeyboardButton { Text = "Назад", CallbackData = "cpfaN" + channelId + "D" + Convert.ToString(dateTime) + "T" + tags } };
                keyboard[keyboard.Length - 1] = new[]
{
                    new InlineKeyboardButton { Text = "Отмена", CallbackData = "/manualPurchaseMenuP" + tags },
                }; 

                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Цена: "+channel.Price+"\nВы уверены?", replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, disableWebPagePreview: true);

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
