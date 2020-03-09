using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace eAdvertisement_bot.Models.DbEntities
{
    [Table("user_status")]
    public class User_Status
    {
        [Key, Column("user_status_id")]
        public long User_Status_Id { get; set; }

        [Column("name")]
        public string Name { get; set; }
        [Column("default_commission")]
        public double Default_Commision { get; set; }
    }
}
