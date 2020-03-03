using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eAdvertisement_bot.Models.DbEntities
{
    [Table("admessage")]
    public class AdMessage
    {
        [Column("admessage_id"), Key]
        public int AdMessage_Id { get; set; }

        [Column("advertisement_id")]
        public int Advertisement_Id { get; set; }
        public Advertisement Advertisement { get; set; }


    }
}
