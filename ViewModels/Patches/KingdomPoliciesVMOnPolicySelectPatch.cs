using HarmonyLib;
using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.KingdomDecision;
using AllegianceOverhaul.CampaignBehaviors.BehaviorManagers;
using AllegianceOverhaul.Helpers;

namespace AllegianceOverhaul.ViewModels.Patches
{
  [HarmonyPatch(typeof(KingdomPoliciesVM), "OnPolicySelect")]
  public static class KingdomPoliciesVMOnPolicySelectPatch
  {   
    [HarmonyPostfix]
    public static void OnPolicySelectPatch(KingdomPolicyItemVM policy, KingdomPoliciesVM __instance)
    {
      try
      {
        if (SettingsHelper.SubSystemEnabled(SubSystemType.ElectionCooldownsForPlayer))
        {
          bool HasCooldown = AOCooldownManager.HasDecisionCooldown(new KingdomPolicyDecision(Clan.PlayerClan, policy.Policy, Clan.PlayerClan.Kingdom.ActivePolicies.Contains(policy.Policy)), out float elapsedDaysUntilNow);
          __instance.CanProposeOrDisavowPolicy = __instance.CanProposeOrDisavowPolicy && !HasCooldown;
          __instance.ProposeActionExplanationText += HasCooldown ? "\n" + StringHelper.GetCooldownText(typeof(KingdomPolicyDecision), elapsedDaysUntilNow).ToString() : string.Empty;
        }
      }
      catch (Exception ex)
      {
        MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
        DebugHelper.HandleException(ex, methodInfo, "Harmony patch for KingdomPoliciesVM. OnPolicySelect");
      }
    }

    public static bool Prepare()
    {
      return SettingsHelper.SubSystemEnabled(SubSystemType.ElectionCooldownsForPlayer);
    }
  }
}
