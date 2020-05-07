using eAdvertisement_bot.DAO;
using eAdvertisement_bot.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;


namespace eAdvertisement_bot.Models.Commands
{
    public class SortsMenuCommand : Command
    {
        public override string Name => "/sortsMenuPIISC";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("/sortsMenuP");
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            try
            {
                string tags = update.CallbackQuery.Data.Substring(11);    //0I100I200S1,2,3,4,5,6C1,2,3,4,5,6,7

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

                //string sortsStrPart = String.Join(',', sStrs.ToArray());
                // Sorts part => 1 is by cpm; 2 is by price; 3 is by cpm desc; 4 is by price desc;
                InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[2][];
                keyboard[0] = new InlineKeyboardButton[2];
                keyboard[1] = new InlineKeyboardButton[1];
                keyboard[1][0] = new InlineKeyboardButton { Text = "OK", CallbackData = "/manualPurchaseMenuP0I" + intervalFrom + "I" + intervalTo + "S" + String.Join(',', sStrs.ToArray()) + "C" + String.Join(',', cStrs.ToArray()) };

                List<string> sStrsCopy = new List<string>(sStrs);
                if (sIndexes.Contains(1))
                {
                    sStrs.Remove("1");
                    sStrs.Add("3");
                    keyboard[0][0] = new InlineKeyboardButton { Text = "По возрастанию CPM ✔", CallbackData = "/sortsMenuP0I" + intervalFrom + "I" + intervalTo + "S" + String.Join(',', sStrs.ToArray()) + "C" + String.Join(',', cStrs.ToArray()) };
                }
                else if (sIndexes.Contains(3))
                {
                    sStrs.Remove("3");
                    sStrs.Add("1");
                    keyboard[0][0] = new InlineKeyboardButton { Text = "По убыванию CPM ✔", CallbackData = "/sortsMenuP0I" + intervalFrom + "I" + intervalTo + "S" + String.Join(',', sStrs.ToArray()) + "C" + String.Join(',', cStrs.ToArray()) };

                }

                if (sIndexes.Contains(2))
                {
                    sStrsCopy.Remove("2");
                    sStrsCopy.Add("4");
                    keyboard[0][1] = new InlineKeyboardButton { Text = "По возрастанию цены ✔", CallbackData = "/sortsMenuP0I" + intervalFrom + "I" + intervalTo + "S" + String.Join(',', sStrsCopy.ToArray()) + "C" + String.Join(',', cStrs.ToArray()) };
                }
                else if (sIndexes.Contains(4))
                {
                    sStrsCopy.Remove("4");
                    sStrsCopy.Add("2");
                    keyboard[0][1] = new InlineKeyboardButton { Text = "По убыванию цены ✔", CallbackData = "/sortsMenuP0I" + intervalFrom + "I" + intervalTo + "S" + String.Join(',', sStrsCopy.ToArray()) + "C" + String.Join(',', cStrs.ToArray()) };
                }

                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Нажмите на кнопки ниже чтобы изменить параметры, если параметры вас устраивают – нажмите \"OK\"", replyMarkup: new InlineKeyboardMarkup(keyboard));
                try
                {
                    await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                }
                catch
                {
                }
            }
            catch(Exception ex)
            {
                MainLogger.LogException(ex, "SortsMenuCommand");
            }
        }
    }
}
