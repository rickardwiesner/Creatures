using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.Helpers
{
    public class Helper
    {
        static public string GetElementUrl(Element element)
        {
            switch (element)
            {
                case Element.Fire:
                    return "/Images/Elements/fire.png";

                case Element.Air:
                    return "/Images/Elements/air.png";

                case Element.Earth:
                    return "/Images/Elements/earth.png";

                case Element.Water:
                    return "/Images/Elements/water.png";

                default:
                    return string.Empty;
            }
        }

        static public string GetPercentage(int xp, int maxXp)
        {
            return Convert.ToString(Convert.ToDecimal(xp) / Convert.ToDecimal(maxXp) * 100).Replace(",", ".");
        }

        static public string GetTime(DateTime dateTime)
        {
            if ((DateTime.Now.Subtract(dateTime.TimeOfDay).Minute < 1))
            {
                return string.Format("{0}{1}", dateTime.Subtract(DateTime.Now.TimeOfDay).Second, "s");
            }

            else
            {
                return string.Format("{0}{1}", dateTime.Subtract(DateTime.Now.TimeOfDay).Minute, "m");
            }
        }

        static public string GetEffect(Effect effect)
        {
            switch (effect)
            {
                case Effect.VeryBad:
                    return "Not effective at all";

                case Effect.Bad:
                    return "Not effective";

                case Effect.Good:
                    return "Effective";

                case Effect.VeryGood:
                    return "Very effective";

                case Effect.GravityAttack:
                    return "Quite effective";

                default:
                    return "Normal";
            }
        }
    }
}