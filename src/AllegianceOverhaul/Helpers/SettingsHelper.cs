using AllegianceOverhaul.Extensions;

using MCM.Common;

using System.Collections.Generic;
using System.Linq;

using TaleWorlds.CampaignSystem;

namespace AllegianceOverhaul.Helpers
{
    internal static class SettingsHelper
    {
        public static bool FactionInScope(IFaction faction, Dropdown<string>? scope)
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
            return subSystem switch
            {
                //Loyalty
                SubSystemType.EnsuredLoyalty => Settings.Instance!.UseEnsuredLoyalty,
                SubSystemType.LoyaltyWithholding => Settings.Instance!.UseEnsuredLoyalty && Settings.Instance.UseRelationForEnsuredLoyalty && Settings.Instance.UseWithholdPrice,
                SubSystemType.LoyaltyInConversations => Settings.Instance!.UseEnsuredLoyalty && Settings.Instance.UseLoyaltyInConversations,
                SubSystemType.LoyaltyTooltips => Settings.Instance!.UseAdvancedHeroTooltips && Settings.Instance.UseEnsuredLoyalty,
                //Migration
                SubSystemType.MigrationTweaks => Settings.Instance!.UseMigrationTweaks,
                SubSystemType.AllowJoinRequests => Settings.Instance!.UseMigrationTweaks && Settings.Instance.AllowJoinRequests,
                SubSystemType.AllowHireRequests => Settings.Instance!.UseMigrationTweaks && Settings.Instance.AllowHireRequests,
                SubSystemType.UseDeterminedKingdomPick => Settings.Instance!.UseMigrationTweaks && Settings.Instance.UseDeterminedKingdomPick,
                SubSystemType.LeaderDefectionFix => Settings.Instance!.UseMigrationTweaks && Settings.Instance.ApplyLeaderDefectionFix,
                SubSystemType.PersuasionLockoutTweak => Settings.Instance!.UseMigrationTweaks && Settings.Instance.UsePersuasionLockoutTweak,
                //Politics
                SubSystemType.ElectionRebalance => Settings.Instance!.UsePoliticsRebalance && Settings.Instance.UseElectionRebalance,
                SubSystemType.ElectionCooldowns => Settings.Instance!.UsePoliticsRebalance && Settings.Instance.UseElectionRebalance && Settings.Instance.UseElectionCooldowns,
                SubSystemType.ElectionCooldownsForPlayer => Settings.Instance!.UsePoliticsRebalance && Settings.Instance.UseElectionRebalance && Settings.Instance.UseElectionCooldowns && Settings.Instance.UseElectionCooldownsForPlayer,
                //General
                SubSystemType.AdvancedHeroTooltips => Settings.Instance!.UseAdvancedHeroTooltips,
                //Testing
                SubSystemType.FreeDecisionOverriding => Settings.Instance!.UseTestingSettings && Settings.Instance.FreeDecisionOverriding,
                SubSystemType.AlwaysPickPlayerKingdom => Settings.Instance!.UseTestingSettings && Settings.Instance.AlwaysPickPlayerKingdom,
                SubSystemType.DestabilizeJoinEvaluation => Settings.Instance!.UseTestingSettings && Settings.Instance.DestabilizeJoinEvaluation,
                SubSystemType.DestabilizeLeaveEvaluation => Settings.Instance!.UseTestingSettings && Settings.Instance.DestabilizeLeaveEvaluation,
                _ => false,
            };
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
            Dropdown<string>? scope = DetermineSubSystemScope(subSystem);
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
            return debugType switch
            {
                DebugType.General => Settings.Instance!.EnableGeneralDebugging && SystemInScope(system, Settings.Instance.DebugSystemScope.SelectedValue.EnumValue),
                DebugType.Technical => Settings.Instance!.EnableTechnicalDebugging && SystemInScope(system, Settings.Instance.DebugSystemScope.SelectedValue.EnumValue),
                DebugType.Any => (Settings.Instance!.EnableGeneralDebugging || Settings.Instance.EnableTechnicalDebugging) && SystemInScope(system, Settings.Instance.DebugSystemScope.SelectedValue.EnumValue),
                _ => false,
            };
        }
        public static bool SystemDebugEnabled(AOSystems system, DebugType debugType, IFaction faction)
        {
            return SystemDebugEnabled(system, debugType) && FactionInScope(faction, Settings.Instance!.DebugFactionScope);
        }
        public static bool SystemDebugEnabled(AOSystems system, DebugType debugType, List<IFaction> factionList)
        {
            if (!SystemDebugEnabled(system, debugType))
            {
                return false;
            }
            foreach (IFaction faction in factionList)
            {
                if (FactionInScope(faction, Settings.Instance!.DebugFactionScope))
                {
                    return true;
                }
            }
            return false;
        }
        private static Dropdown<string>? DetermineSubSystemScope(SubSystemType subSystem)
        {
            Dropdown<string> defaultGlobalScope = new(new string[] { Settings.DropdownValueAllFactions, Settings.DropdownValuePlayers, Settings.DropdownValueRuledBy }, 0);
            return (int) subSystem switch
            {
                int subSystemIdx when subSystemIdx < (int) SubSystemType.MigrationTweaks => Settings.Instance!.EnsuredLoyaltyScope,
                int subSystemIdx when subSystemIdx is >= (int) SubSystemType.MigrationTweaks and < (int) SubSystemType.ElectionRebalance => defaultGlobalScope,
                int subSystemIdx when subSystemIdx is >= (int) SubSystemType.ElectionRebalance and < 150 => Settings.Instance!.PoliticsRebalanceScope,
                _ => null,
            };
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
        //Migration
        MigrationTweaks = 50,
        AllowJoinRequests = 51,
        AllowHireRequests = 52,
        UseDeterminedKingdomPick = 55,
        LeaderDefectionFix = 56,
        PersuasionLockoutTweak = 70,
        //Politics
        ElectionRebalance = 100,
        //Politics - ElectionCooldowns
        ElectionCooldowns = 110,
        ElectionCooldownsForPlayer = 111,
        //GeneralVM
        AdvancedHeroTooltips = 200,
        //TestingSettings
        FreeDecisionOverriding = 210,
        AlwaysPickPlayerKingdom = 211,
        DestabilizeJoinEvaluation = 221,
        DestabilizeLeaveEvaluation = 222
    }
}