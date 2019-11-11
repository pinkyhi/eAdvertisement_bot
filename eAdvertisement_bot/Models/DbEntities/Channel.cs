using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace eAdvertisement_bot.Models.DbEntities
{
    [Table("channel")]
    public class Channel
    {
        [Key, Column("channel_id")]
        public long Channel_Id { get; set; }

        [Column("link")]
        public string Link { get; set; }

        [Column("subscribers")]
        public int? Subscribers { get; set; }

        [Column("coverage")]
        public int? Coverage { get; set; }

        [Column("cpm")]
        public int? Cpm { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("ban")]
        public bool Ban { get; set; }

        [Column("user_id")]
        public long User_Id { get; set; }
        public User User { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }


        //One to Many relationships
        public List<Advertisement> Advertisements { get; set; }


        // Many To Many relationship lists
        public List<Autobuy_Channel> Autobuy_Channels { get; set; }
        public List<Channel_Category> Channel_Categories { get; set; }
        public List<Place> Places { get; set; }


        // Some logic
        public bool IsBanned()
        {
            return this.Ban;
        }
    }

}
