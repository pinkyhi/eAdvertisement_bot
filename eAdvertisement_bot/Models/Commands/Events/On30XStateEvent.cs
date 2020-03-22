using eAdvertisement_bot.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace eAdvertisement_bot.Models.Commands
{
    public class On30XStateEvent : Command
    {
        public override string Name => "/on30XStateEvent";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                return false;
            }
            else
            {
                AppDbContext dbContext = new AppDbContext();
                try
                {
                    DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.Message.From.Id));
                    return Convert.ToString(user.User_State_Id).StartsWith("30") && (Convert.ToString(user.User_State_Id)).Length > 2;
                }
                catch
                {
                    return false;
                }
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.Message.From.Id));
                long state = user.User_State_Id;
                if (state == 301)
                {
                    string tags = user.Tag.Substring(1);

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
                    for (int si = 0; si < sStrs.Count; si++)
                    {

                        sIndexes.Add(Convert.ToInt32(sStrs[si]));
                    }
                    for (int ci = 0; ci < cStrs.Count; ci++)
                    {
                        cIndexes.Add(Convert.ToInt32(cStrs[ci]));
                    }

                    try
                    {
                        string[] niStrs = update.Message.Text.Trim().Split('-');
                        intervalFrom = Convert.ToInt32(niStrs[0]);
                        intervalTo = Convert.ToInt32(niStrs[1]);
                    }
                    catch
                    {
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Неправильный формат ввода");
                        return;
                    }

                                        

                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Изменение успешно", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Показать обновленное меню", CallbackData = "/manualPurchaseMenuP0I" + intervalFrom + "I" + intervalTo + "S" + String.Join(',', sStrs.ToArray()) + "C" + String.Join(',', cStrs.ToArray()) }));
                    user.Tag = null;


                }

                user.User_State_Id = 0;
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, ex.StackTrace + "\n" + ex.Message +"\n");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
