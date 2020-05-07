using eAdvertisement_bot.DAO;
using eAdvertisement_bot.Logger;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace eAdvertisement_bot.Models.Commands
{
    public class ChangingAutobuyPostIsAcceptedCommand : Command
    {
        public override string Name => "/changingAutobuyPostIsAccepted";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("cabpubiaN");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                DbEntities.Autobuy autobuy = dbContext.Autobuys.Find(Convert.ToInt32(user.Object_Id));

                autobuy.Publication_Snapshot = JsonSerializer.Serialize(dbContext.Publications.Include("Medias").Include("Buttons").FirstOrDefault(p=>p.Publication_Id==Convert.ToInt32(update.CallbackQuery.Data.Substring(9))));
                dbContext.SaveChanges();

                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Пост изменен!", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Показать обновленное меню", CallbackData = "sabN" + user.Object_Id }));
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
                MainLogger.LogException(ex, addStr: "ChangingAutobuyPostIsAccepted");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
