using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace eAdvertisement_bot.Models.Commands
{
    public abstract class Command
    {
        public abstract string Name { get; }
        public abstract Task Execute(Update update, TelegramBotClient botClient); // This method contains the main logic which should be executed if this command contains in update message
        public abstract bool Contains(Update update); // This method is used to check if this command contains in update message


        // Static elements


        protected InlineKeyboardMarkup entryStoppedBotKeyboard = new InlineKeyboardMarkup(new[]
        {
                        new[] //first row
                        {
                            new InlineKeyboardButton { Text = "Ввод средств", CallbackData = "input" },
                             new InlineKeyboardButton { Text = "Вывод", CallbackData = "output" },

                        },
                        new[] // second row
                        {
                            new InlineKeyboardButton { Text = "Купить", CallbackData = "/buyMenu" },
                            new InlineKeyboardButton { Text = "Продать", CallbackData = "/sellMenuP0" },
                        },
                        new[] // third row
                        {                            
                            new InlineKeyboardButton { Text = "Купленные", CallbackData = "/boughtPostsMenu" },
                            new InlineKeyboardButton { Text = "Проданные", CallbackData = "/soldPostsMenu" },
                        },
                        new[] // fourth row
                        {
                            new InlineKeyboardButton { Text = "Заказы", CallbackData = "/ordersMenuP0" },
                        },
                        new[] // fifth row
                        {
                            new InlineKeyboardButton { Text = "Инфо", CallbackData = "/infoMenu" },
                        }
        });


        protected InlineKeyboardMarkup entryLaunchedBotKeyboard = new InlineKeyboardMarkup(new[]
        {
                        new[] //first row
                        {
                            new InlineKeyboardButton { Text = "Ввод средств", CallbackData = "input" },
                            new InlineKeyboardButton { Text = "Вывод", CallbackData = "output" },
                        },
                        new[] // second row
                        {
                            new InlineKeyboardButton { Text = "Купить", CallbackData = "/buyMenu" },
                            new InlineKeyboardButton { Text = "Продать", CallbackData = "/sellMenuP0" },
                        },
                        new[] // third row
                        {
                            new InlineKeyboardButton { Text = "Купленные", CallbackData = "/boughtPostsMenu" },
                            new InlineKeyboardButton { Text = "Проданные", CallbackData = "/soldPostsMenu" },
                        },
                        new[] // fourth row
                        {
                            new InlineKeyboardButton { Text = "Заказы", CallbackData = "/ordersMenuP0" },
                        },
                        new[] // fifth row
                        {
                            new InlineKeyboardButton { Text = "Инфо", CallbackData = "/infoMenu" },
                        },
                        new[] // sixth row
                        {
                            new InlineKeyboardButton { Text = "Feedback", CallbackData = "feedback" },
                        }
        });

        protected InlineKeyboardMarkup buyMenuKeyboard = new InlineKeyboardMarkup(new[]
       {
                        new[] //first row
                        {
                            new InlineKeyboardButton { Text = "Каталог", CallbackData = "/manualPurchaseMenuP0IIS1,2C" },
                            new InlineKeyboardButton { Text = "Автозакуп", CallbackData = "abblock" },  //  abm in RELEASE
                        },
                        new[] // second row
                        {
                            new InlineKeyboardButton { Text = "Мои посты", CallbackData = "/myPostsMenu" },
                        },
                        new[] // third row
                        {
                            new InlineKeyboardButton { Text = "Назад", CallbackData = "/backToStartMenu" },
                        }
        });


        public async void SendPost(DbEntities.Publication post, Update update, TelegramBotClient botClient )
        {
            if (post.Media != null && post.Media.Count > 1)
            {
                List<InputMediaPhoto> album = new List<InputMediaPhoto>();
                for (int i = 0; i < post.Media.Count; i++)
                {
                    album.Add(new InputMediaPhoto(new InputMedia(post.Media[i].Path)));
                }
                album[0].Caption = post.Text != null ? post.Text : "newPost";
                album[0].ParseMode = ParseMode.Markdown;


                await botClient.SendMediaGroupAsync(album, update.CallbackQuery.Message.Chat.Id);
            }
            else if (post.Media != null && post.Media.Count == 1)
            {
                if (post.Buttons != null)
                {
                    InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[post.Buttons.Count][];

                    int indexToPaste = 0;
                    foreach (DbEntities.Button b in post.Buttons)
                    {
                        keyboard[indexToPaste] = new[]
                        {
                                new InlineKeyboardButton{Text = b.Text, Url = b.Url}
                            };
                        indexToPaste++;
                    }

                    await botClient.SendPhotoAsync(update.CallbackQuery.Message.Chat.Id, post.Media[0].Path, caption: post.Text != null ? post.Text : "newPost", replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: ParseMode.Markdown);
                }
                else
                {
                    await botClient.SendPhotoAsync(update.CallbackQuery.Message.Chat.Id, post.Media[0].Path, caption: post.Text != null ? post.Text : "newPost", parseMode: ParseMode.Markdown);
                }

            }
            else if (post.Media == null || post.Media.Count == 0)
            {
                if (post.Buttons != null)
                {
                    InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[post.Buttons.Count][];

                    int indexToPaste = 0;
                    foreach (DbEntities.Button b in post.Buttons)
                    {
                        keyboard[indexToPaste] = new[]
                        {
                                new InlineKeyboardButton{Text = b.Text, Url = b.Url}
                            };
                        indexToPaste++;
                    }
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, post.Text != null ? post.Text : "newPost", replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: ParseMode.Markdown, disableWebPagePreview: true);
                }
                else
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, post.Text != null ? post.Text : "newPost", parseMode: ParseMode.Markdown, disableWebPagePreview: true);
                }

            }
        }

    }
}
