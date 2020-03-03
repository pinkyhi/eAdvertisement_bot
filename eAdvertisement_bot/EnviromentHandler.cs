using eAdvertisement_bot.DAO;
using eAdvertisement_bot.Models.DbEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace eAdvertisement_bot
{
    public class EnviromentHandler
    {
        TelegramBotClient botClient;
        ClientApiHandler clientApiHandler;

        public EnviromentHandler(TelegramBotClient botClient)
        {
            this.botClient = botClient;
            clientApiHandler = new ClientApiHandler();
            clientApiHandler.ConnectClient().Wait();
            clientApiHandler.SetClientId().Wait();
        }

        public void Start()
        {

            while (true)
            {
                AppDbContext dbContext = new AppDbContext();
                try
                {

                    PublishAccepted(dbContext).Wait();
                    CloseTransactions(dbContext);
                    Thread.Sleep(4000);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    dbContext.Dispose();
                }
            }
        }

        public async Task PublishAccepted(AppDbContext dbContext)
        {
            List<Advertisement> ads = dbContext.Advertisements.Where(a => a.Advertisement_Status_Id == 2 && a.Date_Time < DateTime.Now).ToList();
            
            for(int i = 0; i < ads.Count; i++)
            {
                Publication post;
                try
                {
                    post = JsonSerializer.Deserialize<Publication>(ads[i].Publication_Snapshot);
                }
                catch
                {
                    ads[i].Advertisement_Status_Id = 10;
                    continue;
                }

                if (post != null && post.Text!= null) {
                    Message[] messages = await SendPostToChat(post, ads[i].Channel_Id, botClient);
                    try
                    {
                        ads[i].AdMessages = messages.Select(m=>new AdMessage {AdMessage_Id = m.MessageId, Advertisement_Id=ads[i].Advertisement_Status_Id }).ToList();
                        ads[i].Advertisement_Status_Id = 4;
                    }
                    catch
                    {
                        ads[i].Advertisement_Status_Id = 11;
                        continue;
                    }
                }
                else
                {
                    ads[i].Advertisement_Status_Id = 10;
                    continue;
                }
            }
            dbContext.SaveChanges();
        }
        public void CheckAds(AppDbContext dbContext)    // Delete or interrupt
        {

        }     
        public void CloseAds(AppDbContext dbContext)
        {

        }
        public void CloseOffers(AppDbContext dbContext)
        {
            List<Advertisement> advertisements = dbContext.Advertisements.Where(a => a.Advertisement_Status_Id == 1 && a.Date_Time< DateTime.Now).ToList();
            for(int i = 0; i < advertisements.Count; i++)
            {
                advertisements[i].Advertisement_Status_Id = 3;
            }
            dbContext.SaveChanges();

        }
        public void CloseTransactions(AppDbContext dbContext)   // Send money back or forward in dependency of ad status
        {
            List<Advertisement> moneyForwardAds = dbContext.Advertisements.Include("User").Include("Channel.User").Include("AdMessages").Where(a => a.Advertisement_Status_Id == 5).ToList();
            List<Advertisement> moneyBackAds = dbContext.Advertisements.Include("User").Include("Channel.User").Include("AdMessages").Where(a => a.Advertisement_Status_Id == 3 || a.Advertisement_Status_Id == 6 || a.Advertisement_Status_Id == 10 || a.Advertisement_Status_Id == 11).ToList(); //3,6,10,11

            foreach (Advertisement ad in moneyBackAds)
            {
                ad.User.Balance += ad.Price;
                ad.Advertisement_Status_Id = 12;
            }
            dbContext.SaveChanges();
            foreach (Advertisement ad in moneyForwardAds)
            {
                int coverage = clientApiHandler.GetCoverageOfPost(ad.AdMessages[0].AdMessage_Id, ad.Channel_Id).Result;
                if (ad.Price> (Convert.ToDouble(coverage)/ 1000*ad.Channel.Cpm))
                {
                    ad.Channel.User.Balance += Convert.ToInt32((Convert.ToDouble(coverage) / 1000 * ad.Channel.Cpm) * 0.93);
                }
                else
                {
                    ad.Channel.User.Balance += Convert.ToInt32(ad.Price * 0.93);
                }

                ad.Advertisement_Status_Id = 12;
            }
            dbContext.SaveChanges();
        }
        public void TryAutobuy(AppDbContext dbContext)
        {

        }

        // Daily
        public void UpdateCoverage(AppDbContext dbContext)
        {

        }
        public void CleanDB(AppDbContext dbContext) // Include delete of channels where bot isn't admin
        {

        }

        public async Task<Message[]> SendPostToChat(Publication post, long chatId, TelegramBotClient botClient)
        {
            if (post.Media != null && post.Media.Count > 1)
            {
                List<InputMediaPhoto> album = new List<InputMediaPhoto>();

                for (int i = 0; i < post.Media.Count; i++)
                {
                    album.Add(new InputMediaPhoto(new InputMedia(post.Media[i].Path)));
                    album[i].ParseMode = Telegram.Bot.Types.Enums.ParseMode.Markdown;
                }

                album[0].Caption = post.Text != null ? post.Text : "newPost";

                return await botClient.SendMediaGroupAsync(album, chatId);
            }
            else if (post.Media != null && post.Media.Count == 1)
            {
                if (post.Buttons != null)
                {
                    InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[post.Buttons.Count][];

                    int indexToPaste = 0;
                    foreach (Button b in post.Buttons)
                    {
                        keyboard[indexToPaste] = new[]
                        {
                                new InlineKeyboardButton{Text = b.Text, Url = b.Url}
                            };
                        indexToPaste++;
                    }

                    return new Message[] { await botClient.SendPhotoAsync(chatId, post.Media[0].Path, caption: post.Text, replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown) };
                }
                else
                {
                    return new Message[] { await botClient.SendPhotoAsync(chatId, post.Media[0].Path, caption: post.Text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown) };
                }

            }
            else if (post.Media == null || post.Media.Count == 0)
            {
                if (post.Buttons != null)
                {
                    InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[post.Buttons.Count][];

                    int indexToPaste = 0;
                    foreach (Button b in post.Buttons)
                    {
                        keyboard[indexToPaste] = new[]
                        {
                                new InlineKeyboardButton{Text = b.Text, Url = b.Url}
                            };
                        indexToPaste++;
                    }
                    return new Message[] { await botClient.SendTextMessageAsync(chatId, post.Text, replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown) };
                }
                else
                {
                    return new Message[] { await botClient.SendTextMessageAsync(chatId, post.Text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown) };
                }

            }
            return Array.Empty<Message>();
        }
    }
}
