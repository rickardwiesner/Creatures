using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.Models
{
    public class SellShopView
    {
        public List<Character> ShoppingItems { get; set; }
        public List<Character> SellItems { get; set; }
    }
}