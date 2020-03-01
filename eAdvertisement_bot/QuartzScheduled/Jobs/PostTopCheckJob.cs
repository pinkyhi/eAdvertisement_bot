using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Telegram.Bot;
using eAdvertisement_bot.Models.DbEntities;
using eAdvertisement_bot.DAO;

//  TODO: get post id to db FROM DB 
//  TODO: change hold (if needed) - completed
//  TODO: Inform channel owner about interruption - completed
namespace eAdvertisement_bot.QuartzScheduled.Jobs
{
    public class PostTopCheckJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            AppDbContext dbContext = new AppDbContext();
            JobDataMap map = context.JobDetail.JobDataMap;
            ClientApiHandler cah = (ClientApiHandler)map["Client"];
            TelegramBotClient bot = (TelegramBotClient)map["Bot"];
            int interval = (int)map["AddCheckInterval"];
            //bool isInterrupted = await cah.CheckPostTop( /*...*/);
            DateTime now = DateTime.Now.AddSeconds(-DateTime.Now.Second);
            try
            {
               
                TimeSpan temp;
                List<Advertisement> advertisements = dbContext.Advertisements.Where(x => x.Advertisement_Status_Id == 2).Where(x => 
                    StartScheduler.DateInIntervalCheck(now.AddMinutes(-interval), now, x.Date_Time))
                    .ToList();
                foreach(Advertisement ad in advertisements)
                {
                    if (await cah.CheckPostTop(ad.Channel_Id, /*ad.Post_id*/))
                        continue;
                    else
                    {
                        await bot.SendTextMessageAsync(dbContext.Channels.Where(x => x.Channel_Id == ad.Channel_Id).First().User_Id,
                                                       $"You interrupted advertisement in \"{(await bot.GetChatAsync(ad.Channel_Id)).Username}\"" +
                                                       $" at {ad.Date_Time.ToString("HH:mm")}.\nHold is returned to buyer.");
                        dbContext.Users.First(x => x.User_Id == ad.User_Id).Balance += ad.Price;
                        ad.Advertisement_Status_Id = 6;
                    }
                        
                }
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally 
            {
                dbContext.Dispose();
            }
            
        }
        
        
    }
}

//foreach(Advertisement ad in dbContext.Advertisements)
//{
//    if (ad.Top < 1) temp = new TimeSpan(hours: 0, minutes: 59, seconds: 0);
//    else temp = new TimeSpan(hours: ad.Top - 1, minutes: 59, seconds: 0);

//    List<Advertisement> adsToCheck = 

//}
//TimeSpan temp;
//List<Advertisement> advertisements = dbContext.Advertisements.
//    Where(x => {
//        temp = new TimeSpan(hours: x.Top-1, minutes: 59, seconds: 0); 
//        x.Date_Time + temp == now; }).ToList();