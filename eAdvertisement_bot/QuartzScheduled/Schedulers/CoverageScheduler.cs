using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

// https://www.freeformatter.com/cron-expression-generator-quartz.html
namespace eAdvertisement_bot.QuartzScheduled.Schedulers
{
    public static class CoverageScheduler
    {
        public async static void Start(JobDataMap map)
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
            IJobDetail job = JobBuilder.Create<eAdvertisement_bot.QuartzScheduled.Jobs.CoverageUpdateJob>().
                WithDescription("Updates channels coverages every 24 hours").
                SetJobData(map).Build();
            ITrigger trigger = TriggerBuilder.Create().StartAt((DateTime)map["TakeOffTime"]).
                WithPriority(1).                                //StartAt(x => x = new DateTime(hour: 4, minute: 20)) 
                WithCronSchedule("0 20 4 ? * * *").             //every day at 4:20AM UTC (i guess) -> because everyone is asleep
                Build();
            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
