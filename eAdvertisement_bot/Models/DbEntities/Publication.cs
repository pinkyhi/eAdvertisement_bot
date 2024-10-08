﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace eAdvertisement_bot.Models.DbEntities
{
    [Table("publication")]
    [Serializable]
    public class Publication
    {
        [Key, Column("publication_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Publication_Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("user_id")]
        public long? User_Id { get; set; }
        [JsonIgnore]
        public User User { get; set; }

        [Column("text")]
        public string Text { get; set; }

        // One to Many relationships lists
        public List<Media> Media { get; set; }
        public List<Button> Buttons { get; set; }
        
    }
}
