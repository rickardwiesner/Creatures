using ClashOfTheCharacters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.ViewModels
{
    public class MapViewModel
    {
        public Travel Travel { get; set; }

        public CurrentLand CurrentLand { get; set; }
    }
}