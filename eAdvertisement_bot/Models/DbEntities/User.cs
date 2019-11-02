﻿using System;
using System.Collections.Generic;
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

        [Column("publications_limit")]
        public int Publications_Limit { get; set; }

        [Column("autobuys_limit")]
        public int Autobuys_Limit { get; set; }

        [Column("ban")]
        public bool Ban { get; set; }

        [Column("firstname")]
        public string FirstName { get; set; }

        [Column("lastname")]
        public string LastName { get; set; }

        // One to Many relationship lists
        public List<Input> Inputs { get; set; }
        public List<Advertisement> Advertisements { get; set; }
        public List<Autobuy> Autobuys { get; set; }
        public List<Publication> Publications { get; set; }
        public List<Channel> Channels { get; set; }

        // Many to Many relationship lists


        // Some logic
        public bool IsBanned()
        {
            return this.Ban;
        }
    }
}