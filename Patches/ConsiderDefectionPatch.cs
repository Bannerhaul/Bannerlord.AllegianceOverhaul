using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.BarterBehaviors;

namespace AllegianceOverhaul.Patches
{
  [HarmonyPatch(typeof(DiplomaticBartersBehavior), "ConsiderDefection")]
  public class ConsiderDefectionPatch
  {
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    public static void DebugPrefix(Clan clan1, Kingdom kingdom) //void prefixes are guaranteed to run
    {
      try
      {
        LoyaltyRebalance.LoyaltyDebugHelper.DefectKingdomDebug(clan1, kingdom);
      }
      catch (Exception ex)
      {
        MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
        DebugHelper.HandleException(ex, methodInfo, "Harmony patch for ConsiderDefection");
      }
    }

    [HarmonyPriority(Priority.HigherThanNormal)]
    public static bool Prefix(Clan clan1, Kingdom kingdom) //Bool prefixes compete with each other and skip others, as well as original, if return false
    {
      try
      {
        return !LoyaltyRebalance.EnsuredLoyalty.LoyaltyManager.CheckLoyalty(clan1, kingdom);
      }
      catch (Exception ex)
      {
        MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
        DebugHelper.HandleException(ex, methodInfo, "Harmony patch for ConsiderDefection");
        return true;
      }
    }

    public static bool Prepare()
    {
      return Settings.Instance.UseEnsuredLoyalty || Settings.Instance.EnableGeneralDebugging || Settings.Instance.EnableTechnicalDebugging;
    }
  }
}