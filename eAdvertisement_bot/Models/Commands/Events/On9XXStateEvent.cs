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
    public class On9XXStateEvent : Command
    {
        public override string Name =>"/on9XXStateEvent";

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
                    return Convert.ToString(user.User_State_Id).StartsWith("9") && (Convert.ToString(user.User_State_Id)).Length>2;
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
                long channelId = user.Object_Id;
                long tag = user.User_State_Id;
                //22-06-01 13:31 3
                if (tag == 901 && dbContext.Channels.Find(channelId)!=null)
                {
                    DateTime dateTime = DateTime.Parse(update.Message.Text.Substring(0, update.Message.Text.LastIndexOf(':')+3).Trim());
                    int topHours = Convert.ToInt32(update.Message.Text.Substring(update.Message.Text.LastIndexOf(':') + 3));

                    List<Advertisement> ads = dbContext.Advertisements.Where(a => a.Channel_Id == channelId && a.Date_Time < dateTime).ToList();
                    Advertisement nearestAd = ads.Where(a => a.Date_Time == ads.Max(a => a.Date_Time)).FirstOrDefault();
                    if(dateTime < DateTime.Now)
                    {
                        InlineKeyboardButton[][] keyboard = new[] { new[] { new InlineKeyboardButton { Text = "Назад в меню проданых реклам", CallbackData = "/soldPostsMenu" }, } };
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Реклама не была добавлена так как дата/время добавление меньше текущих.", replyMarkup: new InlineKeyboardMarkup(keyboard));
                        return;
                    }
                    else if(DateTime.Now.Subtract(dateTime).TotalDays > 365)
                    {
                        InlineKeyboardButton[][] keyboard = new[] { new[] { new InlineKeyboardButton { Text = "Назад в меню проданых реклам", CallbackData = "/soldPostsMenu" }, } };
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Реклама не была добавлена так как дата/время добавление будет более чем через год.", replyMarkup: new InlineKeyboardMarkup(keyboard));
                        return;
                    }
                    if (ads.Count==0 || nearestAd == null || dateTime.Subtract(nearestAd.Date_Time).TotalHours >= nearestAd.Top)
                    {
                        Advertisement nearestTopAd= null ;
                        if (ads.Count != 0)
                        {
                            nearestTopAd = dbContext.Advertisements.Where(a=>a.Date_Time.Date == dateTime.Date).Where(a => a.Channel_Id == channelId && a.Date_Time > dateTime).Where(a => a.Date_Time == ads.Min(a => a.Date_Time)).FirstOrDefault();
                        }

                        if (nearestTopAd== null || nearestTopAd.Date_Time.Subtract(dateTime) > new TimeSpan(topHours,0,0))
                        {
                            dbContext.Advertisements.Add(new Advertisement { Is_Opened = true, Channel_Id = channelId, Date_Time = dateTime, Advertisement_Status_Id = 9, Price = 0, Top = topHours, Moment_Cpm = 0 }); ;
                            dbContext.SaveChanges();
                            InlineKeyboardButton[][] keyboard = new[] { new[] { new InlineKeyboardButton { Text = "Назад в меню проданых реклам", CallbackData = "/soldPostsMenu" }, } };
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Реклама добавлена успешно", replyMarkup: new InlineKeyboardMarkup(keyboard));
                        }
                        else
                        {
                            InlineKeyboardButton[][] keyboard = new[] { new[] { new InlineKeyboardButton { Text = "Назад в меню проданых реклам", CallbackData = "/soldPostsMenu" }, } };
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Реклама не была добавлена так как вы имеете продажу время постинга которой входит в диапазон постинга вашей рекламы. Попробуйте ещё раз или вернитесь назад.", replyMarkup: new InlineKeyboardMarkup(keyboard));
                            return;
                        }

                        
                    }
                    else if(nearestAd!= null && dateTime.Subtract(nearestAd.Date_Time).Hours < nearestAd.Top)
                    {
                        InlineKeyboardButton[][] keyboard = new[] { new[] { new InlineKeyboardButton { Text = "Назад в меню проданых реклам", CallbackData = "/soldPostsMenu" }, } };
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Реклама не была добавлена так как вы имеете продажу время+время топа которой больше чем время которе вы определили сейчас. Попробуйте ещё раз или вернитесь назад.", replyMarkup: new InlineKeyboardMarkup(keyboard));
                        return;
                    }

                }
                user.User_State_Id = 0;
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                MainLogger.LogException(ex, "On9XXStateEvent");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
