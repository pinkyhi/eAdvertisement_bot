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
    public class OwnSoldMenuCommand : Command
    {
        public override string Name => "/ownSoldMenuP";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("osmP");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            int pageNow = Convert.ToInt32(update.CallbackQuery.Data.Substring(4));
            AppDbContext dbContext = new AppDbContext();
            try
            {
                List<DbEntities.Channel> channels = dbContext.Channels.Where(chssda => chssda.User_Id == update.CallbackQuery.Message.Chat.Id).ToList().OrderBy(c => c.Channel_Id).ToList();
                List<DbEntities.Channel> channelsToType = channels.Skip(7 * pageNow).Take(7).ToList();

                channelsToType.Remove(null);

                bool toNextPageButton = false;
                bool toPreviousPageButton = false;

                if ((pageNow + 1) * 7 < channels.Count)
                {
                    toNextPageButton = true;
                }
                if (pageNow > 0)
                {
                    toPreviousPageButton = true;
                }


                InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[0][];
                if (toNextPageButton || toPreviousPageButton)
                {
                    Array.Resize(ref keyboard, 2 + channelsToType.Count);
                }
                else
                {
                    Array.Resize(ref keyboard, 1 + channelsToType.Count);
                }

                int indexToPaste = 0;
                foreach (Channel ch in channelsToType)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = ch.Name, CallbackData = "aostN" + ch.Channel_Id }, };
                    indexToPaste++;
                }

                if (toNextPageButton && toPreviousPageButton)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "←←←", CallbackData = "osmP" + (pageNow - 1) }, new InlineKeyboardButton { Text = "→→→", CallbackData = "osmP" + (pageNow + 1) }, };
                    indexToPaste++;
                }
                else if (toNextPageButton)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "→→→", CallbackData = "osmP" + (pageNow + 1) }, };
                    indexToPaste++;
                }
                else if (toPreviousPageButton)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "←←←", CallbackData = "osmP" + (pageNow - 1) }, };
                    indexToPaste++;
                }

                keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "Назад", CallbackData = "/soldPostsMenu" }, };



                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Выберите канал", replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

                try
                {
                    await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                }
                catch (Exception ex)
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, ex.Message);
                }

            }
            catch (Exception ex) { await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, ex.Message); }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
