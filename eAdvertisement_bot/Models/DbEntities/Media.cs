using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace eAdvertisement_bot.Models.DbEntities
{
    [Table("media")]
    public class Media
    {
        [Key, Column("media_id")]
        public int Media_Id { get; set; }

        [Column("publication_id")]
        public int Publication_Id { get; set; }
        public Publication Publication { get; set; }

        [Column("path")]
        public string Path { get; set; }
    }
}
