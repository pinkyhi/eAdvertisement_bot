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
    public class On20XStateEvent : Command  
    {
        public override string Name => "/on20XStateEvent";

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
                    return Convert.ToString(user.User_State_Id).StartsWith("20") && (Convert.ToString(user.User_State_Id)).Length>2;
                }
                catch
                {
                    return false;
                }
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.Message.From.Id));
                long tag = user.User_State_Id;
                if (tag == 201)
                {
                    string timeStr="";
                    try
                    {
                        timeStr = update.Message.Text.Trim();
                    }
                    catch
                    {
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Некорректный формат времени. Попробуйте ещё раз.\n", replyMarkup: new InlineKeyboardMarkup(new[] { new[] { new InlineKeyboardButton { Text = "Назад", CallbackData = "/showChannelForSellerN" + user.Object_Id }, } }));
                        return;
                    }
                    if (timeStr.Length != 5)
                    {

                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Некорректный формат времени. Попробуйте ещё раз.\n", replyMarkup: new InlineKeyboardMarkup(new[] { new[] { new InlineKeyboardButton { Text = "Назад", CallbackData = "/showChannelForSellerN" + user.Object_Id }, } }));
                        return;
                    }
                    else
                    {
                        if (dbContext.Places.Count(p => p.Channel_Id == user.User_State_Id) < 5)
                        {
                            TimeSpan ts;
                            try
                            {
                                ts = TimeSpan.Parse(timeStr);
                            }
                            catch
                            {
                                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Некорректный формат времени. Попробуйте ещё раз.\n", replyMarkup: new InlineKeyboardMarkup(new[] { new[] { new InlineKeyboardButton { Text = "Назад", CallbackData = "/showChannelForSellerN" + user.Object_Id }, } }));
                                return;
                            }
                            
                            dbContext.Places.Add(new DbEntities.Place { Channel_Id = user.Object_Id, Time = ts });
                            dbContext.SaveChanges();
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Место добавленно успешно :)\n", replyMarkup: new InlineKeyboardMarkup(new[] { new[] { new InlineKeyboardButton { Text = "Показать обновленное меню", CallbackData = "/showChannelForSellerN" + user.Object_Id }, } }));
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Количество мест должно быть менее четырех.", replyMarkup: new InlineKeyboardMarkup(new[] { new[] { new InlineKeyboardButton { Text = "Назад", CallbackData = "/showChannelForSellerN" + user.Object_Id }, } }));

                        }

                    }
                }
                
                else if(tag==202)
                {
                    int amount;
                    try
                    {
                        amount = Convert.ToInt32(update.Message.Text.Trim());
                    }
                    catch
                    {
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Некорректный формат CPM.\nИли вернитесь назад нажав на кнопку ниже", replyMarkup: new InlineKeyboardMarkup(new[] { new[] { new InlineKeyboardButton { Text = "Назад", CallbackData = "/showChannelForSellerN" + user.Object_Id }, } }));
                        return;
                    }
                    if (amount < 10000)
                    {
                        DbEntities.Channel channel = dbContext.Channels.Find(user.Object_Id);
                        channel.Cpm = amount;
                        channel.Price = Convert.ToInt32(channel.Coverage * channel.Cpm / 1000);
                        dbContext.SaveChanges();
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "CPM изменено успешно :)\n", replyMarkup: new InlineKeyboardMarkup(new[] { new[] { new InlineKeyboardButton { Text = "Показать обновленное меню", CallbackData = "/showChannelForSellerN" + user.Object_Id }, } }));
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Это слишком много, бро:C Поробуй снова.\nЧтобы вернуться назад нажмите на кнопку ниже", replyMarkup: new InlineKeyboardMarkup(new[] { new[] { new InlineKeyboardButton { Text = "Назад", CallbackData = "/showChannelForSellerN" + user.Object_Id }, } }));

                    }
                }
                else if (tag == 203)
                {
                    string newDescription = "";
                    try
                    {
                        newDescription = update.Message.Text.Trim();
                    }
                    catch
                    {
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Неправильный формат описания. Поробуйте снова.\n", replyMarkup: new InlineKeyboardMarkup(new[] { new[] { new InlineKeyboardButton { Text = "Назад", CallbackData = "/showChannelForSellerN" + user.Object_Id }, } }));
                        return;
                    }
                    if (newDescription.Length == 0)
                    {
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Описание не может быть пустым.\n", replyMarkup: new InlineKeyboardMarkup(new[] { new[] { new InlineKeyboardButton { Text = "Назад", CallbackData = "/showChannelForSellerN" + user.Object_Id }, } }));
                        return;
                    }
                    else if (newDescription.Length > 1024)
                    {
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Описание должно быть менее 1024 символов\nВаше: " + newDescription.Length+ " Попробуйте снова.\n", replyMarkup: new InlineKeyboardMarkup(new[] { new[] { new InlineKeyboardButton { Text = "Назад", CallbackData = "/showChannelForSellerN" + user.Object_Id }, } }));
                        return;
                    }
                    dbContext.Channels.Find(user.Object_Id).Description = newDescription;
                    dbContext.SaveChanges();
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Описание изменено успено :)\n", replyMarkup: new InlineKeyboardMarkup(new[] { new[] { new InlineKeyboardButton { Text = "Показать обновленное меню", CallbackData = "/showChannelForSellerN" + user.Object_Id }, } }));

                }
                user.User_State_Id = 0;
            }
            catch(Exception ex)
            {
                MainLogger.LogException(ex, "On20XStateEvent");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
