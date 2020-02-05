using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace eAdvertisement_bot.Models.DbEntities
{
    [Table("button")]
    [Serializable]
    public class Button
    {
        [Key, Column("button_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Button_Id { get; set; }

        [Column("publication_id")]
        public int Publication_Id { get; set; }

        [JsonIgnore]
        public Publication Publication { get; set; }

        [Column("text")]
        public string Text { get; set; }
        [Column("url")]
        public string Url { get; set; }
    }
}
