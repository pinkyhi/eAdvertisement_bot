using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace eAdvertisement_bot.Models.DbEntities
{
    [Table("advertisement_status")]
    public class Advertisement_Status
    {
        [Key, Column("advertisement_status_id")]
        public int Advertisement_Status_Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }
    }
}
