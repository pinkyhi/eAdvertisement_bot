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
    public class DecreaseBalanceManagerCommand : Command
    {
        public override string Name => "/MANAGERdb";

        public override bool Contains(Update update)
        {
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message && update.Message.From.Id == MainLogger.ManagerId && update.Message.Text.StartsWith("/db"))
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
                        int decSum = 0;
                        if (Int32.TryParse(message[1], out uid))
                        {
                            if(Int32.TryParse(message[2], out decSum))
                            {
                                DbEntities.User user = dbContext.Users.FirstOrDefault(u => u.User_Id == uid);
                                if (user != null)
                                {
                                    if (user.Balance >= decSum)
                                    {
                                        user.Balance -= decSum;
                                        dbContext.SaveChanges();
                                        await botClient.SendTextMessageAsync(update.Message.From.Id, $"Success\nBalance of user {uid} is decreased on {decSum}\nNow balance is {user.Balance}");

                                    }
                                    else
                                    {
                                        await botClient.SendTextMessageAsync(update.Message.From.Id, $"Balance of user {uid} is lower than {decSum}\nBalance is {user.Balance}");
                                    }
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
