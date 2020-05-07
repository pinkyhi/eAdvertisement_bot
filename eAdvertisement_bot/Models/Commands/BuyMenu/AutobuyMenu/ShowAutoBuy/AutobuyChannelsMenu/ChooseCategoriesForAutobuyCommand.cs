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
    public class ChooseCategoriesForAutobuyCommand : Command
    {
        public override string Name => "/chooseCategoriesForAutobuyP";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("ccgsfabP");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            

            int page = Convert.ToInt32(update.CallbackQuery.Data.Substring(8));

           

            //keyboard[1][0] = new InlineKeyboardButton { Text = "OK", CallbackData = "/manualPurchaseMenuP0I" + intervalFrom + "I" + intervalTo + "S" + String.Join(',', sStrs.ToArray()) + "C" + String.Join(',', cStrs.ToArray()) };

            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                string tags = user.Tag;    //C1,2,3,4,5,6,7
                int indexOfC = tags.IndexOf('C');
                tags = tags.Substring(indexOfC + 1);

                List<string> cStrs = tags.Split(',').ToList();

                cStrs.Remove("");

                List<int> cIndexes = new List<int>(cStrs.Count);  // Indexes of categories
                for (int ci = 0; ci < cStrs.Count; ci++)
                {
                    cIndexes.Add(Convert.ToInt32(cStrs[ci]));
                }

                List<Category> categories = dbContext.Categories.OrderBy(c => c.Category_Id).ToList();
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
                keyboard[0] = new[] { new InlineKeyboardButton { Text = "ОЧИСТИТЬ", CallbackData = "clalcgsiabP" + page } };

                int indexToPaste = 1;
                foreach (Category ca in categories)
                {
                    List<string> cIndexesForButton = new List<string>(cStrs);
                    string cIdStr = ca.Category_Id.ToString();

                    if (cIndexes.Contains(ca.Category_Id))
                    {
                        cIndexesForButton.Remove(cIdStr);
                        keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = ca.Name + " ✔", CallbackData = "dcgfabcgsP" + page + "C"+cIdStr }, };

                    }
                    else
                    {
                        cIndexesForButton.Add(cIdStr);
                        keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = ca.Name, CallbackData = "acgtabcgsP" + page +"C" + cIdStr }, };
                    }
                    indexToPaste++;
                }

                if (toNextPageButton && toPreviousPageButton)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "←←←", CallbackData = "ccgsfabP" + (page - 1)  }, new InlineKeyboardButton { Text = "→→→", CallbackData = "ccgsfabP" + (page + 1)  }, };
                    indexToPaste++;
                }
                else if (toNextPageButton)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "→→→", CallbackData = "ccgsfabP" + (page + 1)  }, };
                    indexToPaste++;
                }
                else if (toPreviousPageButton)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "←←←", CallbackData = "ccgsfabP" + (page - 1)  }, };
                    indexToPaste++;
                }
                keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "OK", CallbackData = "acstabP0" }, };

                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Выберите категории и нажмите OK", replyMarkup: new InlineKeyboardMarkup(keyboard));
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
                MainLogger.LogException(ex, addStr: "ChooseCategoriesForAutobuy");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
