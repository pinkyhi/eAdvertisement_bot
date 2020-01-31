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
    public class CategoriesMenuCommand : Command
    {
        public override string Name => "/categoriesMenuPIISC";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("/categoriesMenuP");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            string tags = update.CallbackQuery.Data.Substring(16);    //0I100I200S1,2,3,4,5,6C1,2,3,4,5,6,7

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
            for (int si = 0; si < sStrs.Count; si++)
            {
                sIndexes.Add(Convert.ToInt32(sStrs[si]));
            }
            for (int ci = 0; ci < cStrs.Count; ci++)
            {
                cIndexes.Add(Convert.ToInt32(cStrs[ci]));
            }

            //keyboard[1][0] = new InlineKeyboardButton { Text = "OK", CallbackData = "/manualPurchaseMenuP0I" + intervalFrom + "I" + intervalTo + "S" + String.Join(',', sStrs.ToArray()) + "C" + String.Join(',', cStrs.ToArray()) };

            AppDbContext dbContext = new AppDbContext();
            try
            {
                List<Category> categories = dbContext.Categories.OrderBy(c=>c.Category_Id).ToList();
                bool toNextPageButton = false;
                bool toPreviousPageButton = false;

                if ((page + 1) * 7 < categories.Count)
                {
                    toNextPageButton = true;
                }
                if (page > 0)
                {
                    toPreviousPageButton = true;
                }
                categories = categories.Skip(7 * page).Take(7).ToList();
                categories.Remove(null);



                InlineKeyboardButton[][] keyboard;
                if (toNextPageButton || toPreviousPageButton)
                {
                    keyboard = new InlineKeyboardButton[categories.Count + 3][];
                }
                else
                {
                    keyboard = new InlineKeyboardButton[categories.Count + 2][];
                }
                keyboard[0] = new[] { new InlineKeyboardButton { Text = "CLEAR ALL", CallbackData = "/categoriesMenuP" + page + "I" + intervalFrom + "I" + intervalTo + "S" + String.Join(',', sStrs) + "C" } };

                int indexToPaste = 1;
                foreach (Category ca in categories)
                {
                    List<string> cIndexesForButton = new List<string>(cStrs);
                    string cIdStr = ca.Category_Id.ToString();

                    if (cIndexes.Contains(ca.Category_Id))
                    {
                        cIndexesForButton.Remove(cIdStr);
                        keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = ca.Name+ " ✔", CallbackData = "/categoriesMenuP"+page+"I"+intervalFrom+"I"+intervalTo+"S"+String.Join(',',sStrs)+"C"+String.Join(',',cIndexesForButton) }, };

                    }
                    else
                    {
                        cIndexesForButton.Add(cIdStr);
                        keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = ca.Name, CallbackData = "/categoriesMenuP" + page + "I" + intervalFrom + "I" + intervalTo + "S" + String.Join(',', sStrs) + "C" + String.Join(',', cIndexesForButton) }, };
                    }
                    indexToPaste++;
                }

                string extraTag = "I" + intervalFrom + "I" + intervalTo + "S" + String.Join(',', sStrs) + "C" + String.Join(',', cStrs);
                if (toNextPageButton && toPreviousPageButton)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "←←←", CallbackData = "/categoriesMenuP" + (page - 1) + extraTag }, new InlineKeyboardButton { Text = "→→→", CallbackData = "/manualPurchaseMenuP" + (page + 1) + extraTag }, };
                    indexToPaste++;
                }
                else if (toNextPageButton)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "→→→", CallbackData = "/categoriesMenuP" + (page + 1) + extraTag }, };
                    indexToPaste++;
                }
                else if (toPreviousPageButton)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "←←←", CallbackData = "/categoriesMenuP" + (page - 1) + extraTag }, };
                    indexToPaste++;
                }
                keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "OK", CallbackData = "/manualPurchaseMenuP0I"+intervalFrom+"I"+intervalTo+"S"+String.Join(',', sStrs)+"C"+ String.Join(',', cStrs) }, };

                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Choose wanted categories and press OK", replyMarkup: new InlineKeyboardMarkup(keyboard));
                try
                {
                    await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                }
                catch
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Impossible to delete old message");
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
