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
    public class ChangeAutobuyChannelsCommand : Command
    {
        public override string Name => "/changeAutobuyChannels";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("cabcsP");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {


            AppDbContext dbContext = new AppDbContext();
            // To use: sIndexes, cIndexes, intervalFrom, intervalTo, page
            try
            {
                int page = Convert.ToInt32(update.CallbackQuery.Data.Substring(6));
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                Autobuy autobuy = dbContext.Autobuys.Find(Convert.ToInt32(user.Object_Id));
                List<Autobuy_Channel> autobuyChannels = dbContext.Autobuy_Channels.Where(ac => ac.Autobuy_Id == autobuy.Autobuy_Id).ToList();
                var t = autobuyChannels.Select(ac => ac.Channel_Id);
                List<Channel> channels = dbContext.Channels.Where(c => t.Contains(c.Channel_Id)).OrderBy(c=>c.Channel_Id).ToList();




                bool toNextPageButton = false;
                bool toPreviousPageButton = false;

                if ((page + 1) * 7 < channels.Count)
                {
                    toNextPageButton = true;
                }
                if (page > 0)
                {
                    toPreviousPageButton = true;
                }
                channels = channels.Skip(7 * page).Take(7).ToList();
                channels.Remove(null);




                InlineKeyboardButton[][] keyboard;
                if (toNextPageButton || toPreviousPageButton)
                {
                    keyboard = new InlineKeyboardButton[channels.Count + 3][];
                }
                else
                {
                    keyboard = new InlineKeyboardButton[channels.Count + 2][];
                }
                keyboard[0] = new[] { new InlineKeyboardButton { Text = "Add channels", CallbackData = "acstabP0" }, };

                int indexToPaste = 1;
                foreach (DbEntities.Channel ch in channels)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "CPM: " + ch.Cpm + " Price: " + ch.Price + " \n" + ch.Name, CallbackData = "satabc" + ch.Channel_Id }, }; //Show attached to autobuy channel
                    indexToPaste++;
                }

                if (toNextPageButton && toPreviousPageButton)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "←←←", CallbackData = "cabcsP" + (page - 1)}, new InlineKeyboardButton { Text = "→→→", CallbackData = "cabcsP" + (page + 1) }, };
                    indexToPaste++;
                }
                else if (toNextPageButton)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "→→→", CallbackData = "cabcsP" + (page + 1)  }, };
                    indexToPaste++;
                }
                else if (toPreviousPageButton)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "←←←", CallbackData = "cabcsP" + (page - 1)  }, };
                    indexToPaste++;
                }
                keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "Back", CallbackData = "sabN"+user.Object_Id }, };

                user.Tag = "C";
                //user.User_State_Id = 5;
                dbContext.SaveChanges();


                string text = "Here you can see channels that are attached to this autobuy\n Click on channel will give info about it";
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, text, replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

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
