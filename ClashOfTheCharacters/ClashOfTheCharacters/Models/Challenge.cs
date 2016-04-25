using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.Models
{
    public class Challenge
    {
        public int Id { get; set; }
        public bool Accepted { get; set; }

        public string ChallengerId { get; set; }
        public virtual ApplicationUser Challenger { get; set; }

        public string ReceiverId { get; set; }
        public virtual ApplicationUser Receiver { get; set; }
    }
}