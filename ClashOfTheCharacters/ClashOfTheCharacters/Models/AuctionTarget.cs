using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.Models
{
    public class AuctionTarget
    {
        public int Id { get; set; }

        public int AuctionCreatureId { get; set; }
        public virtual AuctionCreature AuctionCreature { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}