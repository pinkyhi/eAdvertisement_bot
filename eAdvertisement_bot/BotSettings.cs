using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eAdvertisement_bot
{
    public static class BotSettings
    {
        public static string Name { get; set; } = "eAdvertisement_bot";
        public static string Token { get; set; } = "1020132635:AAFyYm-SPKdcL16Tejtc-A5F3tZXQvtfZww";    // Bot token that gives botfather
        public static string WebHookUrl { get; set; } = "https://df443335.ngrok.io";    // Part of webhook url that gives ngrok. Command to get it for ssl in IIS Express: ngrok http https://localhost:44360 -host-header="localhost:44360"

    }
}
