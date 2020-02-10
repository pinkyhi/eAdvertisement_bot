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
    public class On40XStateEvent : Command
    {
        public override string Name => "/on40XStateEvent";

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
                    return Convert.ToString(user.User_State_Id).StartsWith("40") && (Convert.ToString(user.User_State_Id)).Length > 2;
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
                if (state == 401)
                {
                    dbContext.Autobuys.Find(Convert.ToInt32(user.Object_Id)).Name = update.Message.Text;
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Name is changed!", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Show updated menu", CallbackData = "sabN" + user.Object_Id }));
                }
                else if(state == 402)
                {
                    int cpm = 0;
                    try
                    {
                        cpm = Convert.ToInt32(update.Message.Text.Trim());
                    }
                    catch
                    {
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Wrong format");
                        return;
                    }

                    dbContext.Autobuys.Find(Convert.ToInt32(user.Object_Id)).Max_Cpm = cpm;
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Max cpm is changed!", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Show updated menu", CallbackData = "sabN" + user.Object_Id }));
                }
                else if(state == 403)
                {
                    int iF, iT = 0;
                    try
                    {
                        iF = Convert.ToInt32(update.Message.Text.Trim().Substring(0, update.Message.Text.Trim().IndexOf('-')));
                        iT = Convert.ToInt32(update.Message.Text.Trim().Substring(update.Message.Text.Trim().IndexOf('-') + 1));
                    }
                    catch
                    {
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Wrong format");
                        return;
                    }
                    DbEntities.Autobuy ab = dbContext.Autobuys.Find(Convert.ToInt32(user.Object_Id));
                    if (iT < 0)
                    {
                        iT = 0;
                    }
                    if (iF < 0)
                    {
                        iF = 0;
                    }
                    if (iF > iT)
                    {
                        int t = iF;
                        iF = iT;
                        iT = t;
                    }

                    ab.Max_Price = iT;
                    ab.Min_Price = iF;

                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Price interval is changed!", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Show updated menu", CallbackData = "sabN" + user.Object_Id }));
                }
                else if (state == 404)
                {
                    int interval = 0;
                    try
                    {
                        interval = Convert.ToInt32(update.Message.Text.Trim());
                    }
                    catch
                    {
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Wrong format");
                        return;
                    }

                    dbContext.Autobuys.Find(Convert.ToInt32(user.Object_Id)).Interval = interval;
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Interval is changed!", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Show updated menu", CallbackData = "sabN" + user.Object_Id }));
                }
                else if (state == 405)
                {
                    int balance = 0;
                    try
                    {
                        balance = Convert.ToInt32(update.Message.Text.Trim());
                    }
                    catch
                    {
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Wrong format");
                        return;
                    }
                    if (user.Balance < balance)
                    {
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Not enough balance on your account");
                        return;
                    }
                    user.Balance -= balance;

                    dbContext.Autobuys.Find(Convert.ToInt32(user.Object_Id)).Balance+=balance;
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Balance is added!", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Show updated menu", CallbackData = "sabN" + user.Object_Id }));
                }
                else if (state == 406)
                {
                    int iF, iT = 0;
                    try
                    {
                        iF = Convert.ToInt32(update.Message.Text.Trim().Substring(0, update.Message.Text.Trim().IndexOf('-')));
                        iT = Convert.ToInt32(update.Message.Text.Trim().Substring(update.Message.Text.Trim().IndexOf('-') + 1));
                    }
                    catch
                    {
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Wrong format");
                        return;
                    }
                    DbEntities.Autobuy ab = dbContext.Autobuys.Find(Convert.ToInt32(user.Object_Id));
                    if (iT < 0)
                    {
                        iT = 0;
                    }
                    if (iF < 0)
                    {
                        iF = 0;
                    }
                    if (iF > iT)
                    {
                        int t = iF;
                        iF = iT;
                        iT = t;
                    }

                    ab.Daily_Interval_To = new TimeSpan(iT,0,0);
                    ab.Daily_Interval_From = new TimeSpan(iF, 0, 0);

                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Daily interval is changed!", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Show updated menu", CallbackData = "sabN" + user.Object_Id }));
                }

                user.User_State_Id = 0;
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, ex.Message);
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
