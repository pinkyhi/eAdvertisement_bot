using eAdvertisement_bot.DAO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace eAdvertisement_bot.Models.Commands
{
    public class ShowAutoBuyCommand : Command
    {
        public override string Name => "/showAutobuyN";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("sabN");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            int abId = Convert.ToInt32(update.CallbackQuery.Data.Substring(4));
            AppDbContext dbContext = new AppDbContext();
            try
            {
                List<DbEntities.Autobuy_Channel> autobuyChannels = dbContext.Autobuy_Channels.Where(ac => ac.Autobuy_Id == abId).ToList();
                List<DbEntities.Channel> channels = dbContext.Channels.Where(c=>autobuyChannels.Select(ac=> ac.Channel_Id).Contains(c.Channel_Id)).ToList();

                DbEntities.Autobuy autobuyToShow = dbContext.Autobuys.Find(abId);

                DbEntities.Publication post = autobuyToShow.Publication_Snapshot == null ? null : JsonSerializer.Deserialize<DbEntities.Publication>(autobuyToShow.Publication_Snapshot);

                string postName = post == null ? "*None*" : post.Name;

                string text = "*" + autobuyToShow.Name + "*" +
                    "\nBalance of autobuy: " + autobuyToShow.Balance +
                    "\nState: " + (autobuyToShow.State == 1 ? "On" : "Off") +
                    "\nInterval: " + autobuyToShow.Interval + (autobuyToShow.Interval > 1 ? "day" : "days") +
                    "\nPrice\n    Min: " + autobuyToShow.Min_Price + "\n    Max: " + autobuyToShow.Max_Price +
                    "\nMax CPM: " + autobuyToShow.Max_Cpm +
                    "\nPost: " + postName +
                    "\n*Channels*" +
                    "\n" + String.Join(',',channels.Select(c=>c.Name)) + ".";


                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                user.User_State_Id = 4;
                user.Object_Id = abId;
                dbContext.SaveChanges();

                InlineKeyboardButton[][] keyboardControll = new[]
                {
                    new[]
                    {
                        new InlineKeyboardButton{Text = "Change State", CallbackData = "cabst"},
                    },
                    new[]
                    {
                        new InlineKeyboardButton{Text = "Change Name", CallbackData = "cabn"},
                        new InlineKeyboardButton{Text = "Change Max CPM", CallbackData = "cabmac"}
                    },
                    new[]
                    {
                        new InlineKeyboardButton{Text = "Change Min-Max price", CallbackData = "cabmimap"},
                        new InlineKeyboardButton{Text = "Change buy interval", CallbackData = "cabbi"}
                    },
                    new[]
                    {
                        new InlineKeyboardButton{Text="Change post",CallbackData = "cabpub"}
                    },
                    new[]
                    {
                        new InlineKeyboardButton{Text="Change channels",CallbackData = "cabcs"}
                    },
                    new[]
                    {
                        new InlineKeyboardButton{Text="Back",CallbackData = "abm"}
                    }
                };

                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, text, replyMarkup: new InlineKeyboardMarkup(keyboardControll),parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

                await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);


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
