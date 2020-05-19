using eAdvertisement_bot.Logger;
using eAdvertisement_bot.Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TeleSharp.TL;
using TeleSharp.TL.Channels;
using TeleSharp.TL.Messages;
using TLSharp.Core;
using TLSharp.Core.Network;

namespace eAdvertisement_bot
{
    public static class ClientApiHandler
    {

        // Client API part

        //var hash = await client.SendCodeRequestAsync("+380509400345");
        //var code = "10910";
        //var user = await client.MakeAuthAsync("+380509400345", hash, code);


        // Dialogs snapshot can't be older than 1 minute, it's made to decrease pressure on project

        public static TLDialogs DialogsSnapshot { get; set; }
        public static long UpdateTicks { get; set; }
        public async static Task<TLDialogs> UpdateDialogsSnapshot()
        {
            if (DateTime.Now.Ticks - UpdateTicks > TimeSpan.TicksPerMinute*2)
            {
                UpdateTicks = DateTime.Now.Ticks;
                DialogsSnapshot = (TLDialogs) await Client.GetUserDialogsAsync();
            }
            return DialogsSnapshot;
        }
        //

        public static int Api_Id { get; set; }
        public static string Api_Hash { get; set; }
        public static int Client_Id { private set; get; } 	

        public static TelegramClient Client { get; set; }
        static ClientApiHandler()
        {
            Api_Id = 1026352;
            Api_Hash = "a5913624290fc8ba734d33597d39ad87";
        }

        public static async Task<int> GetCoverageOfPost(int messageId, long channelId)
        {
            channelId = Math.Abs(1000000000000 + channelId);  // I don't know why, but it's all right
            var dialogs = UpdateDialogsSnapshot().Result;

            foreach (var element in dialogs.Chats)
            {
                if (element is TLChannel && ((TLChannel)element).Id == channelId)
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

                    TLMessage post = realMessages.FirstOrDefault(rm => rm.Id == messageId);
                    int coverage = post == null ? 0 : Convert.ToInt32(post.Views);
                    return coverage;
                }
            }
            return 0;
        }
        public static async Task<int> GetCoverageOfChannel(string inviteLink, long channelId, bool isNewChannel)
        {

            if (isNewChannel)
            {
                string grhash = inviteLink.Trim().Replace("https://t.me/joinchat/", "").Replace("/", "");

                TLRequestImportChatInvite RCHI = new TLRequestImportChatInvite();
                RCHI.Hash = grhash;
                TLUpdates chatInstance = await Client.SendRequestAsync<TLUpdates>(RCHI);
            }

            channelId = Math.Abs(1000000000000 + channelId);  // I don't know why, but it's all right
            var dialogs = UpdateDialogsSnapshot().Result;       // LOOOOK HERE BROOOOOOOOOOOOOOOOOOOO


            foreach (var element in dialogs.Chats)
            {
                if (element is TLChannel && ((TLChannel)element).Id == channelId)
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
                        else if (msg is TLMessageService)
                            continue;
                    }

                    long unixTimeNow = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                    List<TLMessage> realMessagesAY = realMessages.Where(r => unixTimeNow - r.Date < 259200 && unixTimeNow - r.Date > 172800).ToList();
                    List<TLMessage> realMessagesY = realMessages.Where(r => unixTimeNow - r.Date < 172800 && unixTimeNow - r.Date > 86400).ToList();

                    double ay = realMessagesAY.Select(r => r.Views).Min() * 0.87 ?? 0;
                    double y = realMessagesY.Select(r => r.Views).Min() ?? 0;
                    int coverage = Convert.ToInt32((ay + y) / 2);
                    return coverage;
                }
            }

            return 0;

        }

        public static long ConvertToUnixTime(DateTime datetime)
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (long)(datetime - sTime).TotalSeconds;
        }

        public static async Task<bool> IsWorkingPostOk(List<int> messageIds, Advertisement ad)
        {
            DateTime now = DateTime.Now;
            long channelId = Math.Abs(1000000000000 + ad.Channel_Id);  // I don't know why, but it's all right
            var dialogs = UpdateDialogsSnapshot().Result;

            List<TLMessage> realMessages = new List<TLMessage>();

            foreach (var element in dialogs.Chats)
            {
                if (element is TLChannel && ((TLChannel)element).Id == channelId)
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
                        Limit = Convert.ToInt32(60*(Convert.ToDouble(ad.Alive)/24))+10,    // toWork and abt the same to ServiceMessages
                    });
                    var msgs = res.Messages;

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
                    break;
                }
            }
            realMessages = realMessages.Where(m => ConvertToUnixTime(now) - m.Date > 300).ToList(); // Pick posts that are posted not less than 5 min ago
            List<TLMessage> goodMessages = realMessages.Where(m=>messageIds.Contains(m.Id)).ToList();
            
            foreach(TLMessage msg in goodMessages)  // Check for editing post
            {
                if (msg.EditDate != null)
                {
                    return false;
                }
            }
            if (goodMessages.Count==0 || goodMessages.Count != ad.AdMessages.Count)  // If one of the messages is deleted not by bot
            {
                return false;
            }
            TLMessage lastMessage = realMessages.FirstOrDefault(m => m.Date == realMessages.Max(m => m.Date));
            TLMessage messageToCheck = goodMessages.FirstOrDefault(m => m.Date == goodMessages.Max(m => m.Date));
            if (now.Ticks - ad.Date_Time.Ticks < TimeSpan.TicksPerHour * ad.Top) // Top check
            {
                if (messageToCheck!= null && lastMessage.Id == messageToCheck.Id)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else // Alive check
            {
                if (messageToCheck != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }
        private static DateTime ConvertFromUnixTime(Double TimestampToConvert, bool Local)
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
        public static async Task SetClientId()
        {
            if (Client_Id == 0)
            {
                var rq = new TeleSharp.TL.Users.TLRequestGetFullUser { Id = new TLInputUserSelf() };
                TLUserFull rUserSelf = await Client.SendRequestAsync<TLUserFull>(rq);
                TLUser userSelf = (TLUser)rUserSelf.User;
                Client_Id = userSelf.Id;
            }
        }
        public static async Task ConnectClient()
        {

            Client = new TelegramClient(Api_Id, Api_Hash);
            await Client.ConnectAsync();

        }

    }
}
