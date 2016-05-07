using ClashOfTheCharacters.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ClashOfTheCharacters.Models
{
    public class Creature
    {
        public int Id { get; set; }
        [Required]
        [StringLength(9, ErrorMessage = "The name can only be 9 characters long")]
        [MinLength(3, ErrorMessage = "The name must be at least 3 characters long")]
        public string Name { get; set; }
        public Element Element { get; set; }
        public string ImageUrl { get; set; }
        [Required]
        public int Price { get; set; }
        public Rarity Rarity { get; set; }
        [Required]
        public int BaseAttack { get; set; }
        [Required]
        public int BaseDefense { get; set; }
        [Required]
        public int BaseHp { get; set; }
        [Required]
        public float AttackMultiplier { get; set; }
        [Required]
        public float DefenseMultiplier { get; set; }
        [Required]
        public float HpMultiplier { get; set; }
    }
}