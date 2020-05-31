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
    public static void DebugPrefix(Clan clan)
    {
      //void prefixes are guaranteed to run
      LoyaltyRebalance.DebugHelper.LeaveKingdomDebug(clan);
    }

    [HarmonyPriority(Priority.Normal)]
    public static bool Prefix(Clan clan)
    {
      //Bool prefixes compete with each other and skip others, as well as original, if return false
      return !LoyaltyRebalance.EnsuredLoyalty.CheckLoyalty(clan);
    }
  }
}