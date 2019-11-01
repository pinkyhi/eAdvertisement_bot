using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace eAdvertisement_bot.Models.DbEntities
{
    [Table("button")]
    public class Button
    {
        [Key, Column("button_id")]
        public int Button_Id { get; set; }

        [Column("publication_id")]
        public int Publication_Id { get; set; }
        public Publication Publication { get; set; }

        [Column("string")]
        public string String { get; set; }
    }
}
