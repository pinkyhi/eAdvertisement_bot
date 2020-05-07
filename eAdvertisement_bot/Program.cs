using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using eAdvertisement_bot.Logger;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace eAdvertisement_bot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MainLogger.LogException(new Exception());
            try
            {
                ClientApiHandler.ConnectClient().Wait();
                ClientApiHandler.SetClientId().Wait();

            }
            catch
            {
                string hash = ClientApiHandler.Client.SendCodeRequestAsync("+380509400345").Result;
                Console.WriteLine("Write your telegram code");
                var code = Console.ReadLine();
                var user = ClientApiHandler.Client.MakeAuthAsync("+380509400345", hash, code).Result;
            }
            EnviromentHandler eh = new EnviromentHandler(Bot.GetBotClientAsync().Result, 60000);
            EnviromentHandler ed = new EnviromentHandler(Bot.GetBotClientAsync().Result, 43200000);

            Thread edThread = new Thread(new ThreadStart(ed.StartEveryDay));
            Thread ehThread = new Thread(new ThreadStart(eh.StartEveryMinute));

            ehThread.Start();
            edThread.Start();

            CreateHostBuilder(args).Build().Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(options =>
                    {
                        

                        options.Listen(IPAddress.Loopback, 443, listenOptions =>
                        {
                            listenOptions.UseHttps("eadvertisements.pfx", "22Dkflbvbhjd06");
                        });
                        options.Listen(IPAddress.Any, 443, listenOptions =>
                        {
                            listenOptions.UseHttps("eadvertisements.pfx", "22Dkflbvbhjd06");
                        });

                    }
                    

                    );

                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://localhost:44359");
                    webBuilder.ConfigureKestrel(o =>
                    {
                        o.ConfigureHttpsDefaults(o =>
                    o.ClientCertificateMode =
                        ClientCertificateMode.NoCertificate);
                    });
                })
                .ConfigureServices((context, services) =>
                {
                    services.Configure<KestrelServerOptions>(
                        context.Configuration.GetSection("Kestrel"));
                });
    }
}
