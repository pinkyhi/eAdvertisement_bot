using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace eAdvertisement_bot.QuartzScheduled.Schedulers
{
    public static class PostScheduler
    {
        public async static void Start(JobDataMap map)
        {
            
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
            IJobDetail job = JobBuilder.Create<eAdvertisement_bot.QuartzScheduled.Jobs.SendPostponedJob>().
                WithDescription("Sends scheduled publications").
                SetJobData(map).Build();
            ITrigger trigger = TriggerBuilder.Create().StartAt((DateTime)map["TakeOffTime"]).
                WithPriority(10).
                WithSimpleSchedule(x => x.WithIntervalInMinutes(1).RepeatForever()).
                Build();
            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
