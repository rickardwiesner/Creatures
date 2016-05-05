using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.Models
{
    public class SellShopView
    {
        public List<Creature> ShoppingItems { get; set; }
        public List<Creature> SellItems { get; set; }
        public int Gold { get; set; }
        public bool Travelling { get; set; }
    }
}