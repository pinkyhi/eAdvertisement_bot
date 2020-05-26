using eAdvertisement_bot.DAO;
using eAdvertisement_bot.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace eAdvertisement_bot.Models.Commands
{
    public class AddBalanceManagerCommand : Command
    {
        public override string Name => "/MANAGERab";

        public override bool Contains(Update update)
        {
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message && update.Message.From.Id == MainLogger.ManagerId && update.Message.Text.StartsWith("/ab"))
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
                if (message.Length == 4)
                {
                    if (message[3].Equals(MainLogger.ManagerPass))
                    {
                        int uid = 0;
                        int addSum = 0;
                        if (Int32.TryParse(message[1], out uid))
                        {
                            if (Int32.TryParse(message[2], out addSum))
                            {
                                DbEntities.User user = dbContext.Users.FirstOrDefault(u => u.User_Id == uid);
                                if (user != null)
                                {

                                    user.Balance += addSum;
                                    dbContext.SaveChanges();
                                    await botClient.SendTextMessageAsync(update.Message.From.Id, $"Success\nBalance of user {uid} is added on {addSum}\nNow balance is {user.Balance}");

                                }
                                else
                                {
                                    await botClient.SendTextMessageAsync(update.Message.From.Id, $"No such user {uid} in DB");
                                }
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(update.Message.From.Id, $"Wrong sum to decrease");
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
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(update.Message.From.Id, $"Some troubles with it");
            }
        }
    }
}
