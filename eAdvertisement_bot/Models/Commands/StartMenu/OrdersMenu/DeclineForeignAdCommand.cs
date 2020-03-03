using eAdvertisement_bot.DAO;
using eAdvertisement_bot.Models.DbEntities;
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
    public class DeclineForeignAdCommand : Command
    {
        public override string Name => "declineForeignAdN";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("dfaN");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            int adId = Convert.ToInt32(update.CallbackQuery.Data.Substring(update.CallbackQuery.Data.IndexOf('N') + 1));
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User userEntity = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                Advertisement ad = dbContext.Advertisements.Find(adId);
                //userEntity.Balance += ad.Price;
                //ad.Price = 0;
                ad.Advertisement_Status_Id = 3;
                dbContext.SaveChanges();

                update.CallbackQuery.Data = "/ordersMenuP0";

                OrdersMenuCommand omc = new OrdersMenuCommand();
                await omc.Execute(update, botClient);


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
