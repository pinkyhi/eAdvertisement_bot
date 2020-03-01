using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using eAdvertisement_bot.QuartzScheduled.Schedulers;

namespace eAdvertisement_bot.QuartzScheduled
{
    public static class StartScheduler
    {
        public static async void StartSchedulers()
        {
            DateTime startTime = DateTime.Now.AddMinutes(1).AddSeconds(-DateTime.Now.Second);
            ClientApiHandler cah = new ClientApiHandler();
            await cah.ConnectClient();
            await cah.SetClientId();
            
            ///Indicates time interval between top check on posted publications
            int interval = 5;

            JobDataMap map = new JobDataMap
            {
                ["Bot"] = await Bot.GetBotClientAsync(),
                ["Client"] = cah,
                ["TakeOffTime"] = startTime,
                ["AddCheckInterval"] = interval
            };

            //add Schedulers here:
            PostScheduler.Start(map);
            CoverageScheduler.Start(map);
            PostTopCheckScheduler.Start(map);
        }

        public static bool DateInIntervalCheck(DateTime startDate, DateTime endDate, DateTime dateToCheck)
        {
            return dateToCheck >= startDate && dateToCheck < endDate;
        }
    }
}
