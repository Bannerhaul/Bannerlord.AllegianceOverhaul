using HarmonyLib;

using System;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.KingdomClan;

using AllegianceOverhaul.CampaignBehaviors.BehaviorManagers;
using AllegianceOverhaul.Helpers;

namespace AllegianceOverhaul.ViewModels.Patches
{
  [HarmonyPatch(typeof(KingdomClanVM), "SetCurrentSelectedClan")]
  public static class KingdomClanVMSetCurrentSelectedClanPatch
  {   
    [HarmonyPostfix]
    public static void SetCurrentSelectedClanPatch(KingdomClanItemVM clan, KingdomClanVM __instance)
    {
      try
      {
        if (SettingsHelper.SubSystemEnabled(SubSystemType.ElectionCooldownsForPlayer))
        {
          bool HasCooldown = AOCooldownManager.HasDecisionCooldown(new ExpelClanFromKingdomDecision(Clan.PlayerClan, clan.Clan), out float elapsedDaysUntilNow);
          __instance.CanExpelCurrentClan = __instance.CanExpelCurrentClan && !HasCooldown;
          __instance.ExpelActionExplanationText += HasCooldown ? "\n" + StringHelper.GetCooldownText(typeof(ExpelClanFromKingdomDecision), elapsedDaysUntilNow).ToString() : string.Empty;
        }
      }
      catch (Exception ex)
      {
        MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
        DebugHelper.HandleException(ex, methodInfo, "Harmony patch for KingdomClanVM. SetCurrentSelectedClan");
      }
    }

    public static bool Prepare()
    {
      return SettingsHelper.SubSystemEnabled(SubSystemType.ElectionCooldownsForPlayer);
    }
  }
}
