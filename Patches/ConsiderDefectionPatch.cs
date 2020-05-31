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
    public static void DebugPrefix(Clan clan1, Kingdom kingdom)
    {
      //void prefixes are guaranteed to run
      LoyaltyRebalance.DebugHelper.DefectKingdomDebug(clan1, kingdom);
    }

    [HarmonyPriority(Priority.Normal)]
    public static bool Prefix(Clan clan1, Kingdom kingdom)
    {
      //Bool prefixes compete with each other and skip others, as well as original, if return false
      return !LoyaltyRebalance.EnsuredLoyalty.CheckLoyalty(clan1, true);
    }
  }
}