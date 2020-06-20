using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.BarterBehaviors;

namespace AllegianceOverhaul.Patches
{
  [HarmonyPatch(typeof(DiplomaticBartersBehavior), "ConsiderClanLeaveKingdom")]
  public class ConsiderClanLeaveKingdomPatch
  {
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    public static void DebugPrefix(Clan clan) //void prefixes are guaranteed to run
    {
      try
      {
        LoyaltyRebalance.LoyaltyDebugHelper.LeaveKingdomDebug(clan);
      }
      catch (Exception ex)
      {
        MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
        DebugHelper.HandleException(ex, methodInfo, "Harmony patch for ConsiderClanLeaveKingdom");
      }
    }

    [HarmonyPriority(Priority.Normal)]
    public static bool Prefix(Clan clan) //Bool prefixes compete with each other and skip others, as well as original, if return false
    {
      try
      {
        return !LoyaltyRebalance.EnsuredLoyalty.LoyaltyManager.CheckLoyalty(clan);
      }
      catch (Exception ex)
      {
        MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
        DebugHelper.HandleException(ex, methodInfo, "Harmony patch for ConsiderClanLeaveKingdom");
        return true;
      }
    }

    public static bool Prepare()
    {
      return Settings.Instance.UseEnsuredLoyalty || Settings.Instance.EnableGeneralDebugging || Settings.Instance.EnableTechnicalDebugging;
    }
  }
}