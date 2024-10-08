﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TeleSharp.TL;
using TeleSharp.TL.Channels;
using TeleSharp.TL.Messages;
using TLSharp.Core;

namespace eAdvertisement_bot
{
    public class ClientApiHandler
    {

        // Client API part

        //var hash = await client.SendCodeRequestAsync("+380509400345");
        //var code = "10910";
        //var user = await client.MakeAuthAsync("+380509400345", hash, code);


        public int Api_Id { get; set; }
        public string Api_Hash { get; set; }
        public int Client_Id { private set; get; }
        public TelegramClient Client { get; set; }
        public ClientApiHandler()
        {
            Api_Id = 1026352;
            Api_Hash = "a5913624290fc8ba734d33597d39ad87";
        }
        public ClientApiHandler(int apiId, string apiHash)
        {
            Api_Id = apiId;
            Api_Hash = apiHash;
        }
        public async Task<int> GetCoverageOfChannel(string inviteLink, long channelId, bool isNewChannel)
        {
            if (isNewChannel)
            {
                string grhash = inviteLink.Trim().Replace("https://t.me/joinchat/", "").Replace("/", "");

                TLRequestImportChatInvite RCHI = new TLRequestImportChatInvite();
                RCHI.Hash = grhash;
                TLUpdates chatInstance = await Client.SendRequestAsync<TLUpdates>(RCHI);
            }
            try
            {
                channelId = Math.Abs(1000000000000+channelId);  // I don't know why, but it's all right
                var dialogs = (TLDialogs)await Client.GetUserDialogsAsync();

                
                foreach (var element in dialogs.Chats)
                {
                    if (element is TLChannel && ((TLChannel)element).Id==channelId)
                    {
                        TLChannel channel = element as TLChannel;
                        var chan = await Client.SendRequestAsync<TeleSharp.TL.Messages.TLChatFull>(new TLRequestGetFullChannel()
                        {
                            Channel = new TLInputChannel()
                            { ChannelId = channel.Id, AccessHash = (long)channel.AccessHash }
                        });
                        TLInputPeerChannel inputPeer = new TLInputPeerChannel()
                        { ChannelId = channel.Id, AccessHash = (long)channel.AccessHash };

                        TLChannelMessages res = await Client.SendRequestAsync<TLChannelMessages>
                        (new TLRequestGetHistory()
                        {
                            Peer = inputPeer,
                            Limit = 140,    // 70 toWork and abt the same to ServiceMessages
                        });
                        var msgs = res.Messages;

                        List<TLMessage> realMessages = new List<TLMessage>();
                        foreach (var msg in msgs)
                        {
                            if (msg is TLMessage)
                            {
                                TLMessage sms = msg as TLMessage;
                                realMessages.Add(sms);
                            }
                            if (msg is TLMessageService)
                                continue;
                        }
                        
                        long unixTimeNow = (Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                        for (int i = 0; i < realMessages.Count; i++)
                        {
                            if(realMessages[i].Date<unixTimeNow-172800|| realMessages[i].Date > unixTimeNow - 84600)
                            {
                                realMessages.RemoveAt(i);
                            }
                        }
                        int min = Convert.ToInt32(realMessages.Min(c => c.Views));
                        int average = Convert.ToInt32(realMessages.Average(c => c.Views));
                        int coverage = (((min+average)/2)+min)/2;  // (((min+average)/2)+average)/2;
                        return coverage;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// Returns false if post was interrupted or deleted during top time
        /// </summary>
        /// <param name="channelID">Channel ID</param>
        /// <param name="postID">Post id</param>
        /// <param name="topTime"> Time, during which post mustn't be interrupted </param>
        /// <returns>Returns false if post was interrupted during top time</returns>
        public async /*double*/ Task<Boolean> CheckPostTop(long channelID, int postID,TimeSpan topTime)
        {
            if (TimeSpan.Compare(topTime, new TimeSpan(hours: 1, minutes: 0, seconds: 0)) == 0)
                topTime.Subtract(new TimeSpan(0, 1, 0));
            try
            {

                TLDialogs dialogs = (TLDialogs)await Client.GetUserDialogsAsync();
                TLChannel channel = (TLChannel)dialogs.Chats.First(x => x is TLChannel && ((TLChannel)x).Id == channelID);
                TLInputPeerChannel peer = new TLInputPeerChannel() { ChannelId = channel.Id, AccessHash = (long)channel.AccessHash };
                var messages = (await Client.SendRequestAsync<TLChannelMessages>(new TLRequestGetHistory()
                {
                    Peer = peer,
                    Limit = 25
                })).Messages;
                TLMessage msg = (TLMessage)messages.Where(i => i is TLMessage).First(j => ((TLMessage)j).Id == postID);

                int maxID = ((TLMessage)messages.Where(i => i is TLMessage).OrderByDescending(x => ((TLMessage)x).Id).First()).Id;

                if (maxID == postID)
                {
                    return true;
                }
                else
                {
                    for (int i = postID + 1; i < maxID; i++)
                    {
                        if (messages.Where(j => j is TLMessage).Any(x => ((TLMessage)x).Id == i))
                        {
                            TLMessage nextMsg = (TLMessage)messages.Where(i => i is TLMessage).First(x => ((TLMessage)x).Id == i);
                            TimeSpan postTopTime = ConvertFromUnixTime((double)nextMsg.Date).Subtract(ConvertFromUnixTime((double)msg.Date));

                            if (postTopTime >= topTime) return true;
                            else return false;
                        }
                    }
                }
                    //foreach( long i in Enumerable.Range(postID+1, maxID - postID)) 
                    
                
            }
            // Usually shows up in line msg initialization, when post with 
            // definite id is deleted()
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Post is deleted => {channelID} : {postID}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return true;
            }
            return true;
        }
        /// <summary>
        /// Returns false if post was interrupted or deleted during top time
        /// </summary>
        /// <param name="channelID">Channel ID</param>
        /// <param name="postID">Post id</param>
        /// <param name="topTime"> Time, during which post mustn't be interrupted </param>
        /// <returns>Returns false if post was interrupted during top time</returns>
        public async Task<Boolean> CheckPostTop(long channelID, List<int> postIDs, TimeSpan topTime)
        {
            TLDialogs dialogs = (TLDialogs)await Client.GetUserDialogsAsync();
            TLChannel channel = (TLChannel)dialogs.Chats.First(x => x is TLChannel && ((TLChannel)x).Id == channelID);
            TLInputPeerChannel peer = new TLInputPeerChannel() { ChannelId = channel.Id, AccessHash = (long)channel.AccessHash };
            var messages = (await Client.SendRequestAsync<TLChannelMessages>(new TLRequestGetHistory()
            {
                Peer = peer,
                Limit = 20
            })).Messages;
            List<TLMessage> advertisements = new List<TLMessage>();
            // Gets biggest ID out of all ID's
            int postId = postIDs.OrderByDescending(x => x).First();
            // Gets last message ID in channel
            int maxID = ((TLMessage)messages.Where(i => i is TLMessage).OrderByDescending(x => ((TLMessage)x).Id).First()).Id;

            try
            {
                foreach (int postID in postIDs)
                {
                    advertisements.Add((TLMessage)messages.Where(i => i is TLMessage).First(j => ((TLMessage)j).Id == postID));
                }
                //edited check:
                foreach( TLMessage post in advertisements)
                {
                    if (post.EditDate != null)
                    {
                        return false;
                    }
                }

                if (postId == maxID) return true;
                else
                {
                    for (int i = postId + 1; i < maxID; i++)
                    {
                        if (messages.Where(j => j is TLMessage).Any(x => ((TLMessage)x).Id == i))
                        {
                            TLMessage nextMsg = (TLMessage)messages.Where(i => i is TLMessage).First(x => ((TLMessage)x).Id == i);
                            TimeSpan postTopTime = ConvertFromUnixTime((double)nextMsg.Date).
                                          Subtract(ConvertFromUnixTime((double)advertisements.Where(x => ((TLMessage)x).Id == postId).First().Date));

                            if (postTopTime >= topTime) return true;
                            else return false;
                        }
                    }
                    throw new NullReferenceException("Last message not found");
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Post is deleted => {channelID} : {postId}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Checks if post is alive
        /// </summary>
        /// <param name="channelID"></param>
        /// <param name="postID"></param>
        /// <returns>Returns true if post is alive</returns>
        public async Task<Boolean> AliveCheck(long channelID, int postID)
        {
            try
            {

                TLDialogs dialogs = (TLDialogs)await Client.GetUserDialogsAsync();
                TLChannel channel = (TLChannel)dialogs.Chats.First(x => x is TLChannel && ((TLChannel)x).Id == channelID);
                TLInputPeerChannel peer = new TLInputPeerChannel() { ChannelId = channel.Id, AccessHash = (long)channel.AccessHash };
                var messages = (await Client.SendRequestAsync<TLChannelMessages>(new TLRequestGetHistory()
                {
                    Peer = peer,
                    Limit = 25
                })).Messages;
                TLMessage msg = (TLMessage)messages.Where(i => i is TLMessage).First(j => ((TLMessage)j).Id == postID);
            }
            catch(Exception ex)
            {
                return false;
            }
            return true;
        }

        public static long ConvertToUnixTime(DateTime datetime)
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (long)(datetime - sTime).TotalSeconds;
        }

        private static DateTime ConvertFromUnixTime(Double TimestampToConvert, bool Local = false)
        {
            var mdt = new DateTime(1970, 1, 1, 0, 0, 0);
            if (Local)
            {
                return mdt.AddSeconds(TimestampToConvert).ToLocalTime();
            }
            else
            {
                return mdt.AddSeconds(TimestampToConvert);
            }
        }
        public async Task SetClientId()
        {
            if (Client_Id == 0)
            {
                var rq = new TeleSharp.TL.Users.TLRequestGetFullUser { Id = new TLInputUserSelf() };
                TLUserFull rUserSelf = await Client.SendRequestAsync<TLUserFull>(rq);
                TLUser userSelf = (TLUser)rUserSelf.User;
                Client_Id = userSelf.Id;
            }
        }
        public async Task ConnectClient()
        {
            if(Client == null || Client.IsConnected == false)
            {
                Client = new TelegramClient(Api_Id, Api_Hash);
                await Client.ConnectAsync();
            }
        }
    }
}


//     maxID<postID       postID    \/     topTime    \/
//--------------------------|-----------------|--------------->t