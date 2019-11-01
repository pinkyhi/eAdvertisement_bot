using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace eAdvertisement_bot.Models.DbEntities
{
    [Table("autobuy_channel")]
    public class Autobuy_Channel
    {
        [Key, Column("channel_id")]
        public int Channel_Id { get; set; }
        public Channel Channel { get; set; }

        [Key,Column("autobuy_id")]
        public int Autobuy_Id { get; set; }
        public Autobuy Autobuy { get; set; }
    }
}
