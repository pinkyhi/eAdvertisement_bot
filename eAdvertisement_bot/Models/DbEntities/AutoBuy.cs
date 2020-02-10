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

        [Column("name")]
        public string Name { get; set; }

        [Column("interval")]
        public int Interval { get; set; }

        [Column("min_price")]
        public int Min_Price { get; set; }

        [Column("max_price")]
        public int Max_Price { get; set; }

        [Column("max_cpm")]
        public int Max_Cpm { get; set; }

        [Column("state")]
        public int State { get; set; }

        [Column("balance")]
        public int Balance { get; set; }

        [Column("daily_interval_from")]
        public TimeSpan Daily_Interval_From { get; set; }  // Check this type!

        [Column("daily_interval_to")]
        public TimeSpan Daily_Interval_To { get; set; }  // Check this type!

        [Column("user_id")]
        public long User_Id { get; set; }
        public User User { get; set; }

        // Many To Many relationship lists
        public List<Autobuy_Channel> Autobuy_Channels { get; set; } // Channels where advertisment post have to be sent
    }
}
