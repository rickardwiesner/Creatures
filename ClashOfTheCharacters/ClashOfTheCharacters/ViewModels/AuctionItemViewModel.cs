using ClashOfTheCharacters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.ViewModels
{
    public class AuctionItemViewModel
    {
        public UserItem UserItem { get; set; }

        public int Hours { get; set; }

        public int StartPrice { get; set; }

        public int? BuyoutPrice { get; set; }
    }
}