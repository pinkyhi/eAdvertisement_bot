using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace eAdvertisement_bot.Models.DbEntities
{
    [Table("category")]
    public class Category
    {
        [Key, Column("category_id")]
        public int Category_Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        // Many To Many relationship lists
        public List<Channel_Category> Channel_Categories { get; set; }

    }
}
