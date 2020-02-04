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
    public class ShowPlacesCalendarForBuyerCommand : Command
    {
        public override string Name => "/showPlacesCalendarForBuyer";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("/showPlacesCalendarForBuyerN");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            long channelId = Convert.ToInt64(update.CallbackQuery.Data.Substring(28, update.CallbackQuery.Data.IndexOf('T') - 28));
            string tags = update.CallbackQuery.Data.Substring(update.CallbackQuery.Data.IndexOf('T') + 1);
            try
            {
                Channel channel = dbContext.Channels.Find(channelId);


                InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[4][];

                DateTime nowIs = DateTime.Today;

                for(int i = 0; i < 3; i++)
                {
                    keyboard[i] = new InlineKeyboardButton[5];
                    for(int j = 0; j < 5; j++)
                    {

                        if(dbContext.Advertisements.Count(a => a.Channel_Id == channelId && a.Date_Time.Date == nowIs.Date && a.Advertisement_Status_Id == 4) > 6)
                        {
                            keyboard[i][j] = new InlineKeyboardButton { Text = "×" + Convert.ToString(nowIs.Date).Substring(0, 5) + "×", CallbackData = "/showPlacesForBuyerN" + channelId + "D" + Convert.ToString(nowIs.Date).Substring(0, 10) + "T"+tags };
                        }
                        else
                        {
                            keyboard[i][j] = new InlineKeyboardButton { Text = Convert.ToString(nowIs.Date).Substring(0, 5), CallbackData = "/showPlacesForBuyerN" + channelId + "D" + Convert.ToString(nowIs.Date).Substring(0,10)+"T"+tags};

                        }
                        nowIs=nowIs.AddDays(1);
                    }
                }

                keyboard[3] = new[]
                {
                    new InlineKeyboardButton { Text = "Back", CallbackData = "/showChannelForBuyerN"+channelId+"T"+tags},
                };

                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Here you can choose day where you want to buy an ad", replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, disableWebPagePreview: true);

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
