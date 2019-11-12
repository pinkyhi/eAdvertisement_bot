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
            if(update.Type == Telegram.Bot.Types.Enums.UpdateType.Message && update.Message.ForwardFromChat.Type == Telegram.Bot.Types.Enums.ChatType.Channel)
            {
                int userStateId = 0;
                AppDbContext dbContext = new AppDbContext();
                try
                {
                    userStateId = dbContext.Users.First(u => u.User_Id == update.Message.From.Id).User_State_Id;
                    if (userStateId == 0)
                    {
                        return false;
                    }
                    if (userStateId == 1)
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
                userStateId = dbContext.Users.First(u => u.User_Id == update.Message.From.Id).User_State_Id;
                if (userStateId == 1)
                {
                    DbEntities.Channel chInDb = dbContext.Channels.Find(chatId);

                    
                    ChatMember[] admins = await botClient.GetChatAdministratorsAsync(chatId);
                    ChatMember botAsAChatMember = admins.First(a => a.User.Id == botId);
                    bool isBotAdmin = botAsAChatMember!=null;
                    bool isUserACreator = admins.First(a => a.User.Id == update.Message.From.Id).Status == Telegram.Bot.Types.Enums.ChatMemberStatus.Creator;

                    if (chInDb == null)
                    {
                        if (isBotAdmin && isUserACreator)
                        {
                            int coverage=0;
                            if(botAsAChatMember.CanDeleteMessages==true && botAsAChatMember.CanEditMessages==true && botAsAChatMember.CanPostMessages == true)
                            {
                                ClientApiHandler cah = new ClientApiHandler();
                                await cah.ConnectClient();
                                await cah.SetClientId();

                                if((await botClient.GetChatAsync(chatId)).InviteLink == null)
                                {
                                    await botClient.ExportChatInviteLinkAsync(update.Message.ForwardFromChat.Id);
                                }

                                string inviteLink = (await botClient.GetChatAsync(chatId)).InviteLink;
                                
                                try
                                {
                                    Chat s = await botClient.GetChatAsync(chatId);
                                    if ((await botClient.GetChatMemberAsync(chatId, cah.Client_Id)).Status != Telegram.Bot.Types.Enums.ChatMemberStatus.Member)
                                    {
                                        coverage = await cah.GetCoverageOfChannel(inviteLink,chatId,true);
                                    }
                                    else
                                    {
                                        coverage = await cah.GetCoverageOfChannel(inviteLink, chatId, false);
                                    }
                                    dbContext.Channels.Add(new DbEntities.Channel { Coverage=coverage, Name=update.Message.ForwardFromChat.Username, Date = DateTime.UtcNow, Channel_Id = chatId, Link = inviteLink, Subscribers = await botClient.GetChatMembersCountAsync(update.Message.ForwardFromChat.Id), User_Id = update.Message.From.Id });
                                    dbContext.SaveChanges();
                                    await botClient.SendTextMessageAsync(update.Message.From.Id, "OK! Channel is added :)", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Back to sell menu", CallbackData = "/backToSellMenu" }));
                                    
                                }
                                catch (Exception ex)
                                {
                                    if (ex.Message.Equals("INVITE_HASH_EXPIRED"))
                                    {
                                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "eAdvertisement_bot helper couldn't join to channel, try to add it manually.\n@eAdvertisement_Helper\n" +
                                            "Also that can be because of high load on helper account, so just try again later.\n\n"+inviteLink);
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
                        if(!isBotAdmin)
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
                        if(chInDb.User_Id == update.Message.From.Id)
                        {
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "This channel is already attached to you");
                        }
                        else if(!isUserACreator)
                        {
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Sorry, but you aren't a creator of this channel");
                        }
                        else if(chInDb.User_Id != update.Message.From.Id && isUserACreator)
                        {
                            chInDb.User_Id = update.Message.From.Id;
                            dbContext.SaveChanges();    
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "This channel was attached not to you, but we fixed it!\nCongratulations with a new channel! :)");
                        }
                    }
                }
            }
            catch (Exception ex){ Console.WriteLine(ex.Message); }
            finally { dbContext.Dispose(); }
        }
    }
}
