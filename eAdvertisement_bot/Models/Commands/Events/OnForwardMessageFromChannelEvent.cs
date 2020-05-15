using eAdvertisement_bot.DAO;
using eAdvertisement_bot.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TeleSharp.TL;
using TeleSharp.TL.Messages;
using TLSharp.Core;

namespace eAdvertisement_bot.Models.Commands
{
    public class OnForwardMessageFromChannelEvent : Command
    {
        public override string Name => "/onForwardMessageFromChannelEvent";

        public override bool Contains(Update update)
        {
            if(update.Type == Telegram.Bot.Types.Enums.UpdateType.Message && update.Message.ForwardFromChat!= null && update.Message.ForwardFromChat.Type == Telegram.Bot.Types.Enums.ChatType.Channel)
            {
                long userStateId = 0;
                AppDbContext dbContext = new AppDbContext();
                try
                {
                    userStateId = Convert.ToInt32(dbContext.Users.First(u => u.User_Id == update.Message.From.Id).User_State_Id);
                    if (userStateId == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch { }
                finally { dbContext.Dispose(); }
            }

            return false;

        }

        public async override Task Execute(Update update, TelegramBotClient botClient)
        {
            int userStateId = 0;
            long chatId = update.Message.ForwardFromChat.Id;
            long botId = botClient.BotId;
            AppDbContext dbContext = new AppDbContext();
            try
            {
                userStateId = Convert.ToInt32(dbContext.Users.First(u => u.User_Id == update.Message.From.Id).User_State_Id);
                if (userStateId == 1)
                {

                    DbEntities.Channel chInDb = dbContext.Channels.Find(chatId);


                    ChatMember[] admins = await botClient.GetChatAdministratorsAsync(chatId);
                    ChatMember botAsAChatMember = admins.First(a => a.User.Id == botId);
                    bool isBotAdmin = botAsAChatMember != null;
                    bool isUserACreator = admins.First(a => a.User.Id == update.Message.From.Id).Status == Telegram.Bot.Types.Enums.ChatMemberStatus.Creator;

                    if (chInDb == null)
                    {
                        if (isBotAdmin && isUserACreator)
                        {
                            int coverage = 0;
                            if (botAsAChatMember.CanDeleteMessages == true && botAsAChatMember.CanEditMessages == true && botAsAChatMember.CanPostMessages == true)
                            {


                                //await ClientApiHandler.ConnectClient();
                                //await ClientApiHandler.SetClientId();


                                if ((await botClient.GetChatAsync(chatId)).InviteLink == null)
                                {
                                    await botClient.ExportChatInviteLinkAsync(update.Message.ForwardFromChat.Id);
                                }

                                string inviteLink = (await botClient.GetChatAsync(chatId)).InviteLink;

                                try
                                {
                                    Chat s = await botClient.GetChatAsync(chatId);
                                    lock (ClientApiHandler.Client)
                                    {
                                        try
                                        {
                                            if ((botClient.GetChatMemberAsync(chatId, ClientApiHandler.Client_Id).Result).Status == Telegram.Bot.Types.Enums.ChatMemberStatus.Member)
                                            {
                                                coverage = ClientApiHandler.GetCoverageOfChannel(inviteLink, chatId, false).Result;
                                            }
                                            else
                                            {
                                                coverage = ClientApiHandler.GetCoverageOfChannel(inviteLink, chatId, true).Result;
                                            }
                                        }
                                        catch
                                        {
                                            coverage = ClientApiHandler.GetCoverageOfChannel(inviteLink, chatId, true).Result;
                                        }
                                    }

                                    if (coverage > 1500)
                                    {
                                        dbContext.Channels.Add(new DbEntities.Channel { Price = 0, Coverage = coverage, Name = update.Message.ForwardFromChat.Title, Date = DateTime.UtcNow, Channel_Id = chatId, Link = inviteLink, Subscribers = await botClient.GetChatMembersCountAsync(update.Message.ForwardFromChat.Id), User_Id = update.Message.From.Id });
                                        dbContext.SaveChanges();
                                        await botClient.SendTextMessageAsync(update.Message.From.Id, "OK! Канал добавлен :)\n*Что бы вы могли продать рекламу в своём канале - не забудьте выставить в настройках канала CPM и добавить рекламные места*", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Назад в меню продаж", CallbackData = "/sellMenuP0" }), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                                    }
                                    else
                                    {
                                        await botClient.SendTextMessageAsync(update.Message.From.Id, "Канал не добавлен, т.к. среднесуточный охват в нем менее 1500.", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Назад в меню продаж", CallbackData = "/sellMenuP0" }), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    if (ex.Message.Equals("INVITE_HASH_EXPIRED"))
                                    {
                                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "eAdvertisement_bot helper не может присоединиться к каналу, попробуйте сделать это вручную.\n@eAdvertisement_Helper\n" +
                                            "Также такое может происходить из-за высокой нагрузки на @eAdvertisement_Helper . Попробуйте позже.\n\n" + inviteLink);
                                    }
                                    else
                                    {
                                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, ex.StackTrace + "\n" + ex.Message +"\n");
                                    }
                                }
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Извините, но бот не имеет всех нужных разрешений \nв этом канале для того чтобы работать");
                            }
                        }
                        if (!isBotAdmin)
                        {
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Извините, но бот не является администратором этого канала");
                        }
                        if (!isUserACreator)
                        {
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Вы не создатель этого канала");
                        }
                    }
                    else
                    {
                        if (chInDb.User_Id == update.Message.From.Id)
                        {
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Этот канал уже привязан к вам", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Назад в меню продаж", CallbackData = "/sellMenuP0" }));
                        }
                        else if (!isUserACreator)
                        {
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Вы не создатель этого канала");
                        }
                        else if (chInDb.User_Id != update.Message.From.Id && isUserACreator)
                        {
                            chInDb.User_Id = update.Message.From.Id;
                            dbContext.SaveChanges();
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Этот канал был привязан не к вам, но мы исправили это!\nПоздравляем с новым каналом! :)", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Назад в меню продаж", CallbackData = "/sellMenuP0" }));
                        }
                    }
                }
                else if (userStateId == 3)
                {
                    DbEntities.Channel channel = dbContext.Channels.Find(chatId);
                    string info = "";
                    if (channel != null)
                    {
                        info = "[" + channel.Name + "](" + channel.Link + ")" +
                         "\nПодписчиков: " + channel.Subscribers +
                         "\nОхватов: " + channel.Coverage +
                         "\nERR: " + Math.Round(Convert.ToDouble(channel.Coverage) / Convert.ToDouble(channel.Subscribers), 2) +
                         "\nЦена: " + channel.Price +
                         "\nCPM: " + channel.Cpm;
                        if (channel.Description != null && !channel.Description.Equals(""))
                        {
                            info += "\n*Описание*\n" + channel.Description;
                        }
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Канал не найден", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Назад", CallbackData = "/manualPurchaseMenuP0IIS1,2C" }), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, disableWebPagePreview: true);
                        return;
                    }


                    InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[1][];

                    keyboard[0] = new[]
                    {
                        new InlineKeyboardButton { Text = "Купить место", CallbackData = "/showPlacesCalendarForBuyerN"+channel.Channel_Id+"T0IIS1,2C"},
                        new InlineKeyboardButton { Text = "Назад", CallbackData = "/manualPurchaseMenuP0IIS1,2C"},
                    };

                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, info, replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, disableWebPagePreview: true);

                    try
                    {
                        await botClient.DeleteMessageAsync(update.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                    }
                    catch
                    {}
                }
                else if (userStateId == 5)
                {
                    DbEntities.Channel channel;
                    try
                    {
                       channel = dbContext.Channels.Find(chatId);
                    }
                    catch
                    {
                        await botClient.SendTextMessageAsync(update.Message.From.Id, "Этот канал не прикреплен к боту");
                        return;
                    }
                    if (channel.User_Id == update.Message.From.Id)
                    {
                        await botClient.SendTextMessageAsync(update.Message.From.Id, "Вы не можете купит рекламу в этом канале, потому что это ваш канал");
                        return;
                    }
                    DbEntities.User user = dbContext.Users.Find(Convert.ToInt64(update.Message.From.Id));

                    try
                    {
                        List<DbEntities.Autobuy_Channel> abcs = dbContext.Autobuy_Channels.Where(a => a.Autobuy_Id == Convert.ToInt32(user.Object_Id)).ToList();
                        if (!abcs.Select(ac => ac.Channel_Id).Contains(chatId))
                        {
                            if (dbContext.Autobuys.Find(Convert.ToInt32(user.Object_Id)).Autobuy_Channels != null)
                            {
                                dbContext.Autobuys.Find(Convert.ToInt32(user.Object_Id)).Autobuy_Channels.Add(new DbEntities.Autobuy_Channel { Autobuy_Id = Convert.ToInt32(user.Object_Id), Channel_Id = channel.Channel_Id });
                            }
                            else
                            {
                                dbContext.Autobuys.Find(Convert.ToInt32(user.Object_Id)).Autobuy_Channels = new List<DbEntities.Autobuy_Channel> { new DbEntities.Autobuy_Channel { Autobuy_Id = Convert.ToInt32(user.Object_Id), Channel_Id = channel.Channel_Id } };
                            }
                            dbContext.SaveChanges();
                            await botClient.SendTextMessageAsync(update.Message.From.Id, "Добавлние успешно", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Показать обновленное меню!", CallbackData = "acstabP0" }));

                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(update.Message.From.Id, "Этот канал уже прикреплен", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Назад", CallbackData = "acstabP0" }));

                        }
                    }
                    catch (Exception ex)
                    {
                        MainLogger.LogException(ex, "On905InStateEvent");

                    }


                }
            }
            catch (Exception ex)
            {
                MainLogger.LogException(ex, "On905StateEvent");
            }
            finally { dbContext.Dispose(); }
        }
    }
}
