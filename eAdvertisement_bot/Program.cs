using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace eAdvertisement_bot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            EnviromentHandler eh = new EnviromentHandler(Bot.GetBotClientAsync().Result, 60000);
            EnviromentHandler ed = new EnviromentHandler(Bot.GetBotClientAsync().Result, 86400000);
            Thread edThread = new Thread(new ThreadStart(ed.StartEveryDay));
            Thread ehThread = new Thread(new ThreadStart(eh.StartEveryMinute));
            ehThread.Start();
            CreateHostBuilder(args).Build().Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
