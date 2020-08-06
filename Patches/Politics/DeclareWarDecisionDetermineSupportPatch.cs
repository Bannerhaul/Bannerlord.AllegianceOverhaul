using HarmonyLib;
using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using AllegianceOverhaul.Helpers;
using AllegianceOverhaul.Extensions;

namespace AllegianceOverhaul.Patches.Politics
{
  [HarmonyPatch(typeof(DeclareWarDecision), "DetermineSupport")]
  class DeclareWarDecisionDetermineSupportPatch
  {
    public static void Postfix(Clan clan, DecisionOutcome possibleOutcome, ref float __result, DeclareWarDecision __instance)
    {
      try
      {
        float newResult = Campaign.Current.GetAOGameModels().DecisionSupportScoringModel.DetermineSupport(clan, __instance, possibleOutcome);
        if (SettingsHelper.SystemDebugEnabled(AOSystems.PoliticsRebalance, DebugType.Technical, clan))
        {
          MessageHelper.TechnicalMessage(string.Format("Support of {0} for {1} declaring war on {2}.\nNative result = {3}. Rebalanced result = {4}",
                                                       clan.Name,
                                                       FieldAccessHelper.ShouldPeaceBeDeclaredByRef(possibleOutcome) ? "accepting" : "denying",
                                                       __instance.FactionToDeclareWarOn, __result, newResult));
        }
        if (SettingsHelper.SubSystemEnabled(SubSystemType.DeclareWarSupportRebalance, clan))
        {
          __result = newResult;
        }
      }
      catch (Exception ex)
      {
        MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
        DebugHelper.HandleException(ex, methodInfo, "Harmony patch for DeclareWarDecision. DetermineSupport");
      }
    }

    public static bool Prepare()
    {
      return SettingsHelper.SubSystemEnabled(SubSystemType.DecisionSupportRebalance) || SettingsHelper.SystemDebugEnabled(AOSystems.PoliticsRebalance, DebugType.Technical);
    }
  }
}
