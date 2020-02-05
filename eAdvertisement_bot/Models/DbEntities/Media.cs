using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace eAdvertisement_bot.Models.DbEntities
{
    [Table("media")]
    [Serializable]
    public class Media
    {
        [Key, Column("media_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Media_Id { get; set; }

        [Column("publication_id")]
        public int Publication_Id { get; set; }

        [JsonIgnore]
        public Publication Publication { get; set; }

        [Column("path")]
        public string Path { get; set; }
    }
}
