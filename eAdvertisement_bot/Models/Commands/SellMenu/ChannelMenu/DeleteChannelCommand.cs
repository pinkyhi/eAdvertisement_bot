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
    public class DeleteChannelCommand : Command
    {
        public override string Name => "/deleteChannel";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("/deleteChannel");
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                user.User_State_Id = 204;

                dbContext.SaveChanges();

                InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[1][];
                keyboard[0] = new[] { new InlineKeyboardButton { Text = "УДАЛИТЬ КАНАЛ", CallbackData = "/acceptChannelDeletingN"+user.Object_Id }, new InlineKeyboardButton { Text = "Назад", CallbackData = "/showChannelForSellerN"+user.Object_Id } };


                try
                {
                    await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                }
                catch (Exception ex)
                {
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, ex.Message);
                }

                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Подтвердите удаление канала", true);
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Вы уверены что хотите удалить этот канал из бота?", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: new InlineKeyboardMarkup(keyboard));
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
