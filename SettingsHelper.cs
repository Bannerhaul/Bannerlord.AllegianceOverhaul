using MCM.Abstractions.Data;
using TaleWorlds.CampaignSystem;

namespace AllegianceOverhaul
{
  internal class SettingsHelper
  {
    public static bool InDebugBranch { get; set; } = false;

    public static bool FactionInScope (IFaction Faction, DefaultDropdown<string> Scope)
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
          if (Clan.PlayerClan.MapFaction is null || !Clan.PlayerClan.MapFaction.IsKingdomFaction || !(Faction.MapFaction.IsKingdomFaction &&  Faction.MapFaction.Leader != Hero.MainHero))
            return false;
          break;
      }

      return true;
    }
  }
}
