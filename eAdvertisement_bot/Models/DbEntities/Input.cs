using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace eAdvertisement_bot.Models.DbEntities
{
    [Table("input")]
    public class Input
    {
        [Key, Column("input_id")]
        public int Input_Id { get; set; }

        [Column("user_id")]
        public int User_Id { get; set; }
        public User User { get; set; }

        [Column("sum")]
        public double Sum { get; set; }

        [Column("inputed")]
        public int Inputed { get; set; }

        [Column("description")]
        public string Description { get; set; }
    }
}
