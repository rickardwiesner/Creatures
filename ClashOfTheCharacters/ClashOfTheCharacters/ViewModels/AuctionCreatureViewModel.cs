using ClashOfTheCharacters.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.ViewModels
{
    public class AuctionCreatureViewModel
    {
        public int UserCreatureId { get; set; }

        public string Name { get; set; }

        [Required]
        [Range(1, 24, ErrorMessage = "Need to be between 1 and 24 hours")]
        public int Hours { get; set; }

        [Required]
        [Display(Name = "Start Price")]
        public int StartPrice { get; set; }

        [Display(Name = "Buyout Price")]
        public int? BuyoutPrice { get; set; }
    }
}