using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace eAdvertisement_bot.Models.DbEntities
{
    [Table("advertisement")]
    public class Advertisement
    {
        [Column("advertisement_id"), Key]
        public int Advertisement_Id { get; set; }

        [Column("channel_id")]
        public long Channel_Id { get; set; }
        public Channel Channel { get; set; }



        [Column("date_time")]
        public DateTime Date_Time { get; set; }

        [Column("top")]
        public int Top { get; set; }

        [Column("alive")]
        public int Alive { get; set; }

        [Column("publication_snapshot")]
        public string Publication_Snapshot { get; set; }

        [Column("price")]
        public int Price { get; set; }

        [Column("user_id")]
        public long User_Id { get; set; }
        public User User { get; set; }

        [Column("autobuy_id")]
        public int? Autobuy_Id { get; set; }
        public Autobuy Autobuy { get; set; }

        [Column("advertisement_status_id")]
        public int Advertisement_Status_Id { get; set; }
        public Advertisement_Status Advertisement_Status { get; set; }

        // One to Many
        public List<AdMessage> AdMessages { get; set; }
    }
}
