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
    public class ShowChannelForBuyerCommand : Command
    {
        public override string Name => "/showChannelForBuyerNT";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("/showChannelForBuyerN");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            long channelId = Convert.ToInt64(update.CallbackQuery.Data.Substring(21, update.CallbackQuery.Data.IndexOf('T')-21));
            string tags = update.CallbackQuery.Data.Substring(update.CallbackQuery.Data.IndexOf('T')+1);
            try
            {
                Channel channel = dbContext.Channels.Find(channelId);
                string info = "[" + channel.Name + "](" + channel.Link + ")" +
                    "\nПодписчиков: " + channel.Subscribers +
                    "\nОхват: " + channel.Coverage +
                    "\nERR: " + Math.Round(Convert.ToDouble(channel.Coverage) / Convert.ToDouble(channel.Subscribers), 2) +
                    "\nЦена: " + channel.Price +
                    "\nCPM: " + channel.Cpm;
                if (channel.Description != null && !channel.Description.Equals(""))
                {
                    info += "\n*Описание*\n" + channel.Description;
                }

                InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[1][];

                keyboard[0] = new[]
                {
                    new InlineKeyboardButton { Text = "Купить место", CallbackData = "/showPlacesCalendarForBuyerN"+channelId+"T"+tags},
                    new InlineKeyboardButton { Text = "Назад", CallbackData = "/manualPurchaseMenuP"+tags},
                };

                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, info, replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, disableWebPagePreview: true);

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
