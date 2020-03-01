using System;
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
        /// Returns false if post was interrupted during top time
        /// </summary>
        /// <param name="channelID">Channel ID</param>
        /// <param name="postID">Post id</param>
        /// <param name="topTime"> Time, during which post mustn't be interrupted </param>
        /// <returns>Returns false if post was interrupted during top time</returns>
        public async /*double*/ Task<Boolean> CheckPostTop(long channelID, long postID,TimeSpan topTime)
        {
            bool /*double*/ coef = true;
            long tempId = postID + 1;
            channelID = Math.Abs(1000000000000 + channelID);
            try
            {

                TLDialogs dialogs = (TLDialogs) await Client.GetUserDialogsAsync();
                TLChannel channel = (TLChannel)dialogs.Chats.First(x => x is TLChannel && ((TLChannel)x).Id == channelID);
                TLInputPeerChannel peer = new TLInputPeerChannel() { ChannelId = channel.Id, AccessHash = (long)channel.AccessHash };
                var messages = (await Client.SendRequestAsync<TLChannelMessages>(new TLRequestGetHistory()
                    {
                       Peer = peer,
                        Limit = 100
                    })).Messages;

                TLMessage msg = (TLMessage)messages.Where(i => i is TLMessage).First(j => ((TLMessage)j).Id == postID);
                TLMessage nextMsg = (TLMessage)messages.Where(i => i is TLMessage).First(j => ((TLMessage)j).Id == tempId);

                if (msg is null) return false;
                if (nextMsg is null)
                    while (nextMsg.Id != ((TLMessage)messages.Where(i => i is TLMessage).OrderByDescending(x => ((TLMessage)x).Id).First()).Id)
                    {
                        tempId += 1;
                        nextMsg = (TLMessage)messages.Where(i => i is TLMessage).First(j => ((TLMessage)j).Id == tempId);
                        if (nextMsg is null)
                            return true;
                    }
                else
                {
                    // a.Subract(b) => a - b
                    //TimeSpan hour = new TimeSpan(hours: 0, minutes: 59, seconds: 0); - default time is hour
                    TimeSpan postTopTime = ConvertFromUnixTime((double)nextMsg.Date).Subtract(ConvertFromUnixTime((double)msg.Date));

                    int index = TimeSpan.Compare(postTopTime, topTime);
                    if (index > 0 || index == 0) return true;
                    else return false;

                    // 1 - 59 minutes
                    // x - timeDiff.Minutes
                    // convert all to ticks to calculate coeff:
                    //coef = (double)postTime.Ticks / hour.Ticks;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return true;
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
            var rq = new TeleSharp.TL.Users.TLRequestGetFullUser { Id = new TLInputUserSelf() };
            TLUserFull rUserSelf = await Client.SendRequestAsync<TLUserFull>(rq);
            TLUser userSelf = (TLUser)rUserSelf.User;
            Client_Id = userSelf.Id;
        }
        public async Task ConnectClient()
        {
            Client = new TelegramClient(Api_Id, Api_Hash);
            await Client.ConnectAsync();
        }
    }
}
