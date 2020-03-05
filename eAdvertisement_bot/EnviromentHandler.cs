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
        int Interval;

        public EnviromentHandler(TelegramBotClient botClient, int interval)
        {
            this.botClient = botClient;
            clientApiHandler = new ClientApiHandler();
            clientApiHandler.ConnectClient().Wait();
            clientApiHandler.SetClientId().Wait();
            Interval = interval;
        }

        public void Start()
        {

            while (true)
            {
                AppDbContext dbContext = new AppDbContext();
                try
                {
                    PublishAccepted(dbContext).Wait();
                    CloseAds(dbContext);
                    CheckAds(dbContext);
                    CloseTransactions(dbContext);
                    Thread.Sleep(Interval);
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
            DateTime now = DateTime.Now;
            List<Advertisement> adsToCheck = dbContext.Advertisements.Include("AdMessages").Where(ad => ad.Advertisement_Status_Id == 4 && now.Ticks - ad.Date_Time.Ticks>(TimeSpan.TicksPerMinute*10)).ToList();
            foreach(Advertisement ad in adsToCheck)
            {
                if (!clientApiHandler.IsWorkingPostOk(ad.AdMessages.Select(adm=>adm.AdMessage_Id).ToList(), ad).Result)
                {
                    ad.Advertisement_Status_Id = 6;
                }

            }
        }     
        public void CloseAds(AppDbContext dbContext)
        {
            DateTime now = DateTime.Now;
            List<Advertisement> ads = dbContext.Advertisements.Include("AdMessages").Include("Channel").Where(a => a.Advertisement_Status_Id == 4 && new DateTime(a.Date_Time.Ticks).AddHours(a.Alive) < now).ToList();
            for(int i = 0; i < ads.Count; i++)
            {
                ads[i].Advertisement_Status_Id = 5;
                if (ads[i].AdMessages != null)
                {
                    foreach (AdMessage adm in ads[i].AdMessages)
                        try
                        {
                            botClient.DeleteMessageAsync(ads[i].Channel_Id, adm.AdMessage_Id).Wait();
                        }
                        catch { }
                }
            }
            dbContext.SaveChanges();
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
            List<Advertisement> moneyForwardAds = dbContext.Advertisements.Include("Channel.User").Include("AdMessages").Where(a => a.Advertisement_Status_Id == 5).ToList();
            List<Advertisement> moneyBackAds = dbContext.Advertisements.Include("Autobuy").Include("User").Include("Channel.User").Include("AdMessages").Where(a => a.Advertisement_Status_Id == 3 || a.Advertisement_Status_Id == 6 || a.Advertisement_Status_Id == 10 || a.Advertisement_Status_Id == 11).ToList(); //3,6,10,11

            foreach (Advertisement ad in moneyBackAds)
            {
                if (ad.Autobuy == null)
                {
                    ad.User.Balance += ad.Price;
                    ad.Advertisement_Status_Id = 12;
                }
                else
                {
                    ad.Autobuy.Balance += ad.Price;
                    ad.Advertisement_Status_Id = 12;
                }

            }
            dbContext.SaveChanges();
            foreach (Advertisement ad in moneyForwardAds)
            {
                int coverage = clientApiHandler.GetCoverageOfPost(ad.AdMessages[0].AdMessage_Id, ad.Channel_Id).Result;
                if (ad.Price > (Convert.ToDouble(coverage) / 1000 * ad.Channel.Cpm))
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
        // Autobuy part
        public bool IsPlaceFreeForAd(AppDbContext dbContext, DateTime nowIs, long channelId, DateTime placeTime)
        {
            DateTime tDT = placeTime;

            if (new DateTime(tDT.Year, tDT.Month, tDT.Day, tDT.Hour, tDT.Minute, tDT.Second) < nowIs)
            {
                return false;

            }

            List<Advertisement> nearestAds = dbContext.Advertisements.Where(a => a.Advertisement_Status_Id == 1 && a.Autobuy_Id != null || a.Advertisement_Status_Id == 3 && a.Autobuy_Id != null || a.Advertisement_Status_Id == 2 || a.Advertisement_Status_Id == 4 || a.Advertisement_Status_Id == 9).Where(a => a.Channel_Id == channelId && a.Date_Time <= new DateTime(tDT.Year, tDT.Month, tDT.Day, tDT.Hour, tDT.Minute, tDT.Second)).ToList();
            Advertisement nearestAd = nearestAds.FirstOrDefault(a => a.Date_Time.Equals(nearestAds.Max(a => a.Date_Time)));

            List<Advertisement> nearestTopAds = dbContext.Advertisements.Where(a => a.Advertisement_Status_Id ==1 && a.Autobuy_Id!=null || a.Advertisement_Status_Id == 3 && a.Autobuy_Id != null || a.Advertisement_Status_Id == 2 || a.Advertisement_Status_Id == 4 || a.Advertisement_Status_Id == 9).Where(a => a.Channel_Id == channelId && a.Date_Time > new DateTime(tDT.Year, tDT.Month, tDT.Day, tDT.Hour, tDT.Minute, tDT.Second)).ToList();
            Advertisement nearestTopAd = nearestTopAds.FirstOrDefault(a => a.Date_Time.Equals(nearestTopAds.Min(a => a.Date_Time)));

            if (nearestAd != null)
            {
                if ((new DateTime(tDT.Year, tDT.Month, tDT.Day, tDT.Hour, tDT.Minute, tDT.Second)).Subtract(new TimeSpan(nearestAd.Top, 0, 0)) < nearestAd.Date_Time)
                {
                    return false;
                }
            }
            if (nearestTopAd != null)
            {
                if ((new DateTime(nowIs.Year, nowIs.Month, nowIs.Day, tDT.Hour, tDT.Minute, tDT.Second)).Add(new TimeSpan(1, 0, 0)) > nearestTopAd.Date_Time)
                {
                    return false;
                }
            }
            return true;
        }

        public void TryAutobuy(AppDbContext dbContext)
        {
            DateTime nowDT = DateTime.Now;
            TimeSpan nowTP = new TimeSpan(nowDT.Hour, nowDT.Minute, nowDT.Second);
            List<Autobuy> autobuys = dbContext.Autobuys.Include("Autobuy_Channels").Include("Autobuy_Channels.Channel").Include("Autobuy_Channels.Channel.Places").Include("Autobuy_Channels.Channel.Advertisements").Where(ab => ab.Balance != 0 && ab.State == 1 && ab.Daily_Interval_From < nowTP && nowTP < ab.Daily_Interval_To).ToList();
            foreach(Autobuy ab in autobuys)
            {
                for(int i = 0; i < ab.Autobuy_Channels.Count; i++)
                {
                    Channel channel = ab.Autobuy_Channels[i].Channel;
                    if (channel.Advertisements.FirstOrDefault(ad => ad.Advertisement_Status_Id==5 && ad.Date_Time.AddDays(ab.Interval) > new DateTime(nowDT.Ticks))!=null)
                    {
                        continue;
                    }
                    else
                    {
                        if (ab.Balance >= channel.Price)
                        {
                            try
                            {

                                List<DateTime> dateTimesForPlaces = new List<DateTime>(channel.Places.Count*2);
                                for(int j = 0; j < channel.Places.Count; j++)
                                {
                                    dateTimesForPlaces.Add(new DateTime(nowDT.Year, nowDT.Month, nowDT.Day, channel.Places[j].Time.Hours, channel.Places[j].Time.Minutes, channel.Places[j].Time.Seconds));
                                    dateTimesForPlaces.Add(new DateTime(nowDT.Year, nowDT.Month, nowDT.Day+1, channel.Places[j].Time.Hours, channel.Places[j].Time.Minutes, channel.Places[j].Time.Seconds));

                                }
                                dateTimesForPlaces = dateTimesForPlaces.Where(dt => dt.Ticks - nowDT.Ticks < TimeSpan.TicksPerDay*3 && dt > nowDT).ToList();
                                dateTimesForPlaces = dateTimesForPlaces.ToList();

                                dateTimesForPlaces = dateTimesForPlaces.Where(dt => IsPlaceFreeForAd(dbContext, nowDT, channel.Channel_Id, dt)).OrderBy(dt=>dt).ToList();



                                Advertisement newAd = new Advertisement { Advertisement_Status_Id = 1, Alive = 24, Top = 1, Channel_Id = channel.Channel_Id, Publication_Snapshot = ab.Publication_Snapshot, Date_Time = dateTimesForPlaces[0], User_Id = ab.User_Id, Autobuy_Id = ab.Autobuy_Id, Price = channel.Price };
                                dbContext.Advertisements.Add(newAd);
                                ab.Balance -= channel.Price;
                                dbContext.SaveChanges();
                                try
                                {
                                    botClient.SendTextMessageAsync(channel.User_Id, "У вас новый заказ на рекламу:)", disableNotification: true, replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Заказы", CallbackData = "/ordersMenuP0" })).Wait();
                                }
                                catch {   }
                            }
                            catch
                            {   }

                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
        }

        // Daily
        public void UpdateCoverage(AppDbContext dbContext)
        {
            List<Channel> channels = dbContext.Channels.ToList();
            foreach(Channel ch in channels){
                ch.Coverage = clientApiHandler.GetCoverageOfChannel(ch.Link, ch.Channel_Id, false).Result;
            }
            dbContext.SaveChanges();
        }
        public void CleanDB(AppDbContext dbContext) // Include delete of channels where bot isn't admin 
        {

            DateTime now = DateTime.Now;
            dbContext.Advertisements.RemoveRange(dbContext.Advertisements.Where(a => a.Date_Time.Ticks + TimeSpan.TicksPerDay * 62 < now.Ticks));
            dbContext.Channels.RemoveRange(dbContext.Channels.Where(c => !IsBotAdminInChat(c.Channel_Id)));
            dbContext.SaveChanges();

        }
        public bool IsBotAdminInChat(long chatId)
        {
            ChatMember[] admins = botClient.GetChatAdministratorsAsync(chatId).Result; ;
            ChatMember botAsAChatMember = admins.First(a => a.User.Id == botClient.BotId);
            bool isBotAdmin = botAsAChatMember != null;
            return isBotAdmin;
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
