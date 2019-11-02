using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace eAdvertisement_bot.Models.DbEntities
{
    [Table("channel_category")]
    public class Channel_Category
    {
        [Key, Column("channel_id")]
        public long Channel_Id { get; set; }
        public Channel Channel { get; set; }

        [Key, Column("category_id")]
        public int Category_Id { get; set; }
        public Category Category { get; set; }
    }
}
