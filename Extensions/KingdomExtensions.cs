using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace AllegianceOverhaul.Extensions
{
  public static class KingdomExtensions
  {
    public static int GetNumberOfWars(this Kingdom kingdom)
    {
      return Kingdom.All.Count(k => k != kingdom && k.IsAtWarWith(kingdom));
    }

    public static int GetNumberOfWars(this Kingdom kingdom, Kingdom kingdomToToggle)
    {
      return kingdomToToggle.IsAtWarWith(kingdom) ? Kingdom.All.Count(k => k != kingdom && k != kingdomToToggle && k.IsAtWarWith(kingdom)) : Kingdom.All.Count(k => k != kingdom && (k == kingdomToToggle || k.IsAtWarWith(kingdom)));
    }

    public static IEnumerable<Kingdom> GetAdversaries(this Kingdom kingdom)
    {
      return Kingdom.All.Where(k => k != kingdom && k.IsAtWarWith(kingdom));
    }

    public static IEnumerable<Kingdom> GetAdversaries(this Kingdom kingdom, Kingdom kingdomToToggle)
    {
      return kingdomToToggle.IsAtWarWith(kingdom) ? Kingdom.All.Where(k => k != kingdom && k != kingdomToToggle && k.IsAtWarWith(kingdom)) : Kingdom.All.Where(k => k != kingdom && (k == kingdomToToggle || k.IsAtWarWith(kingdom)));
    }

    public static float GetEffectiveStrength(this Kingdom kingdom)
    {
      return kingdom.TotalStrength / Math.Max(1, kingdom.GetNumberOfWars());
    }

    public static float GetEffectiveStrength(this Kingdom kingdom, Kingdom kingdomToToggle)
    {
      return kingdom.TotalStrength / Math.Max(1, kingdom.GetNumberOfWars() - (kingdom.IsAtWarWith(kingdomToToggle) ? 1 : -1));
    }
  }
}
