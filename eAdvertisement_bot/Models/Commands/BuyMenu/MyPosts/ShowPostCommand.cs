using eAdvertisement_bot.DAO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace eAdvertisement_bot.Models.Commands
{
    public class ShowPostCommand : Command
    {
        public override string Name => "/showPostN";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("/showPostN");
            }
        }

        public override async Task Execute(Update update, TelegramBotClient botClient)
        {
            int postId = Convert.ToInt32(update.CallbackQuery.Data.Substring(10));
            AppDbContext dbContext = new AppDbContext();
            try
            {
                List<DbEntities.Media> mediasDb = dbContext.Medias.ToList();
                List<DbEntities.Button> buttonsDb = dbContext.Buttons.ToList();

                DbEntities.Publication postToShow = dbContext.Publications.Find(postId);


                if (postToShow.Media!=null && postToShow.Media.Count > 1)
                {
                    List<InputMediaPhoto> album = new List<InputMediaPhoto>();
                    
                    for (int i = 0; i < postToShow.Media.Count; i++)
                    {
                        album.Add(new InputMediaPhoto(new InputMedia(postToShow.Media[i].Path)));
                        album[i].ParseMode = Telegram.Bot.Types.Enums.ParseMode.Markdown;
                    }

                    album[0].Caption = postToShow.Text != null ? postToShow.Text : "newPost";

                    await botClient.SendMediaGroupAsync(album, update.CallbackQuery.Message.Chat.Id);
                }
                else if (postToShow.Media !=null && postToShow.Media.Count == 1)
                {
                    if (postToShow.Buttons != null)
                    {
                        InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[postToShow.Buttons.Count][];

                        int indexToPaste = 0;
                        foreach (DbEntities.Button b in postToShow.Buttons)
                        {
                            keyboard[indexToPaste] = new[]
                            {
                                new InlineKeyboardButton{Text = b.Text, Url = b.Url}
                            };
                            indexToPaste++;
                        }

                        await botClient.SendPhotoAsync(update.CallbackQuery.Message.Chat.Id, postToShow.Media[0].Path, caption: postToShow.Text != null ? postToShow.Text : "newPost", replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    }
                    else
                    {
                        await botClient.SendPhotoAsync(update.CallbackQuery.Message.Chat.Id, postToShow.Media[0].Path, caption: postToShow.Text != null ? postToShow.Text : "newPost", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    }

                }
                else if (postToShow.Media==null || postToShow.Media.Count == 0)
                {
                    if (postToShow.Buttons != null)
                    {
                        InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[postToShow.Buttons.Count][];

                        int indexToPaste = 0;
                        foreach (DbEntities.Button b in postToShow.Buttons)
                        {
                            keyboard[indexToPaste] = new[]
                            {
                                new InlineKeyboardButton{Text = b.Text, Url = b.Url}
                            };
                            indexToPaste++;
                        }
                        await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, postToShow.Text != null ? postToShow.Text : "newPost", replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, postToShow.Text != null ? postToShow.Text : "newPost", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    }
                   
                }

                InlineKeyboardButton[][] keyboardControll = new[]
                {
                    new[]
                    {
                        new InlineKeyboardButton{Text = "Медиа", CallbackData = "/addImageToPostN"+postId},
                        new InlineKeyboardButton{Text = "Кнопки", CallbackData = "/addButtonToPostN"+postId}
                    },
                    new[]
                    {
                        new InlineKeyboardButton{Text = "Текст", CallbackData = "/changeTextOnPostN"+postId},
                        new InlineKeyboardButton{Text = "Удалить пост", CallbackData = "/deletePostN"+postId}
                    },
                    new[]
                    {
                        new InlineKeyboardButton{Text="Изменить имя",CallbackData = "/changePostNameN"+postId}
                    },
                    new[]
                    {
                        new InlineKeyboardButton{Text="Назад",CallbackData = "/myPostsMenu"}
                    }
                };
                
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Кнопки для изменения поста\n*Не забудьте выставить какой-то текст, иначе реклама не запостится!*", replyMarkup: new InlineKeyboardMarkup(keyboardControll), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

                await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                

            }
            catch(Exception ex)
            {
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, ex.Message);
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
