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
    public class AddCategoryToAutobuyCategoriesCommand : Command
    {
        public override string Name => "/addCategoryToAutobuyCategoriesPC";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("acgtabcgsP");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            string page = update.CallbackQuery.Data.Substring(10, update.CallbackQuery.Data.IndexOf('C')-10);
            int cId = Convert.ToInt32(update.CallbackQuery.Data.Substring(update.CallbackQuery.Data.IndexOf('C') + 1));

            AppDbContext dbContext = new AppDbContext();

            // To use: sIndexes, cIndexes, intervalFrom, intervalTo, page
            try
            {

                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));

                string tags = new string(user.Tag);
                int indexOfC = tags.IndexOf('C');
                tags = tags.Substring(indexOfC + 1);


                List<string> cStrs = tags.Split(',').ToList();

                cStrs.Remove("");

                List<int> cIndexes = new List<int>(cStrs.Count);  // Indexes of categories

                for (int ci = 0; ci < cStrs.Count; ci++)
                {
                    cIndexes.Add(Convert.ToInt32(cStrs[ci]));
                }

                if (!cIndexes.Contains(cId))
                {
                    cIndexes.Add(cId);
                    cIndexes.OrderBy(cI => cI);
                }

                user.Tag = "C" + String.Join(',', cIndexes);
                dbContext.SaveChanges();

                update.CallbackQuery.Data = "ccgsfabP" + page;

                ChooseCategoriesForAutobuyCommand com = new ChooseCategoriesForAutobuyCommand();
                await com.Execute(update, botClient);

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
