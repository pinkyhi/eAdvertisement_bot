using eAdvertisement_bot.DAO;
using eAdvertisement_bot.Logger;
using eAdvertisement_bot.Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace eAdvertisement_bot.Models.Commands
{
    public class DeleteAutobuyCommand : Command
    {
        public override string Name => "deleteAutobuy";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("delab");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                dbContext.SaveChanges();

                Autobuy ab = dbContext.Autobuys.Find(Convert.ToInt32(user.Object_Id));
                int t = ab.Balance;
                ab.Balance = 0;
                user.Balance += t; 
                update.CallbackQuery.Data = "abm";
                user.Object_Id = 0;

                dbContext.Autobuys.Remove(ab);

                dbContext.SaveChanges();

                AutobuyMenuCommand c = new AutobuyMenuCommand();
                await c.Execute(update, botClient);


            }
            catch (Exception ex)
            {
                MainLogger.LogException(ex, addStr: "DeleteAutobuy");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
