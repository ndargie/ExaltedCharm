using System.Text;
using ExaltedCharm.Api.Entities;

namespace ExaltedCharm.Api.Extensions
{
    public static class CharmEntensions
    {
        public static string GetCharmCost(this Charm charm)
        {
            StringBuilder cost = new StringBuilder();
            if (charm.MoteCost > 0)
            {
                cost.Append($"{charm.MoteCost} m ");
            }

            if (charm.WillpowerCost > 0)
            {
                cost.Append($"{charm.WillpowerCost} w ");
            }

            if (charm.HealthCost > 0)
            {
                cost.Append($"{charm.HealthCost} h");
            }

            return cost.ToString().Trim();
        }
    }
}