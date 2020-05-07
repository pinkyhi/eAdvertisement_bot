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
    public class AddNewAutoBuyCommand : Command
    {
        public override string Name => "/addNewAutobuy";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("anab");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                if (dbContext.Publications.Count(p => p.User_Id == update.CallbackQuery.From.Id) < 8)
                {
                    DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));

                    DbEntities.Autobuy newAb = new DbEntities.Autobuy { User_Id = user.User_Id, Balance = 0, Interval = 0, State = 0, Name = "Новый автозакуп", Min_Price = 0, Max_Cpm = 0, Max_Price = 0, Daily_Interval_From = new TimeSpan(0,0,0), Daily_Interval_To = new TimeSpan(24,0,0)  };
                    dbContext.Autobuys.Add(newAb);
                    dbContext.SaveChanges();

                    await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, null, false);

                    AutobuyMenuCommand x = new AutobuyMenuCommand();
                    await x.Execute(update, botClient);

                }
                else
                {
                    await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Лимит автозакупов – 8\nУдалите один чтобы добавить другой", true);
                    return;
                }

            }
            catch (Exception ex)
            {
                MainLogger.LogException(ex, addStr: "AddNewAutoBuy");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
