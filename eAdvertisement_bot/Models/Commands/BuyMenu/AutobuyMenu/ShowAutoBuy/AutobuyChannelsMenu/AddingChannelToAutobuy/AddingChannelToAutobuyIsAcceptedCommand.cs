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
    public class AddingChannelToAutobuyIsAcceptedCommand:Command
    {
        public override string Name => "/addingChannelToAutobuyIsAcceptedNP";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("acltabiaN");
            }
        }



        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            long channelId = Convert.ToInt64(update.CallbackQuery.Data.Substring(9, update.CallbackQuery.Data.IndexOf('P') - 9));
            string page = update.CallbackQuery.Data.Substring(update.CallbackQuery.Data.IndexOf('P') + 1);

            AppDbContext dbContext = new AppDbContext();

            try
            {

                DbEntities.Channel channel;
                try
                {
                    channel = dbContext.Channels.Find(channelId);
                }
                catch
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.From.Id, "Этот канал не прикреплен к боту");
                    return;
                }
                if (channel.User_Id == update.CallbackQuery.From.Id)
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.From.Id, "Вы не можете добавить этот канал так как он ваш");
                    return;
                }
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));

                try
                {
                    List<DbEntities.Autobuy_Channel> abcs = dbContext.Autobuy_Channels.Where(a => a.Autobuy_Id == Convert.ToInt32(user.Object_Id)).ToList();
                    if (!abcs.Select(ac => ac.Channel_Id).Contains(channelId))
                    {
                        if (dbContext.Autobuys.Find(Convert.ToInt32(user.Object_Id)).Autobuy_Channels != null)
                        {
                            dbContext.Autobuys.Find(Convert.ToInt32(user.Object_Id)).Autobuy_Channels.Add(new DbEntities.Autobuy_Channel { Autobuy_Id = Convert.ToInt32(user.Object_Id), Channel_Id = channel.Channel_Id });
                        }
                        else
                        {
                            dbContext.Autobuys.Find(Convert.ToInt32(user.Object_Id)).Autobuy_Channels = new List<DbEntities.Autobuy_Channel> { new DbEntities.Autobuy_Channel { Autobuy_Id = Convert.ToInt32(user.Object_Id), Channel_Id = channel.Channel_Id } };
                        }
                        dbContext.SaveChanges();
                        await botClient.SendTextMessageAsync(update.CallbackQuery.From.Id, "Добавление успешно", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Показать обновленное меню!", CallbackData = "acstabP"+page }));
                        try
                        {
                            await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                        }
                        catch (Exception ex)
                        {
                            await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, ex.Message);
                        }

                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(update.CallbackQuery.From.Id, "Этот канал уже прикреплён", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Назад", CallbackData = "acstabP"+page }));

                    }
                }
                catch (Exception ex)
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.From.Id, ex.Message);

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
