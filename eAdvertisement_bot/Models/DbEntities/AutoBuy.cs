using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace eAdvertisement_bot.Models.DbEntities
{
    [Table("autobuy")]
    public class Autobuy
    {
        [Key, Column("autobuy_id")]
        public int Autobuy_Id { get; set; }

        [Column("publication_snapshot")]
        public string Publication_Snapshot { get; set; }

        [Column("interval")]
        public int Interval { get; set; }

        [Column("min_price")]
        public int Min_Price { get; set; }

        [Column("max_price")]
        public int Max_Price { get; set; }

        [Column("max_cpm")]
        public int Max_Cpm { get; set; }

        [Column("user_id")]
        public long User_Id { get; set; }
        public User User { get; set; }

        // Many To Many relationship lists
        public List<Autobuy_Channel> Autobuy_Channels { get; set; } // Channels where advertisment post have to be sent
    }
}
