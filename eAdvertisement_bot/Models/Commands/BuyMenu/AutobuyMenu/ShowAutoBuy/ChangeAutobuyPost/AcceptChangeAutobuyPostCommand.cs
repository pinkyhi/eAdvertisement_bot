﻿using eAdvertisement_bot.DAO;
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
    public class AcceptChangeAutobuyPostCommand : Command
    {
        public override string Name => "/acceptChangeAutobuyPostN";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("acabpubN");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            int postId = Convert.ToInt32(update.CallbackQuery.Data.Substring(8));
            AppDbContext dbContext = new AppDbContext();
            try
            {
                List<DbEntities.Media> mediasDb = dbContext.Medias.ToList();
                List<DbEntities.Button> buttonsDb = dbContext.Buttons.ToList();

                DbEntities.Publication postToShow = dbContext.Publications.Find(postId);


                if (postToShow.Media != null && postToShow.Media.Count > 1)
                {
                    List<InputMediaPhoto> album = new List<InputMediaPhoto>();
                    for (int i = 0; i < postToShow.Media.Count; i++)
                    {
                        album.Add(new InputMediaPhoto(new InputMedia(postToShow.Media[i].Path)));
                    }
                    album[0].Caption = postToShow.Text != null ? postToShow.Text : "newPost";

                    await botClient.SendMediaGroupAsync(album, update.CallbackQuery.Message.Chat.Id);
                }
                else if (postToShow.Media != null && postToShow.Media.Count == 1)
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

                        await botClient.SendPhotoAsync(update.CallbackQuery.Message.Chat.Id, postToShow.Media[0].Path, caption: postToShow.Text != null ? postToShow.Text : "newPost", replyMarkup: new InlineKeyboardMarkup(keyboard));
                    }
                    else
                    {
                        await botClient.SendPhotoAsync(update.CallbackQuery.Message.Chat.Id, postToShow.Media[0].Path, caption: postToShow.Text != null ? postToShow.Text : "newPost");
                    }

                }
                else if (postToShow.Media == null || postToShow.Media.Count == 0)
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
                        await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, postToShow.Text != null ? postToShow.Text : "newPost", replyMarkup: new InlineKeyboardMarkup(keyboard));
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, postToShow.Text != null ? postToShow.Text : "newPost");
                    }

                }

                InlineKeyboardButton[][] keyboardControll = new[]
                {
                    new[]
                    {
                        new InlineKeyboardButton{Text = "Accept", CallbackData = "cabpubiaN"+postId},
                        new InlineKeyboardButton{Text = "Decline", CallbackData = "cabpub"}
                    },
                    new[]
                    {
                        new InlineKeyboardButton{Text="Back",CallbackData = "cabpub"}
                    }
                };

                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Do you want to attach this post?", replyMarkup: new InlineKeyboardMarkup(keyboardControll));


                await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);


            }
            catch (Exception ex)
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
