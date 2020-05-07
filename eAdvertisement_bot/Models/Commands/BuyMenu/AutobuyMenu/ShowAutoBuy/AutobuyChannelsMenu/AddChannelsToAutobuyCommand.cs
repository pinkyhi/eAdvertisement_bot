using eAdvertisement_bot.DAO;
using eAdvertisement_bot.Logger;
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
    public class AddChannelsToAutobuyCommand : Command
    {
        public override string Name => "/addChannelsToAutobuyP";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("acstabP");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {



            AppDbContext dbContext = new AppDbContext();

            // To use: sIndexes, cIndexes, intervalFrom, intervalTo, page
            try
            {

                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                Autobuy autobuy = dbContext.Autobuys.Find(Convert.ToInt32(user.Object_Id));
                List<Autobuy_Channel> autobuyChannels = dbContext.Autobuy_Channels.Where(ac => ac.Autobuy_Id == autobuy.Autobuy_Id).ToList();
                //
                //
                //

                string tags = user.Tag;    //0I100I200S1,2,3,4,5,6C1,2,3,4,5,6,7
                string tagsC = new string(tags);
                
                int page = Convert.ToInt32(update.CallbackQuery.Data.Substring(7));

                int indexOfC = tags.IndexOf('C');
                tags = tags.Substring(indexOfC + 1);


                List<string> cStrs = tags.Split(',').ToList();

                cStrs.Remove("");

                List<int> cIndexes = new List<int>(cStrs.Count);  // Indexes of categories

                for (int ci = 0; ci < cStrs.Count; ci++)
                {
                    cIndexes.Add(Convert.ToInt32(cStrs[ci]));
                }
                int intervalFrom = autobuy.Min_Price;
                int intervalTo = autobuy.Max_Price;
                int maxCpm = autobuy.Max_Cpm;
                if (intervalFrom < 1)
                {
                    intervalFrom = 1;
                }
                if (intervalTo > 100000 || intervalTo < 1)
                {
                    intervalTo = 100000;
                }
                if (maxCpm < 1)
                {
                    maxCpm = 1;
                }

                //
                //
                //

                var t = autobuyChannels.Select(ac => ac.Channel_Id);

                //List<Channel> channelsInAutoBuy = dbContext.Channels.Where(c => t.Contains(c.Channel_Id)).OrderBy(c => c.Channel_Id).ToList();
                List<DbEntities.Channel> channels = dbContext.Channels.Where(c => c.Price >= intervalFrom && c.Price <= intervalTo && c.Cpm != null && c.Cpm<maxCpm && c.Coverage > 1500 && c.Places != null && c.Places.Count > 0).ToList();


                List<DbEntities.Channel_Category> channelsCategoriesToShow = dbContext.Channel_Categories.Where(cc => cIndexes.Contains(cc.Category_Id)).ToList();
                List<int> categoriesToShow = channelsCategoriesToShow.Select(cc => cc.Category_Id).Distinct().ToList();
                List<string> categoriesStrs = (dbContext.Categories.Where(cc => cIndexes.Contains(cc.Category_Id))).Select(c => c.Name).ToList();

                if (cIndexes.Count != 0)
                {
                    channels = channels.Where(c => c.Channel_Categories != null && c.Channel_Categories.Any(cc => categoriesToShow.Contains(cc.Category_Id))).ToList();
                }


                // Sorts part => 1 is by cpm; 2 is by price; 3 is by cpm desc; 4 is by price desc;

                channels = channels.OrderBy(c => c.Price).ToList();
                    
                channels = channels.OrderBy(c => c.Cpm).ToList();


                

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
                keyboard[0] = new[] { new InlineKeyboardButton { Text = "Категории", CallbackData = "ccgsfabP0"} };

                int indexToPaste = 1;
                foreach (DbEntities.Channel ch in channels)
                {
                    if (t!= null && t.Contains(ch.Channel_Id))
                    {
                        keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "CPM: " + ch.Cpm + " Цена: " + ch.Price + " \nX" + ch.Name+"X", CallbackData = "tciatabBlock"}, };
                    }
                    else
                    {
                        keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "CPM: " + ch.Cpm + " Цена: " + ch.Price + " \n" + ch.Name, CallbackData = "aacltabN" + ch.Channel_Id + "P" + page }, };
                    }
                    indexToPaste++;
                }
                
                if (toNextPageButton && toPreviousPageButton)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "←←←", CallbackData = "acstabP" + (page - 1) }, new InlineKeyboardButton { Text = "→→→", CallbackData = "acstabP" + (page + 1)  }, };
                    indexToPaste++;
                }
                else if (toNextPageButton)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "→→→", CallbackData = "acstabP" + (page + 1) }, };
                    indexToPaste++;
                }
                else if (toPreviousPageButton)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "←←←", CallbackData = "acstabP" + (page - 1)  }, };
                    indexToPaste++;
                }
                keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "Назад", CallbackData = "cabcsP0" }, };

                dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id)).User_State_Id = 5;
                dbContext.SaveChanges();


                string text = "Здесь вы можете добавить каналы к автозакупу.\nЕсли вы хотите добавить определенный канал перешлите пост оттуда\n*Показать настройки*\nКатегории:" + String.Join(", ", categoriesStrs) + "\nИнтервал цены: " + intervalFrom + "-" + intervalTo+"\nМаксимальный CPM: "+maxCpm;
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, text, replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

                try
                {
                    await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                }
                catch
                {

                }



            }
            catch (Exception ex)
            {
                MainLogger.LogException(ex, addStr: "AddChannelsToAutobuy");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
