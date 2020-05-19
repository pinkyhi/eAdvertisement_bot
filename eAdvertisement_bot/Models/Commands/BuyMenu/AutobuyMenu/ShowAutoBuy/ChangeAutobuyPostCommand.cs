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
    public class ChangeAutobuyPostCommand : Command
    {
        public override string Name => "/changeAutobuyPostCommand";

        public override bool Contains(Update update) //cabpub
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.Equals("cabpub");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                user.User_State_Id = 0;
                dbContext.SaveChanges();
                List<DbEntities.Publication> posts = dbContext.Publications.Where(p => p.User_Id == update.CallbackQuery.From.Id).ToList();
                InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[posts.Count + 2][];

                keyboard[0] = new[]{
                    new InlineKeyboardButton { Text = "Меню управления постами", CallbackData = "/myPostsMenu" },
                };
                int indexToPaste = 1;
                while (indexToPaste-1 < posts.Count)
                {
                    keyboard[indexToPaste] = new[]
                    {
                        new InlineKeyboardButton{Text=posts[indexToPaste-1].Name, CallbackData="acabpubN"+posts[indexToPaste-1].Publication_Id}
                    };
                    indexToPaste++;
                }
                keyboard[indexToPaste] = new[]
                {
                    new InlineKeyboardButton{Text="Назад", CallbackData = "sabN"+user.Object_Id}
                };
                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, null, false);
                try
                {
                    await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                }
                catch
                {

                }
                finally
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Выберите пост", replyMarkup: new InlineKeyboardMarkup(keyboard));
                }

            }
            catch (Exception ex)
            {
                MainLogger.LogException(ex, addStr: "ChangeAutobuyPost");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
