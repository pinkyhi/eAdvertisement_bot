using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Telegram.Bot;
using eAdvertisement_bot.DAO;
using eAdvertisement_bot.Models.DbEntities;


namespace eAdvertisement_bot.QuartzScheduled.Jobs
{
    public class CoverageUpdateJob : IJob
    {

        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap map = context.JobDetail.JobDataMap;
            ClientApiHandler cah = (ClientApiHandler)map["Client"];
            AppDbContext dbContext = new AppDbContext();
            try
            {
                foreach (Channel channel in dbContext.Channels)
                {
                    channel.Coverage = await cah.GetCoverageOfChannel(channel.Link, channel.Channel_Id, false);
                    // it might ruin everything(await + foreach), but it should not
                }

                dbContext.SaveChanges();
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

    }
}

// in case everything blows up:
//
// AppDbContext dbContext;
// ClientApiHandler cah;
//
//var tasks = new List<Task>();
//foreach (Channel channel in dbContext.Channels)
//{
//    Task task = UpdateCoverage(channel);
//    tasks.Add(task);
//}
//await Task.WhenAll();
//

//private async Task UpdateCoverage(Channel channel)
//{
//    channel.Coverage = await cah.GetCoverageOfChannel(channel.Link, channel.Channel_Id, false);
//}