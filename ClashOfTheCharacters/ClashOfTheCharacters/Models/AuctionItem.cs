using ClashOfTheCharacters.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.Models
{
    public class AuctionItem
    {
        public int Id { get; set; }

        public int StartPrice { get; set; }

        public int? CurrentBid { get; set; }

        public int? BuyoutPrice { get; set; }

        public DateTimeOffset TimeRemaining { get; set; }

        public int UserItemId { get; set; }
        public virtual UserItem UserItem { get; set; }

        public string OwnerId { get; set; }
        public virtual ApplicationUser Owner { get; set; }

        public string CurrentBidderId { get; set; }
        public virtual ApplicationUser CurrentBidder { get; set; }
    }
}