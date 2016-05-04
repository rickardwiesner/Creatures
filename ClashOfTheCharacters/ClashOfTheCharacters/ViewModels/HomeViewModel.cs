using ClashOfTheCharacters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.ViewModels
{
    public class HomeViewModel
    {
        public ICollection<ApplicationUser> Users { get; set; }
        public ICollection<Creature> Characters { get; set; }
    }
}