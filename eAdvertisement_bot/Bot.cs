using eAdvertisement_bot.Models.Commands;
using eAdvertisement_bot.Models.Commands.ManualPurchase;
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
                commandsList.Add(new HowToCommand());
                commandsList.Add(new StartCommand());
                commandsList.Add(new StopBotCommand());
                commandsList.Add(new LaunchBotCommand());
                commandsList.Add(new BuyMenuCommand());
                commandsList.Add(new BackToStartMenu());
                commandsList.Add(new SellMenuCommand());
                commandsList.Add(new AddChannelCommand());
                commandsList.Add(new ShowChannelForSellerCommand());
                commandsList.Add(new DeleteChannelPlaceCommand());
                commandsList.Add(new MyPostsMenuCommand());
                commandsList.Add(new AddPostCommand());
                commandsList.Add(new ShowPostCommand());
                commandsList.Add(new AddImageToPostCommand());
                commandsList.Add(new AddButtonToPostCommand());
                commandsList.Add(new ChangeTextOnPostCommand());
                commandsList.Add(new ChangePostNameCommand());
                commandsList.Add(new DeletePostCommand());
                commandsList.Add(new ManualPurchaseMenuCommand());
                commandsList.Add(new SortsMenuCommand());
                commandsList.Add(new CategoriesMenuCommand());
                commandsList.Add(new ShowChannelForBuyerCommand());
                commandsList.Add(new ShowPlacesCalendarForBuyerCommand());
                commandsList.Add(new ShowPlacesForBuyerCommand());
                commandsList.Add(new ChoosePostForAddCommandcs());
                commandsList.Add(new BuyerPostInitConfirmationCommand());
                commandsList.Add(new AcceptManufactureBuyCommand());
                commandsList.Add(new InfoMenuCommand());
                commandsList.Add(new SoldPostsMenuCommand());
                commandsList.Add(new BoughtPostsMenuCommand());
                commandsList.Add(new OwnSoldMenuCommand());
                commandsList.Add(new AddOwnSoldTimeCommand());



                //TODO: Next add command texts
                commandsList.Add(new ChangeChannelCpmCommand());
                commandsList.Add(new ChangeChannelDescriptionCommand());
                commandsList.Add(new AddChannelPlaceCommand());

                //TODO: Next add events, because they require dbConnection
                commandsList.Add(new OnForwardMessageFromChannelEvent());
                commandsList.Add(new OnPhotoMessageEvent());
                commandsList.Add(new On10XStateEvent());
                commandsList.Add(new On20XStateEvent());
                commandsList.Add(new On9XXStateEvent());

                botClient = new TelegramBotClient(BotSettings.Token);   // Token setting

                string hook = String.Concat(BotSettings.WebHookUrl,"/df443335");    // Setting the webhook for telegram
                await botClient.SetWebhookAsync(hook);
            }   
            return botClient;
        }

    }
}
