using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace eAdvertisement_bot.Models.DbEntities
{
    [Table("place")]
    public class Place
    {
        [Key, Column("time")]
        public TimeSpan Time { get; set; }  // Check this type!

        [Column("channel_id")]
        public long Channel_Id { get; set; }
        public Channel Channel { get; set; }

        [Column("place_id")]
        public int Place_Id { get; set; }
    }
}
