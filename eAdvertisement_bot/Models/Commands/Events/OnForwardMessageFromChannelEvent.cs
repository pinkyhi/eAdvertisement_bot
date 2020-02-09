using eAdvertisement_bot.DAO;
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
                                ClientApiHandler cah = new ClientApiHandler();
                                await cah.ConnectClient();
                                await cah.SetClientId();

                                if ((await botClient.GetChatAsync(chatId)).InviteLink == null)
                                {
                                    await botClient.ExportChatInviteLinkAsync(update.Message.ForwardFromChat.Id);
                                }

                                string inviteLink = (await botClient.GetChatAsync(chatId)).InviteLink;

                                try
                                {
                                    Chat s = await botClient.GetChatAsync(chatId);
                                    try
                                    {
                                        if ((await botClient.GetChatMemberAsync(chatId, cah.Client_Id)).Status == Telegram.Bot.Types.Enums.ChatMemberStatus.Member)
                                        {
                                            coverage = await cah.GetCoverageOfChannel(inviteLink, chatId, false);
                                        }
                                    }
                                    catch
                                    {
                                        coverage = await cah.GetCoverageOfChannel(inviteLink, chatId, true);
                                    }

                                    /* Old Version
                                     *
                                    if ((await botClient.GetChatMemberAsync(chatId, cah.Client_Id)).Status != Telegram.Bot.Types.Enums.ChatMemberStatus.Member)
                                    {
                                        coverage = await cah.GetCoverageOfChannel(inviteLink,chatId,true);
                                    }
                                    else
                                    {
                                        coverage = await cah.GetCoverageOfChannel(inviteLink, chatId, false);
                                    }
                                    */
                                    dbContext.Channels.Add(new DbEntities.Channel { Price = 0, Coverage = coverage, Name = update.Message.ForwardFromChat.Title, Date = DateTime.UtcNow, Channel_Id = chatId, Link = inviteLink, Subscribers = await botClient.GetChatMembersCountAsync(update.Message.ForwardFromChat.Id), User_Id = update.Message.From.Id });
                                    dbContext.SaveChanges();
                                    await botClient.SendTextMessageAsync(update.Message.From.Id, "OK! Channel is added :)", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Back to sell menu", CallbackData = "/sellMenuP0" }));

                                }
                                catch (Exception ex)
                                {
                                    if (ex.Message.Equals("INVITE_HASH_EXPIRED"))
                                    {
                                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "eAdvertisement_bot helper couldn't join to channel, try to add it manually.\n@eAdvertisement_Helper\n" +
                                            "Also that can be because of high load on helper account, so just try again later.\n\n" + inviteLink);
                                    }
                                    else
                                    {
                                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, ex.Message);
                                    }
                                }
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Sorry, but bot hasn't all required permissions \nin this channel to work");
                            }
                        }
                        if (!isBotAdmin)
                        {
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Sorry, but bot isn't in administrators of this channel");
                        }
                        if (!isUserACreator)
                        {
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "You aren't a creator of this channel");
                        }
                    }
                    else
                    {
                        if (chInDb.User_Id == update.Message.From.Id)
                        {
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "This channel is already attached to you", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Back to sell menu", CallbackData = "/sellMenuP0" }));
                        }
                        else if (!isUserACreator)
                        {
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Sorry, but you aren't a creator of this channel");
                        }
                        else if (chInDb.User_Id != update.Message.From.Id && isUserACreator)
                        {
                            chInDb.User_Id = update.Message.From.Id;
                            dbContext.SaveChanges();
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "This channel was attached not to you, but we fixed it!\nCongratulations with a new channel! :)", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Back to sell menu", CallbackData = "/sellMenuP0" }));
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
                         "\nSubscribers: " + channel.Subscribers +
                         "\nCoverage: " + channel.Coverage +
                         "\nERR: " + Math.Round(Convert.ToDouble(channel.Coverage) / Convert.ToDouble(channel.Subscribers), 2) +
                         "\nPrice: " + channel.Price +
                         "\nCpm: " + channel.Cpm;
                        if (channel.Description != null && !channel.Description.Equals(""))
                        {
                            info += "\n*Description*\n" + channel.Description;
                        }
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Channel isn't found", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Back", CallbackData = "/manualPurchaseMenuP0IIS1,2C" }), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, disableWebPagePreview: true);
                        return;
                    }


                    InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[1][];

                    keyboard[0] = new[]
                    {
                        new InlineKeyboardButton { Text = "Buy place", CallbackData = "/showPlacesCalendarForBuyerN"+channel.Channel_Id+"T0IIS1,2C"},
                        new InlineKeyboardButton { Text = "Back", CallbackData = "/manualPurchaseMenuP0IIS1,2C"},
                    };

                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, info, replyMarkup: new InlineKeyboardMarkup(keyboard), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, disableWebPagePreview: true);

                    try
                    {
                        await botClient.DeleteMessageAsync(update.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                    }
                    catch (Exception ex)
                    {
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, ex.Message);
                    }
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
                        await botClient.SendTextMessageAsync(update.Message.From.Id, "This channel isn't attached to a bot");
                        return;
                    }
                    if (channel.User_Id == update.Message.From.Id)
                    {
                        await botClient.SendTextMessageAsync(update.Message.From.Id, "You can't add this channel because it's yours.");
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
                            await botClient.SendTextMessageAsync(update.Message.From.Id, "Adding was succesful", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Show updated menu!", CallbackData = "acstabP0" }));

                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(update.Message.From.Id, "This channel is already attached", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Back", CallbackData = "acstabP0" }));

                        }
                    }
                    catch (Exception ex)
                    {
                        await botClient.SendTextMessageAsync(update.Message.From.Id, ex.Message);
                        
                    }


                }
            }
            catch (Exception ex){ await botClient.SendTextMessageAsync(update.Message.From.Id, ex.Message); }
            finally { dbContext.Dispose(); }
        }
    }
}
