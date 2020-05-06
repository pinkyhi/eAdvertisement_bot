using eAdvertisement_bot.DAO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace eAdvertisement_bot.Models.Commands
{
    public class SellMenuCommand : Command
    {
        public override string Name => "/sellMenu";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("/sellMenuP");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            int pageNow = Convert.ToInt32(update.CallbackQuery.Data.Substring(10));
            AppDbContext dbContext = new AppDbContext();
            try
            {
                List<DbEntities.Channel> channels = dbContext.Channels.Include("Places").Where(chssda => chssda.User_Id == update.CallbackQuery.Message.Chat.Id).ToList().OrderBy(c=>c.Channel_Id).ToList();
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


                InlineKeyboardButton[][] keyboard = new[]
                {
                        new[] //first row
                        {
                            new InlineKeyboardButton { Text = "Добавить канал", CallbackData = "/addChannel" },
                        }
                };

                if (toNextPageButton || toPreviousPageButton)
                {
                    Array.Resize(ref keyboard, 3 + channelsToType.Count);
                }
                else
                {
                    Array.Resize(ref keyboard, 2 + channelsToType.Count);
                }

                int indexToPaste = 1;
                foreach (DbEntities.Channel ch in channelsToType)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = (ch.Cpm!=null&&ch.Cpm>0&&ch.Coverage>1500&&ch.Places!=null&&ch.Places.Count>0) ? "✔️" + ch.Name:"❌" + ch.Name, CallbackData = "/showChannelForSellerN" + ch.Channel_Id }, };
                    indexToPaste++;
                }

                if (toNextPageButton && toPreviousPageButton)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "←←←", CallbackData = "/sellMenuP" + (pageNow - 1) }, new InlineKeyboardButton { Text = "→→→", CallbackData = "/sellMenuP" + (pageNow + 1) }, };
                    indexToPaste++;
                }
                else if (toNextPageButton)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "→→→", CallbackData = "/sellMenuP" + (pageNow + 1) }, };
                    indexToPaste++;
                }
                else if (toPreviousPageButton)
                {
                    keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "←←←", CallbackData = "/sellMenuP" + (pageNow - 1) }, };
                    indexToPaste++;
                }

                keyboard[indexToPaste] = new[] { new InlineKeyboardButton { Text = "Назад", CallbackData = "/backToStartMenu" }, };



                
                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, null, false);  // ...,...,alert    AnswerCallbackQuery is required to send to avoid clock animation ob the button
                dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id)).User_State_Id = 0;
                dbContext.SaveChanges();
                await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, "Вот ваши каналы.\n✔️ – канал отображается в каталоге, купить рекламу можно\n❌ – канал НЕ отображается в каталоге, купить рекламу нельзя.\nВозможные причины:\n    1. Выставлен СРМ = 0\n    2. Не добавлены рекламные места\n    3. В канале охват меньше 1500", replyMarkup: new InlineKeyboardMarkup(keyboard));
                
                
            }
            catch (Exception ex){ Console.WriteLine(ex.StackTrace + "\n" + ex.Message +"\n"); }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}

/*  Part to test some functions
try
{
Chat c = await botClient.GetChatAsync(update.ChannelPost.Chat.Id);
var mems =await botClient.GetChatAdministratorsAsync(c.Id);
var link=await botClient.ExportChatInviteLinkAsync(c.Id);
var mesage = await botClient.SendTextMessageAsync(c.Id, "Eto pisal bot");
//var cmsg = await botClient.EditMessageTextAsync(c.Id, 6, "a eto voobshe izmenil bot 2 raz");
await botClient.DeleteMessageAsync(c.Id, 6);
}
catch(Exception ex)
{
    return Ok();
}
return Ok();
*/
