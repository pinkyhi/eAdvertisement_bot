using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace eAdvertisement_bot.Models.DbEntities
{
    [Table("user_state")]
    public class User_State
    {
        [Key, Column("user_state_id")]
        public int User_State_Id { get; set; }

        [Column("state")]
        public string State { get; set; }
    }
}
