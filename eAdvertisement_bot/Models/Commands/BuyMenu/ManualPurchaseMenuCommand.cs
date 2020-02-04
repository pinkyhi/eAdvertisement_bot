using eAdvertisement_bot.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace eAdvertisement_bot.Models.Commands.ManualPurchase
{
    public class ManualPurchaseMenuCommand : Command
    {
        public override string Name => "/manualPurchaseMenuPIISC";    // Page, IntervalFrom, IntervalTo, Sorts, Categories

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("/manualPurchaseMenuP");
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            string tags = update.CallbackQuery.Data.Substring(20);    //0I100I200S1,2,3,4,5,6C1,2,3,4,5,6,7
            string tagsC = new string(tags);
            int indexOfI1 = tags.IndexOf('I');
            int page = Convert.ToInt32(tags.Substring(0, indexOfI1));
            tags = tags.Substring(indexOfI1 + 1);

            int indexOfI2 = tags.IndexOf('I');
            int intervalFrom = 0;
            if (indexOfI2 != 0)
            {
                intervalFrom = Convert.ToInt32(tags.Substring(0, indexOfI2));
            }
            tags = tags.Substring(indexOfI2 + 1);

            int indexOfS = tags.IndexOf('S');
            int intervalTo = 0;
            if (indexOfS != 0)
            {
                intervalTo = Convert.ToInt32(tags.Substring(0, indexOfS));
            }
            tags = tags.Substring(indexOfS + 1);

            int indexOfC = tags.IndexOf('C');
            List<string> sStrs = tags.Substring(0, indexOfC).Split(',').ToList();
            tags = tags.Substring(indexOfC + 1);

            List<string> cStrs = tags.Split(',').ToList();

            cStrs.Remove("");
            sStrs.Remove("");

            List<int> sIndexes = new List<int>(sStrs.Count);  // Indexes of sorts
            List<int> cIndexes = new List<int>(cStrs.Count);  // Indexes of categories
            for(int si=0;si<sStrs.Count; si++)
            {
                
                sIndexes.Add(Convert.ToInt32(sStrs[si]));
            }
            for(int ci=0;ci<cStrs.Count; ci++)
            {
                cIndexes.Add(Convert.ToInt32(cStrs[ci]));
            }

            AppDbContext dbContext = new AppDbContext();
            if (intervalFrom < 1)
            {
                intervalFrom=1;
            }
            if (intervalTo >10000 || intervalTo<1)
            {
                intervalTo = 10000;
            }
            // To use: sIndexes, cIndexes, intervalFrom, intervalTo, page
            try
            {

                List<DbEntities.Channel_Category> channelsCategoriesToShow = dbContext.Channel_Categories.Where(cc=>cIndexes.Contains(cc.Category_Id)).ToList();
                List<DbEntities.Channel> channels = dbContext.Channels.Where(c=>c.Price>=intervalFrom && c.Price<=intervalTo).ToList();

                List<int> categoriesToShow = channelsCategoriesToShow.Select(cc => cc.Category_Id).Distinct().ToList();
                //List<string> categoriesStrs = dbContext.Categories.Where(c => categoriesToShow.Contains(c.Category_Id)).Select(c => c.Name).ToList(); OLD VERSION
                List<string> categoriesStrs = (dbContext.Categories.Where(cc => cIndexes.Contains(cc.Category_Id))).Select(c=>c.Name).ToList();

                if (cIndexes.Count != 0)
                {
                    channels = channels.Where(c => c.Channel_Categories!=null && c.Channel_Categories.Any(cc => categoriesToShow.Contains(cc.Category_Id))).ToList();
                }

                
                // Sorts part => 1 is by cpm; 2 is by price; 3 is by cpm desc; 4 is by price desc;
                if (sIndexes.Count > 0)
                {

                    if (sIndexes.Contains(2))
                    {
                        channels = channels.OrderBy(c => c.Price).ToList();
                    }
                    if (sIndexes.Contains(4))
                    {
                        channels = channels.OrderByDescending(c => c.Price).ToList();
                    }
                    if (sIndexes.Contains(1))
                    {
                        channels = channels.OrderBy(c => c.Cpm).ToList();
                    }
                    if (sIndexes.Contains(3))
                    {
                        channels = channels.OrderByDescending(c => c.Cpm).ToList();
                    }

                }

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
                keyboard[0] = new[] { new InlineKeyboardButton { Text = "Categories", CallbackData = "/categoriesMenuP0" + update.CallbackQuery.Data.Substring(update.CallbackQuery.Data.IndexOf('I')) }, new InlineKeyboardButton { Text = "Sorts", CallbackData = "/sortsMenu" + update.CallbackQuery.Data.Substring(19) } };

                int indexToPaste = 1;
                foreach (DbEntities.Channel ch in channels)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "CPM: "+ch.Cpm+" Price: "+ch.Price +" \n"+ ch.Name, CallbackData = "/showChannelForBuyerN" + ch.Channel_Id+"T"+tagsC }, };
                    indexToPaste++;
                }

                string extraTag = update.CallbackQuery.Data.Substring(20 + indexOfI1);
                if (toNextPageButton && toPreviousPageButton)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "←←←", CallbackData = "/manualPurchaseMenuP" + (page - 1) + extraTag }, new InlineKeyboardButton { Text = "→→→", CallbackData = "/manualPurchaseMenuP" + (page + 1) + extraTag }, };
                    indexToPaste++;
                }
                else if (toNextPageButton)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "→→→", CallbackData = "/manualPurchaseMenuP" + (page + 1) + extraTag }, };
                    indexToPaste++;
                }
                else if (toPreviousPageButton)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "←←←", CallbackData = "/manualPurchaseMenuP" + (page - 1) + extraTag }, };
                    indexToPaste++;
                }
                keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "Back", CallbackData = "/buyMenu" }, };

                dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id)).User_State_Id = 3;
                dbContext.SaveChanges();


                string text = "Here you can change channel to buy ad there.\nIf you want to buy in specific channel you can send a post from it\n*Show settings*\nCategories:" + String.Join(", ", categoriesStrs) + "\nPrice interval: " + intervalFrom + "-" + intervalTo;
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, text, replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

                try
                {
                    await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                }
                catch { }

                

                }
            catch(Exception ex)
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
