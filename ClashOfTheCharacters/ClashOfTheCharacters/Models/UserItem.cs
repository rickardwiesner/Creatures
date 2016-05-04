using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.Models
{
    public class UserItem
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int ItemId { get; set; }
        public virtual Item Item { get; set; }

        public int Quantity { get; set; }

        public bool InBag { get; set; }

        public int Worth { get { return Convert.ToInt32(Item.Price / 2); } }
    }
}