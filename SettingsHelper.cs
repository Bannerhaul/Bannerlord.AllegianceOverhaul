using System.Linq;
using MCM.Abstractions.Data;
using TaleWorlds.CampaignSystem;

namespace AllegianceOverhaul
{
  internal class SettingsHelper
  {
    public static bool InDebugBranch { get; set; } = false;

    public static bool FactionInScope(IFaction Faction, DefaultDropdown<string> Scope)
    {
      if (Faction is null || Faction.MapFaction is null || Scope.SelectedIndex < 0 || Scope.SelectedIndex > 2)
        return false;

      switch (Scope.SelectedIndex)
      {
        case 0: break;
        case 1:
          if (Clan.PlayerClan.MapFaction is null || !Clan.PlayerClan.MapFaction.IsKingdomFaction || Clan.PlayerClan.MapFaction != Faction.MapFaction)
            return false;
          break;
        case 2:
          if (Clan.PlayerClan.MapFaction is null || !Clan.PlayerClan.MapFaction.IsKingdomFaction || !(Faction.MapFaction.IsKingdomFaction && Faction.MapFaction.Leader != Hero.MainHero))
            return false;
          break;
        default:
          break;
      }

      return true;
    }

    public static bool BloodRelatives(Hero queriedHero, Hero baseHero)
    {
      return
        baseHero.Father == queriedHero || baseHero.Mother == queriedHero || baseHero.Siblings.Contains(queriedHero) || baseHero.Children.Contains(queriedHero) || baseHero.Spouse == queriedHero
        || baseHero.Spouse?.Father == queriedHero || baseHero.Spouse?.Mother == queriedHero || (baseHero.Spouse != null && baseHero.Spouse.Siblings.Contains(queriedHero));
    }
    public static bool BloodRelatives(Clan queriedClan, Clan baseClan)
    {
      foreach (Hero baseHero in baseClan.Heroes)
      {
        foreach (Hero queriedHero in queriedClan.Heroes)
        {
          if (BloodRelatives(queriedHero, baseHero))
            return true;
        }
      }
      return false;
    }
  }
}
