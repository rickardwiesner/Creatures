using ClashOfTheCharacters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.Services
{
    public class AuctionService
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public void CheckAuctions()
        {
            foreach (var auctionCreature in db.AuctionCreatures.Where(ac => ac.EndTime.CompareTo(DateTimeOffset.Now) < 0 && !ac.Finished))
            {
                if (auctionCreature.CurrentBid != null)
                {
                    auctionCreature.Owner.Gold += Convert.ToInt32((float)auctionCreature.CurrentBid * 0.95);
                    auctionCreature.UserCreature.UserId = auctionCreature.CurrentBidderId;
                    auctionCreature.UserCreature.InAuction = false;
                    auctionCreature.Finished = true;
                }

                else
                {
                    auctionCreature.UserCreature.InAuction = false;
                    auctionCreature.Finished = true;
                }            
            }

            db.SaveChanges();
        }
    }
}