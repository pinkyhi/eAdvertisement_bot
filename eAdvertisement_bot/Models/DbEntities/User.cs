using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace eAdvertisement_bot.Models.DbEntities
{
    [Table("user")]
    public class User
    {
        [Key, Column("user_id")]
        public long User_Id { get; set; }

        [Column("phone")]
        public string Phone { get; set; }

        [Column("nickname")]
        public string Nickname { get; set; }

        [Column("balance")]
        public int Balance { get; set; }
        [Column("object_id")]
        public long Object_Id { get; set; }

        [Column("ban", TypeName = "bit")]
        [DefaultValue(false)]
        public bool Ban { get; set; }

        [Column("firstname")]
        public string FirstName { get; set; }

        [Column("lastname")]
        public string LastName { get; set; }

        [Column("language")]
        public string Language { get; set; }

        [Column("tag")]
        public string Tag { get; set; }

        [Column("stopped", TypeName = "bit")]
        [DefaultValue(false)]
        public bool Stopped { get; set; }

        [Column("user_state_id")]
        public long User_State_Id { get; set; }
        public User_State User_State { get; set; }

        // One to Many relationship lists
        public List<Input> Inputs { get; set; }
        public List<Advertisement> Advertisements { get; set; }
        public List<Autobuy> Autobuys { get; set; }
        public List<Publication> Publications { get; set; }
        public List<Channel> Channels { get; set; }

        // Many to Many relationship lists


    }
}
