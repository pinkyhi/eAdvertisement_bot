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
    public class ShowAttachedToAutobuyChannelCommand : Command
    {
        public override string Name => "showAttachedToAutobuyChannelNP";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("satabclN");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            long channelId = Convert.ToInt64(update.CallbackQuery.Data.Substring(8, update.CallbackQuery.Data.IndexOf('P') - 8));
            string page = update.CallbackQuery.Data.Substring(update.CallbackQuery.Data.IndexOf('P') + 1);

            AppDbContext dbContext = new AppDbContext();

            try
            {

                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                Autobuy autobuy = dbContext.Autobuys.Find(Convert.ToInt32(user.Object_Id));
                List<Autobuy_Channel> autobuyChannels = dbContext.Autobuy_Channels.Where(ac => ac.Autobuy_Id == autobuy.Autobuy_Id).ToList();
                Channel channel = dbContext.Channels.Find(channelId);

                InlineKeyboardButton[][] keyboard = new[]
                {
                    new[]
                    {
                        new InlineKeyboardButton{Text = "Unattach channel",CallbackData = "unatclfabN"+channelId+"P"+page},
                        new InlineKeyboardButton{Text = "Back", CallbackData = "cabcsP"+page}
                    }
                };
                string info = "[" + channel.Name + "](" + channel.Link + ")" +
                                    "\nSubscribers: " + channel.Subscribers +
                                    "\nCoverage: " + channel.Coverage +
                                    "\nERR: " + Math.Round(Convert.ToDouble(channel.Coverage) / Convert.ToDouble(channel.Subscribers), 2) +
                                    "\nPrice: " + channel.Price +
                                    "\nCpm: " + channel.Cpm;
                if (channel.Description != null && !channel.Description.Equals(""))
                {
                    info += "\n*Description*\n" + channel.Description;
                }

                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, info, replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, disableWebPagePreview: true);

                try
                {
                    await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                }
                catch (Exception ex)
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, ex.Message);
                }



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
