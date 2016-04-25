using ClashOfTheCharacters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.Services
{
    public class StaminaService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public void UpdateStamina(string userId)
        {
            var user = db.Users.Find(userId);
            var timeSpan = DateTimeOffset.Now - user.LastStaminaTime;

            for (int i = 10; i <= user.MaxStamina * 10; i+=10)
            {
                if(timeSpan.TotalMinutes >= i)
                {
                    if (user.Stamina + 6 < user.MaxStamina)
                    {
                        user.Stamina += 6;
                        user.LastStaminaTime = DateTimeOffset.Now.AddMinutes(i - timeSpan.TotalMinutes);

                    }

                    else
                    {
                        user.Stamina = user.MaxStamina;
                        user.LastStaminaTime = DateTimeOffset.Now;
                        break;
                    }
                }

                else
                {
                    break;
                }
            }

            db.SaveChanges();        
        }
    }
}