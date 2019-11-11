using eAdvertisement_bot.Models.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;

namespace eAdvertisement_bot
{
    public class Bot
    {
        private static TelegramBotClient botClient;
        private static List<Command> commandsList;

        public static List<Command> Commands { get { return commandsList; } }

        // Bot initialization
        public static async Task<TelegramBotClient> GetBotClientAsync()
        {
            if (botClient!=null)
            {
                return botClient;
            }
            else
            {
                commandsList = new List<Command>();
                //TODO: Add here all commands that we have
                commandsList.Add(new StartCommand());
                commandsList.Add(new StopBotCommand());
                commandsList.Add(new LaunchBotCommand());
                commandsList.Add(new BuyMenuCommand());
                commandsList.Add(new BackFromBuyMenuCommand());
                commandsList.Add(new SellMenuCommand());

                botClient = new TelegramBotClient(BotSettings.Token);   // Token setting

                string hook = String.Concat(BotSettings.WebHookUrl,"/df443335");    // Setting the webhook for telegram
                await botClient.SetWebhookAsync(hook);
            }   
            return botClient;
        }

    }
}
