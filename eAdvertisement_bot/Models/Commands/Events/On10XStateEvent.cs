using eAdvertisement_bot.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace eAdvertisement_bot.Models.Commands
{
    public class On10XStateEvent : Command  
    {
        public override string Name => "/on10XStateEvent";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                return false;
            }
            else
            {
                AppDbContext dbContext = new AppDbContext();
                try
                {
                    DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.Message.From.Id));
                    return Convert.ToString(user.User_State_Id).StartsWith("10") && (Convert.ToString(user.User_State_Id)).Length > 2;
                }
                catch
                {
                    return false;
                }
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.Message.From.Id));
                long tag = user.User_State_Id;
                if (tag==102)
                {
                    List<DbEntities.Button> btns = new List<DbEntities.Button>();
                    int postId = Convert.ToInt32(user.Object_Id);
                    if (dbContext.Medias.Count(m => m.Publication_Id == postId) < 2)
                    {
                        int existAlready = dbContext.Buttons.Count(b => b.Publication_Id == postId);
                        string text = update.Message.Text;
                        int maxInd = 0;
                        while (maxInd < 5 - existAlready)
                        {
                            int iofc = text.IndexOf('(');
                            int iosc = text.IndexOf(')');
                            int iofs = text.IndexOf('[');
                            int ioss = text.IndexOf(']');
                            if (iofc == -1 || iosc == -1 || iofs == -1 || ioss == -1)
                            {
                                break;
                            }

                            string buttonText = text.Substring(iofc + 1, iosc - iofc - 1);
                            string buttonUrl = text.Substring(iofs + 1, ioss - iofs - 1);
                            dbContext.Buttons.Add(new DbEntities.Button { Text = buttonText, Url = buttonUrl, Publication_Id = postId });
                            text = text.Substring(ioss + 1);
                            maxInd++;
                        }
                        user.User_State_Id = 0;
                        dbContext.SaveChanges();
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, maxInd + " buttons are added!");
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id,"You can't use buttons because you attached more than 1 image\nRecreate post if you want to use buttons");
                    }

                }
                else if(tag==103)
                {
                    dbContext.Publications.Find(Convert.ToInt32(user.Object_Id)).Text = update.Message.Text;
                    user.User_State_Id = 0;
                    dbContext.SaveChanges();
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Text is changed!");
                }
                else if (tag == 104)
                {
                    dbContext.Publications.Find(Convert.ToInt32(user.Object_Id)).Name = update.Message.Text;
                    user.User_State_Id = 0;
                    dbContext.SaveChanges();
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Name is changed!");
                }
            }
            catch(Exception ex)
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, ex.Message);
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
