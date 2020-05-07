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
    public class DeleteChannelPlaceCommand : Command
    {
        public override string Name => "/deletePlace";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("/deletePlaceN");
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                int placeId = Convert.ToInt32(update.CallbackQuery.Data.Substring(13));
                List<DbEntities.Place> places = dbContext.Places.Where(p => p.Place_Id == placeId).ToList();
                long channelId = places[0].Channel_Id;
                dbContext.Places.RemoveRange(dbContext.Places.Where(p => p.Place_Id == placeId).ToList());
                dbContext.SaveChanges();

                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Место удалено", true);  // ...,...,alert    AnswerCallbackQuery is required to send to avoid clock animation ob the button   
                ShowChannelForSellerCommand scfsc = new ShowChannelForSellerCommand();
                update.CallbackQuery.Data = "/showChannelForSellerN" + channelId;
                await scfsc.Execute(update, botClient);
                
            }
            catch (Exception ex)
            {
                MainLogger.LogException(ex, "DeleteChannelPlaceCommand");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
