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
    public class ClearAllCategoriesInAutobuyCommand : Command
    {
        public override string Name => "/clearAllCategoriesInAutobuyP"; 

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("clalcgsiabP");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            string page = update.CallbackQuery.Data.Substring(11);
            AppDbContext dbContext = new AppDbContext();

            // To use: sIndexes, cIndexes, intervalFrom, intervalTo, page
            try
            {

                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));

                user.Tag = "C";
                dbContext.SaveChanges();

                update.CallbackQuery.Data = "ccgsfabP" + page;

                ChooseCategoriesForAutobuyCommand com = new ChooseCategoriesForAutobuyCommand();
                await com.Execute(update, botClient);

            }
            catch (Exception ex)
            {
                MainLogger.LogException(ex, addStr: "ClearAllCategoriesInAutobuy");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
