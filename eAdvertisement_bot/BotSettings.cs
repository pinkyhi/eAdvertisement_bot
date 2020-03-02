using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eAdvertisement_bot
{
    public static class BotSettings
    {
        public static string Name { get; set; } = "jellyboat_bot";
        public static string Token { get; set; } = "991666001:AAH_ZRAL43agPFsLJsOK6aOB0UeWnq6SlzQ";    // Bot token that gives botfather
        public static string WebHookUrl { get; set; } = "https://5c363b30.ngrok.io";    // Part of webhook url that gives ngrok. Command to get it for ssl in IIS Express: ngrok http https://localhost:44360 -host-header="localhost:44360"

    }
}
