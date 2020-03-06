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
    public class UnattachChannelFromAutobuyCommand : Command
    {
        public override string Name => "unattachChannelFromAutobuyNP";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("unatclfabN");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            long channelId = Convert.ToInt64(update.CallbackQuery.Data.Substring(10, update.CallbackQuery.Data.IndexOf('P') - 10));
            string page = update.CallbackQuery.Data.Substring(update.CallbackQuery.Data.IndexOf('P') + 1);

            AppDbContext dbContext = new AppDbContext();

            try
            {
                Channel channel;
                try
                {
                    channel = dbContext.Channels.Find(channelId);
                }
                catch
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.From.Id, "Это канал не прикрелен к боту");
                    return;
                }
                if (channel.User_Id == update.CallbackQuery.From.Id)
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.From.Id, "Вы не можете добавить этот канал так как он ваш.");
                    return;
                }
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));

                try
                {
                    List<DbEntities.Autobuy_Channel> abcs = dbContext.Autobuy_Channels.Where(a => a.Autobuy_Id == Convert.ToInt32(user.Object_Id)).ToList();
                    if (abcs.Select(ac => ac.Channel_Id).Contains(channelId))
                    {

                            dbContext.Autobuys.Find(Convert.ToInt32(user.Object_Id)).Autobuy_Channels.Remove(dbContext.Autobuy_Channels.Where(ac=>ac.Channel_Id==channelId&&ac.Autobuy_Id==user.Object_Id).FirstOrDefault());


                        dbContext.SaveChanges();
                        await botClient.SendTextMessageAsync(update.CallbackQuery.From.Id, "Открепление успешно", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Показать обновленное меню!", CallbackData = "cabcsP" + page }));
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
                        await botClient.SendTextMessageAsync(update.CallbackQuery.From.Id, "Этот канал не прикреплен", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Назад", CallbackData = "cabcsP" + page}));

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
