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
    public class CheckBalanceManagerCommand : Command
    {
        public override string Name => throw new NotImplementedException();

        public override bool Contains(Update update)
        {
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message && update.Message.From.Id == MainLogger.ManagerId && update.Message.Text.StartsWith("/cb"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                string[] message = update.Message.Text.Split(' ');
                if (message.Length == 3)
                {
                    if (message[2].Equals(MainLogger.ManagerPass))
                    {
                        int uid = 0;
                        if(Int32.TryParse(message[1], out uid))
                        {
                            DbEntities.User user = dbContext.Users.FirstOrDefault(u => u.User_Id == uid);
                            if (user !=null)
                            {
                                await botClient.SendTextMessageAsync(update.Message.From.Id, $"Balance of user {uid} = {user.Balance}");
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(update.Message.From.Id, $"No such user {uid} in DB");
                            }
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(update.Message.From.Id, $"Wrong id value");
                        }
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(update.Message.From.Id, $"Wrong password");
                    }
                }
            }
            catch(Exception ex)
            {
                await botClient.SendTextMessageAsync(update.Message.From.Id, $"Some troubles with it");
            }

        }
    }
}
