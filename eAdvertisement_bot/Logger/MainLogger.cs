using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace eAdvertisement_bot.Logger
{
    public static class MainLogger
    {
        static string writePath = @"log.txt";
        public static bool DEV { get; set; } = false;
        public static void LogException(Exception ex, string addStr = "")
        {
            lock (writePath)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
                    {
                        sw.WriteLine($"{DateTime.Now}\n{addStr}\n{ex.Message}\n\n{ex.StackTrace}\n\n\n");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
