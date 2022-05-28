using AllegianceOverhaul.Helpers;
using AllegianceOverhaul.LoyaltyRebalance;

using HarmonyLib;

using System;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;

namespace AllegianceOverhaul.Patches.Loyalty
{
    [HarmonyPatch(typeof(LeaveKingdomAsClanBarterable), "GetUnitValueForFaction")]
    public static class GetUnitValueForFactionLeaveKingdomPatch
    {
        public static void Postfix(IFaction faction, ref int __result, LeaveKingdomAsClanBarterable __instance)
        {
            try
            {
                Hero iOriginalOwner = __instance.OriginalOwner;
                Clan iOriginalOwnerClan = iOriginalOwner.Clan;
                Kingdom iOriginalOwnerKingdom = iOriginalOwnerClan.Kingdom;

                bool fixMinorFactionVassals = Settings.Instance!.FixMinorFactionVassals;
                bool debugEnabled = LoyaltyDebugHelper.InDebugBranch && SettingsHelper.SystemDebugEnabled(AOSystems.EnsuredLoyalty, DebugType.Technical);

                if (!fixMinorFactionVassals && !debugEnabled)
                    return;

                IFaction mapFaction = iOriginalOwner.MapFaction;
                float CalculatedResult;
                if (faction == iOriginalOwnerClan)
                    CalculatedResult = iOriginalOwnerClan.IsUnderMercenaryService ? (int) Campaign.Current.Models.DiplomacyModel.GetScoreOfMercenaryToLeaveKingdom(iOriginalOwnerClan, iOriginalOwnerKingdom) : (int) Campaign.Current.Models.DiplomacyModel.GetScoreOfClanToLeaveKingdom(iOriginalOwnerClan, iOriginalOwnerKingdom);
                else
                {
                    if (faction == mapFaction)
                    {
                        CalculatedResult = (float) ((!iOriginalOwnerClan.IsUnderMercenaryService ? Campaign.Current.Models.DiplomacyModel.GetScoreOfClanToLeaveKingdom(iOriginalOwnerClan, iOriginalOwnerKingdom) : Campaign.Current.Models.DiplomacyModel.GetScoreOfMercenaryToLeaveKingdom(iOriginalOwnerClan, iOriginalOwnerKingdom)) * (faction == iOriginalOwnerClan || faction == iOriginalOwnerKingdom ? -1.0 : 1.0));
                    }
                    else
                    {
                        float clanStrength = Campaign.Current.Models.DiplomacyModel.GetClanStrength(iOriginalOwnerClan);
                        CalculatedResult = !faction.IsClan || !FactionManager.IsAtWarAgainstFaction(faction, iOriginalOwnerKingdom) ? (!FactionManager.IsAlliedWithFaction(faction, iOriginalOwnerKingdom) ? clanStrength * 0.01f : clanStrength * -0.5f) : clanStrength * 0.5f;
                    }
                }

                if (debugEnabled)
                {
                    string UnitValueDebugInfo = string.Format("LeaveKingdom - UnitValueForFaction. faction: {0}. ScoreOfMercenaryToLeaveKingdom = {1}. ScoreOfClanToLeaveKingdom = {2}. CalculatedResult = {3}. Result = {4}",
                      faction.Name,
                      ((int) Campaign.Current.Models.DiplomacyModel.GetScoreOfMercenaryToLeaveKingdom(iOriginalOwnerClan, iOriginalOwnerKingdom)).ToString("N"),
                      ((int) Campaign.Current.Models.DiplomacyModel.GetScoreOfClanToLeaveKingdom(iOriginalOwnerClan, iOriginalOwnerKingdom)).ToString("N"),
                      CalculatedResult.ToString("N"), __result.ToString("N"));

                    MessageHelper.TechnicalMessage(UnitValueDebugInfo);
                }
                if (fixMinorFactionVassals)
                {
                    __result = (int) CalculatedResult;
                }
            }
            catch (Exception ex)
            {
                MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                DebugHelper.HandleException(ex, methodInfo, "Harmony patch for LeaveKingdomAsClanBarterable.GetUnitValueForFaction");
            }
        }
        public static bool Prepare()
        {
            return Settings.Instance!.FixMinorFactionVassals || SettingsHelper.SystemDebugEnabled(AOSystems.EnsuredLoyalty, DebugType.Technical);
        }
    }
}