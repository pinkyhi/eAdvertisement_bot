using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using eAdvertisement_bot.DAO;
using eAdvertisement_bot.Models.DbEntities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Text.Json;

namespace eAdvertisement_bot.QuartzScheduled.Jobs
{
    public class SendPostponedJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            
            JobDataMap map = context.JobDetail.JobDataMap;
            TelegramBotClient bot = (TelegramBotClient)map["Bot"];
            AppDbContext dbContext = new AppDbContext();
            Publication publication;
            DateTime now = DateTime.Now;
            now = now.AddTicks(-now.Ticks % TimeSpan.TicksPerMinute);
            // now.Kind = DateTimeKind.Unspecified;


            try
            {
                foreach (Advertisement ad in dbContext.Advertisements)
                {
                    
                    if (DateTime.Compare(ad.Date_Time, now) == 0) //t1 is the same as t2.
                    {
                        ChatMember[] admins = await bot.GetChatAdministratorsAsync(ad.Channel_Id);
                        ChatMember adminBot = admins.First(a => a.User.Id == bot.BotId);
                        if (!(adminBot.CanDeleteMessages == true && adminBot.CanEditMessages == true && adminBot.CanPostMessages == true))
                        {
                            Channel guilty = dbContext.Channels.First(x => x.Channel_Id == ad.Channel_Id);
                            await bot.SendTextMessageAsync(guilty.User_Id,
                                                           $"You removed {BotSettings.Name} from your \"{guilty.Name}\"." +
                                                           $" Return it to be able to sell ads on the channel.");
                            //TODO: return hold, change states etc.
                        }

                        else
                        {

                            await bot.SendTextMessageAsync(330507566, $"\n{DateTime.Now}\n");

                            //await SendPost(JsonSerializer.Deserialize<Publication>(ad.Publication_Snapshot), 
                            //               ad.Channel_Id, bot);
                            await bot.SendTextMessageAsync(ad.Channel_Id, "Ну вот оно и первое отправленное сообщение в группу" +
                                ". На костылях конечно, но хоть как-то(");

                            ad.Advertisement_Status_Id = 5;

                            dbContext.SaveChanges();
                        }
                        // await bot.
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        private async Task SendPost(Publication publication, ChatId id, TelegramBotClient bot)
        {
            if (publication.Media != null && publication.Media.Count > 1)
            {
                List<InputMediaPhoto> album = new List<InputMediaPhoto>();
                for (int i = 0; i < publication.Media.Count; i++)
                {
                    album.Add(new InputMediaPhoto(new InputMedia(publication.Media[i].Path)));
                }
                album[0].Caption = publication.Text != null ? publication.Text : "";

                await bot.SendMediaGroupAsync(album, id);
            }
            else if (publication.Media != null && publication.Media.Count == 1)
            {
                if (publication.Buttons != null)
                {
                    InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[publication.Buttons.Count][];

                    int indexToPaste = 0;
                    foreach (Button b in publication.Buttons)
                    {
                        keyboard[indexToPaste] = new[]
                        {
                                            new InlineKeyboardButton{Text = b.Text, Url = b.Url}
                                        };
                        indexToPaste++;
                    }

                    await bot.SendPhotoAsync(id, publication.Media[0].Path,
                        caption: publication.Text != null ? publication.Text : "",      //null?
                        replyMarkup: new InlineKeyboardMarkup(keyboard));
                }
                else
                {
                    await bot.SendPhotoAsync(id, publication.Media[0].Path,
                        caption: publication.Text != null ? publication.Text : "");
                }

            }
            else if (publication.Media == null || publication.Media.Count == 0)
            {
                if (publication.Buttons != null)
                {
                    InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[publication.Buttons.Count][];

                    int indexToPaste = 0;
                    foreach (Button b in publication.Buttons)
                    {
                        keyboard[indexToPaste] = new[]
                        {
                                new InlineKeyboardButton{Text = b.Text, Url = b.Url}
                            };
                        indexToPaste++;
                    }
                    await bot.SendTextMessageAsync(id, publication.Text != null ? publication.Text : "newPost",
                        replyMarkup: new InlineKeyboardMarkup(keyboard));
                }
                else
                {
                    await bot.SendTextMessageAsync(id, publication.Text != null ? publication.Text : "newPost");
                }
            }
        }

    }
}
