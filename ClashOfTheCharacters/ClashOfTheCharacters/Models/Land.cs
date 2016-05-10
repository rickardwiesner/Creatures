using ClashOfTheCharacters.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.Models
{
    public class Land
    {
        public int Id { get; set; }

        public Element Element { get; set; }

        public int Cost { get; set; }

        public int Hours { get; set; }

        public string ImageUrl { get; set; }

        public string BackgroundImageUrl { get; set; }

        public int ShopPositionX { get; set; }

        public int ShopPositionY { get; set; }

        public virtual ICollection<Stage> Stages { get; set; }
    }
}