using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace eAdvertisement_bot.QuartzScheduled.Schedulers
{
    public static class PostTopCheckScheduler
    {
        public async static void Start(JobDataMap map)
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
            IJobDetail job = JobBuilder.Create<eAdvertisement_bot.QuartzScheduled.Jobs.PostTopCheckJob>().
                WithDescription("Checks whethers posts are interrupted").
                SetJobData(map).Build();
            ITrigger trigger = TriggerBuilder.Create().StartAt((DateTime)map["TakeOffTime"]).
                WithPriority(3).
                WithSimpleSchedule(x => x.WithIntervalInMinutes((int)map["AddCheckInterval"]).RepeatForever()).
                Build();
            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
