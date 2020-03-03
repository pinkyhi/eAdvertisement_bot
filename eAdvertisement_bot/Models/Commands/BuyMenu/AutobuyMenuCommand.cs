using eAdvertisement_bot.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace eAdvertisement_bot.Models.Commands
{
    public class AutobuyMenuCommand : Command
    {
        public override string Name => "/autobuyMenu";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("abm");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id)).User_State_Id = 0;
                dbContext.SaveChanges();
                List<DbEntities.Autobuy> autobuys = dbContext.Autobuys.Where(a => a.User_Id == update.CallbackQuery.From.Id).OrderBy(a=>a.Autobuy_Id).ToList();
                InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[autobuys.Count + 2][];
                keyboard[0] = new[]
                {
                    new InlineKeyboardButton{Text = "Add new autobuy", CallbackData = "anab"}
                };

                int indexToPaste = 1;
                while (indexToPaste < autobuys.Count + 1)
                {
                    keyboard[indexToPaste] = new[]
                    {
                        new InlineKeyboardButton{Text=autobuys[indexToPaste-1].Name, CallbackData="sabN"+autobuys[indexToPaste-1].Autobuy_Id}
                    };
                    indexToPaste++;
                }
                keyboard[indexToPaste] = new[]
                {
                    new InlineKeyboardButton{Text="Back", CallbackData = "/buyMenu"}
                };
                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, null, false);
                try
                {
                    await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                }
                catch { }
                finally
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Here you can create autobuys and look them up", replyMarkup: new InlineKeyboardMarkup(keyboard));
                }

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
