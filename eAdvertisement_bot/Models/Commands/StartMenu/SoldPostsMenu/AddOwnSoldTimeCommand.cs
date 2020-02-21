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
    public class AddOwnSoldTimeCommand : Command
    {
        public override string Name => "/addOwnSoldTimeN";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("aostN");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            long channelId = Convert.ToInt64(update.CallbackQuery.Data.Substring(5));
            AppDbContext dbContext = new AppDbContext();
            try
            {
                

                dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id)).User_State_Id = 901;
                dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id)).Object_Id = channelId;
                dbContext.SaveChanges();

                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Отправьте дату, время вашей продажи и сколько часов топ в формате:\n dd-mm-yy hh:mm hh\n\n22-06-01 13:31 4"); 
            }
            catch (Exception ex) { await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, ex.Message); }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
