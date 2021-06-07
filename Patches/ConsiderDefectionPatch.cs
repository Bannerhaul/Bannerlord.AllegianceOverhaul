using HarmonyLib;

using System;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.BarterBehaviors;

using AllegianceOverhaul.Helpers;

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
        MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
        DebugHelper.HandleException(ex, methodInfo, "Harmony patch for ConsiderDefection");
      }
    }

    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    public static bool LoyaltyPrefix(Clan clan1, Kingdom kingdom) //Bool prefixes compete with each other and skip others, as well as original, if return false
    {
      try
      {
        return !LoyaltyRebalance.EnsuredLoyalty.LoyaltyManager.CheckLoyalty(clan1, kingdom);
      }
      catch (Exception ex)
      {
        MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
        DebugHelper.HandleException(ex, methodInfo, "Harmony patch for ConsiderDefection");
        return true;
      }
    }

    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal - 1)]
    public static bool MigrationPrefix(Clan clan1, Kingdom kingdom) //Bool prefixes compete with each other and skip others, as well as original, if return false
    {
      try
      {
        return !LoyaltyRebalance.EnsuredLoyalty.LoyaltyManager.CheckLoyalty(clan1, kingdom);
      }
      catch (Exception ex)
      {
        MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
        DebugHelper.HandleException(ex, methodInfo, "Harmony patch for ConsiderDefection");
        return true;
      }
    }

    public static bool Prepare()
    {
      return Settings.Instance!.UseEnsuredLoyalty || SettingsHelper.SystemDebugEnabled(AOSystems.EnsuredLoyalty, DebugType.Any);
    }
  }
}