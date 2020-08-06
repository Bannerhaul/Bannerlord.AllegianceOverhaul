using System.Collections.Generic;
using System.Linq;
using AllegianceOverhaul.Extensions;
using MCM.Abstractions.Data;
using TaleWorlds.CampaignSystem;

namespace AllegianceOverhaul.Helpers
{
  internal static class SettingsHelper
  {
    public static bool FactionInScope(IFaction faction, DefaultDropdown<string> scope)
    {
      if (faction is null || faction.MapFaction is null || scope is null || scope.SelectedIndex < 0 || scope.SelectedIndex > 2)
        return false;

      switch (scope.SelectedIndex)
      {
        case 0: break;
        case 1:
          if (Clan.PlayerClan.MapFaction is null || !Clan.PlayerClan.MapFaction.IsKingdomFaction || Clan.PlayerClan.MapFaction != faction.MapFaction)
          {
            return false;
          }
          break;
        case 2:
          if (Clan.PlayerClan.MapFaction is null || !Clan.PlayerClan.MapFaction.IsKingdomFaction || !(faction.MapFaction.IsKingdomFaction && faction.MapFaction.Leader != Hero.MainHero))
          {
            return false;
          }
          break;
        default:
          break;
      }

      return true;
    }
    public static bool SubSystemEnabled(SubSystemType subSystem)
    {
      switch (subSystem)
      {
        //Loyalty
        case SubSystemType.EnsuredLoyalty: return Settings.Instance.UseEnsuredLoyalty;
        case SubSystemType.LoyaltyWithholding: return Settings.Instance.UseEnsuredLoyalty && Settings.Instance.UseRelationForEnsuredLoyalty && Settings.Instance.UseWithholdPrice;
        case SubSystemType.LoyaltyInConversations: return Settings.Instance.UseEnsuredLoyalty && Settings.Instance.UseLoyaltyInConversations;
        case SubSystemType.LoyaltyTooltips: return Settings.Instance.UseAdvancedHeroTooltips && Settings.Instance.UseEnsuredLoyalty;
        //Politics
        case SubSystemType.ElectionRebalance: return Settings.Instance.UsePoliticsRebalance && Settings.Instance.UseElectionRebalance;
        case SubSystemType.DecisionSupportRebalance: return Settings.Instance.UsePoliticsRebalance && Settings.Instance.UseElectionRebalance && Settings.Instance.UseDecisionSupportRebalance;
        case SubSystemType.MakePeaceSupportRebalance:
          return Settings.Instance.UsePoliticsRebalance && Settings.Instance.UseElectionRebalance && Settings.Instance.UseDecisionSupportRebalance
                 && Settings.Instance.PeaceSupportCalculationMethod.SelectedValue.EnumValue != PeaceAndWarConsideration.Native;
        case SubSystemType.DeclareWarSupportRebalance:
          return Settings.Instance.UsePoliticsRebalance && Settings.Instance.UseElectionRebalance && Settings.Instance.UseDecisionSupportRebalance
                 && Settings.Instance.WarSupportCalculationMethod.SelectedValue.EnumValue != PeaceAndWarConsideration.Native;
        case SubSystemType.SettlementClaimantSupportRebalance:
          return Settings.Instance.UsePoliticsRebalance && Settings.Instance.UseElectionRebalance && Settings.Instance.UseDecisionSupportRebalance
                 && Settings.Instance.FiefOwnershipSupportCalculationMethod.SelectedValue.EnumValue != FiefOwnershipConsideration.Native;
        case SubSystemType.AnnexationSupportRebalance:
          return Settings.Instance.UsePoliticsRebalance && Settings.Instance.UseElectionRebalance && Settings.Instance.UseDecisionSupportRebalance
                 && Settings.Instance.AnnexSupportCalculationMethod.SelectedValue.EnumValue != FiefOwnershipConsideration.Native;
        case SubSystemType.ElectionCooldowns: return Settings.Instance.UsePoliticsRebalance && Settings.Instance.UseElectionRebalance && Settings.Instance.UseElectionCooldowns;
        case SubSystemType.ElectionCooldownsForPlayer: return Settings.Instance.UsePoliticsRebalance && Settings.Instance.UseElectionRebalance && Settings.Instance.UseElectionCooldowns && Settings.Instance.UseElectionCooldownsForPlayer;
        //General
        case SubSystemType.AdvancedHeroTooltips: return Settings.Instance.UseAdvancedHeroTooltips;
        //Testing
        case SubSystemType.FreeDecisionOverriding: return Settings.Instance.UseTestingSettings && Settings.Instance.FreeDecisionOverriding;
        case SubSystemType.DestabilizeJoinEvaluation: return Settings.Instance.UseTestingSettings && Settings.Instance.DestabilizeJoinEvaluation;
        case SubSystemType.DestabilizeLeaveEvaluation: return Settings.Instance.UseTestingSettings && Settings.Instance.DestabilizeLeaveEvaluation;
        default: return false;
      }
    }
    public static bool SubSystemEnabled(SubSystemType subSystem, IFaction faction)
    {
      return SubSystemEnabled(subSystem) && FactionInScope(faction, DetermineSubSystemScope(subSystem));
    }
    public static bool SubSystemEnabled(SubSystemType subSystem, List<IFaction> factionList)
    {
      if (!SubSystemEnabled(subSystem))
      {
        return false;
      }
      DefaultDropdown<string> scope = DetermineSubSystemScope(subSystem);
      foreach (IFaction faction in factionList)
      {
        if (FactionInScope(faction, scope))
        {
          return true;
        }
      }
      return false;
    }
    public static bool SystemInScope(AOSystems system, AOSystems scope)
    {
      return scope.Contains(system);
    }
    public static bool SystemDebugEnabled(AOSystems system, DebugType debugType)
    {
      switch (debugType)
      {
        case DebugType.General: return Settings.Instance.EnableGeneralDebugging && SystemInScope(system, Settings.Instance.DebugSystemScope.SelectedValue.EnumValue);
        case DebugType.Technical: return Settings.Instance.EnableTechnicalDebugging && SystemInScope(system, Settings.Instance.DebugSystemScope.SelectedValue.EnumValue);
        case DebugType.Any:
          return (Settings.Instance.EnableGeneralDebugging || Settings.Instance.EnableTechnicalDebugging)
                 && SystemInScope(system, Settings.Instance.DebugSystemScope.SelectedValue.EnumValue);
        default: return false;
      }
    }
    public static bool SystemDebugEnabled(AOSystems system, DebugType debugType, IFaction faction)
    {
      return SystemDebugEnabled(system, debugType) && FactionInScope(faction, Settings.Instance.DebugFactionScope);
    }
    public static bool SystemDebugEnabled(AOSystems system, DebugType debugType, List<IFaction> factionList)
    {
      if (!SystemDebugEnabled(system, debugType))
      {
        return false;
      }
      foreach (IFaction faction in factionList)
      {
        if (FactionInScope(faction, Settings.Instance.DebugFactionScope))
        {
          return true;
        }
      }
      return false;
    }
    private static DefaultDropdown<string> DetermineSubSystemScope(SubSystemType subSystem)
    {
      switch ((int)subSystem)
      {
        case int subSystemIdx when (subSystemIdx < 50): return Settings.Instance.EnsuredLoyaltyScope;
        case int subSystemIdx when (subSystemIdx >= 50 && subSystemIdx < 100): return Settings.Instance.PoliticsRebalanceScope;
        default: return null;
      }
    }
  }
  public enum DebugType : byte
  {
    None = 0,
    General = 1,
    Technical = 2,
    Any = 255
  }
  public enum SubSystemType : byte
  {
    None = 0,
    //Loyalty
    EnsuredLoyalty = 1,
    LoyaltyWithholding = 10,
    LoyaltyInConversations = 11,
    //LoyaltyVM
    LoyaltyTooltips = 20,
    //Politics
    ElectionRebalance = 50,
    //Politics - DecisionSupportRebalance
    DecisionSupportRebalance = 60,
    MakePeaceSupportRebalance = 61,
    DeclareWarSupportRebalance = 62,
    SettlementClaimantSupportRebalance = 63,
    AnnexationSupportRebalance = 64,
    //Politics - ElectionCooldowns
    ElectionCooldowns = 70,
    ElectionCooldownsForPlayer = 71,
    //GeneralVM
    AdvancedHeroTooltips = 200,
    //TestingSettings
    FreeDecisionOverriding = 210,
    DestabilizeJoinEvaluation = 211,
    DestabilizeLeaveEvaluation = 212
  }
}
