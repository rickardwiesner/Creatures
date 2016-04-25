using ClashOfTheCharacters.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.Models
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Element Element { get; set; }
        public string ImageUrl { get; set; }
        public int Price { get; set; }

        public int BaseAttack { get; set; }
        public int BaseDefense { get; set; }
        public int BaseHp { get; set; }

        public float AttackMultiplier { get; set; }
        public float DefenseMultiplier { get; set; }
        public float HpMultiplier { get; set; }
    }
}