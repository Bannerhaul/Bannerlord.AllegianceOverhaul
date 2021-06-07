using HarmonyLib;

using System;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;

using AllegianceOverhaul.Helpers;
using AllegianceOverhaul.LoyaltyRebalance;

namespace AllegianceOverhaul.Patches.Loyalty
{
  [HarmonyPatch(typeof(DefaultDiplomacyModel), "GetScoreOfMercenaryToLeaveKingdom")]
  public class GetScoreOfMercenaryToLeaveKingdomPatch
  {
    public static void Postfix(Clan mercenaryClan, Kingdom kingdom, DefaultDiplomacyModel __instance, ref float __result)
    {
      try
      {
        if (!LoyaltyDebugHelper.InDebugBranch || !SettingsHelper.SystemDebugEnabled(AOSystems.EnsuredLoyalty, DebugType.Technical, mercenaryClan))
          return;

        float ComputedResult = (float)(10000.0 * (0.00999999977648258 * Math.Min(100f, mercenaryClan.LastFactionChangeTime.ElapsedDaysUntilNow)) - 5000.0) - __instance.GetScoreOfMercenaryToJoinKingdom(mercenaryClan, kingdom);

        string UnitValueDebugInfo = string.Format("ScoreOfMercenaryToLeaveKingdom. DaysWithFaction = {0}. DaysWithFactionModified = {1}. GetScoreOfMercenaryToJoinKingdom = {2}." +
          "CalculatedResult = {3}. NativeResult = {4}.",
          mercenaryClan.LastFactionChangeTime.ElapsedDaysUntilNow.ToString(), (10000.0 * (0.00999999977648258 * Math.Min(100f, mercenaryClan.LastFactionChangeTime.ElapsedDaysUntilNow))).ToString("N"),
          __instance.GetScoreOfMercenaryToJoinKingdom(mercenaryClan, kingdom).ToString("N"),
          ComputedResult.ToString("N"), __result.ToString("N"));

        MessageHelper.TechnicalMessage(UnitValueDebugInfo);
      }
      catch (Exception ex)
      {
        MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
        DebugHelper.HandleException(ex, methodInfo, "Harmony patch for GetScoreOfMercenaryToLeaveKingdom");
      }
    }
    public static bool Prepare()
    {
      return SettingsHelper.SystemDebugEnabled(AOSystems.EnsuredLoyalty, DebugType.Technical);
    }
  }
}