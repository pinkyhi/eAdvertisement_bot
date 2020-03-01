﻿using eAdvertisement_bot.DAO;
using eAdvertisement_bot.Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;


namespace eAdvertisement_bot.Models.Commands
{
    public class OrdersMenuCommand : Command
    {
        public override string Name => "/ordersMenuP";

        public override bool Contains(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }
            else
            {
                var data = update.CallbackQuery.Data;
                return data.StartsWith("/ordersMenuP");
            }
        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            int page = Convert.ToInt32(update.CallbackQuery.Data.Substring(update.CallbackQuery.Data.IndexOf('P')+1));
            AppDbContext dbContext = new AppDbContext();
            try
            {
                DbEntities.User userEntity = dbContext.Users.Find(Convert.ToInt64(update.CallbackQuery.From.Id));
                List<Channel> channels = dbContext.Channels.Where(c => c.User_Id == userEntity.User_Id).ToList();
                List<long> channelIds = channels.Select(c => c.Channel_Id).ToList();
                List<Advertisement> ads = dbContext.Advertisements.Where(a => a.Date_Time>DateTime.Now && channelIds.Contains(a.Channel_Id) && a.Advertisement_Status_Id == 1).OrderByDescending(a=>a.Price).OrderBy(a=>a.Date_Time).ToList();
                
                Advertisement ad = ads!=null && ads.Count!=0 && ads[page] != null ? ads[page] : null;
                if (ad != null)
                {
                    Publication post;
                    try
                    {
                        post = JsonSerializer.Deserialize<Publication>(ad.Publication_Snapshot);
                    }
                    catch (Exception ex)
                    {
                        post = null;
                        await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, ex.Message);
                    }

                    if (post.Media != null && post.Media.Count > 1)
                    {
                        List<InputMediaPhoto> album = new List<InputMediaPhoto>();

                        for (int i = 0; i < post.Media.Count; i++)
                        {
                            album.Add(new InputMediaPhoto(new InputMedia(post.Media[i].Path)));
                            album[i].ParseMode = Telegram.Bot.Types.Enums.ParseMode.Markdown;
                        }

                        album[0].Caption = post.Text != null ? post.Text : "newPost";

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

                            await botClient.SendPhotoAsync(update.CallbackQuery.Message.Chat.Id, post.Media[0].Path, caption: post.Text != null ? post.Text : "newPost", replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                        }
                        else
                        {
                            await botClient.SendPhotoAsync(update.CallbackQuery.Message.Chat.Id, post.Media[0].Path, caption: post.Text != null ? post.Text : "newPost", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
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
                            await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, post.Text != null ? post.Text : "newPost", replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, post.Text != null ? post.Text : "newPost", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                        }

                    }
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id,"_____________End of post____________");

                    string text = "*Информация о заказе*\n" +
                    "Канал: " + "[" + ad.Channel.Name + "](" + ad.Channel.Link + ")" +
                    "\nЦена: " + ad.Price +
                    "\nТоп: " + ad.Top +
                    "\nЛента: " + ad.Alive +
                    "\nВремя постинга: " + ad.Date_Time;
                    InlineKeyboardButton[][] controllKeyboard;
                    if (ads.Count > 1)
                    {
                        controllKeyboard = new InlineKeyboardButton[3][];
                        if (page == 0)
                        {//→→→←←←
                            controllKeyboard[0] = new[]
                            {
                            new InlineKeyboardButton{CallbackData = "afpC"+ad.Channel_Id+"D"+ad.Date_Time, Text = "Accept"},
                            new InlineKeyboardButton{CallbackData = "rfpC"+ad.Channel_Id+"D"+ad.Date_Time, Text = "Regret"}

                        };
                            controllKeyboard[1] = new[] { new InlineKeyboardButton { CallbackData = "/ordersMenuP" + (page + 1), Text = "→→→"+(page+1) } };
                            controllKeyboard[2] = new[] { new InlineKeyboardButton { CallbackData = "/backToStartMenu", Text = "Back" } };
                        }
                        else if (page == ads.Count - 1)
                        {
                            controllKeyboard[0] = new[]
    {
                            new InlineKeyboardButton{CallbackData = "afpC"+ad.Channel_Id+"D"+ad.Date_Time, Text = "Accept"},
                            new InlineKeyboardButton{CallbackData = "rfpC"+ad.Channel_Id+"D"+ad.Date_Time, Text = "Regret"}

                        };
                            controllKeyboard[1] = new[] { new InlineKeyboardButton { CallbackData = "/ordersMenuP" + (page - 1), Text = (page-1)+"←←←" } };
                            controllKeyboard[2] = new[] { new InlineKeyboardButton { CallbackData = "/backToStartMenu", Text = "Back" } };
                        }
                        else
                        {
                            controllKeyboard[0] = new[]
    {
                            new InlineKeyboardButton{CallbackData = "afpC"+ad.Channel_Id+"D"+ad.Date_Time, Text = "Accept"},
                            new InlineKeyboardButton{CallbackData = "rfpC"+ad.Channel_Id+"D"+ad.Date_Time, Text = "Regret"}

                        };
                            controllKeyboard[1] = new[] { new InlineKeyboardButton { CallbackData = "/ordersMenuP" + (page - 1), Text = (page-1)+"←←←" }, new InlineKeyboardButton { CallbackData = "/ordersMenuP" + page + 1, Text = "→→→"+(page+1) } };
                            controllKeyboard[2] = new[] { new InlineKeyboardButton { CallbackData = "/backToStartMenu", Text = "Back" } };
                        }
                    }
                    else
                    {
                        controllKeyboard = new InlineKeyboardButton[2][];
                        controllKeyboard[0] = new[]
                            {
                            new InlineKeyboardButton{CallbackData = "afpC"+ad.Channel_Id+"D"+ad.Date_Time, Text = "Accept"},
                            new InlineKeyboardButton{CallbackData = "rfpC"+ad.Channel_Id+"D"+ad.Date_Time, Text = "Regret"}

                        };
                        controllKeyboard[1] = new[] { new InlineKeyboardButton { CallbackData = "/backToStartMenu", Text = "Back" } };
                    }


                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, disableWebPagePreview: true, replyMarkup: new InlineKeyboardMarkup(controllKeyboard));

                }
                else
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Пока у вас нет заказов.", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { CallbackData = "/backToStartMenu", Text = "Back" }));
                }


                try
                {
                    await botClient.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                }
                catch (Exception ex)
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, ex.Message);
                }



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
