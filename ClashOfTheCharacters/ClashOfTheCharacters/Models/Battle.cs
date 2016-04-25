using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ClashOfTheCharacters.Helpers;
using ClashOfTheCharacters.Services;

namespace ClashOfTheCharacters.Models
{
    public class Battle
    {
        public int Id { get; set; }

        public DateTime StartTime { get; set; }

        //Testa <= 0 (Eller skit i CompareTo och kör en timespan)
        public bool Aired { get { return StartTime.CompareTo(DateTime.Now) < 0; } }

        public bool Calculated { get; set; }

        public virtual ICollection<Attack> Attacks { get; set; }
        public virtual ICollection<Competitor> Competitors { get; set; }

        public int ChallengeId { get; set; }

        public virtual Challenge Challenge { get; set; }
    }
}