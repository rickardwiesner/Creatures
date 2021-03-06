﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.Models
{
    public class AuctionCreature
    {
        public int Id { get; set; }

        public int StartPrice { get; set; }

        public int? CurrentBid { get; set; }

        public int? BuyoutPrice { get; set; }

        public bool Sold { get { return OwnerId != UserCreature.UserId; } }

        public bool Finished { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public int UserCreatureId { get; set; }
        public virtual UserCreature UserCreature { get; set; }

        public string OwnerId { get; set; }
        public virtual ApplicationUser Owner { get; set; }

        public string CurrentBidderId { get; set; }
        public virtual ApplicationUser CurrentBidder { get; set; }
    }
}