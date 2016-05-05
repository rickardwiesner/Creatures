using ClashOfTheCharacters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.ViewModels
{
    public class LadderViewModel
    {
        public ApplicationUser User { get; set; }

        public int Rank { get; set; }
    }
}