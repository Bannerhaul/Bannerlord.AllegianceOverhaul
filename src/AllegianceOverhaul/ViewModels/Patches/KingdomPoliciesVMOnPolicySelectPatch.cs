using AllegianceOverhaul.CampaignBehaviors.BehaviorManagers;
using AllegianceOverhaul.Helpers;

using HarmonyLib;

using System;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Policies;

namespace AllegianceOverhaul.ViewModels.Patches
{
    [HarmonyPatch(typeof(KingdomPoliciesVM), "OnPolicySelect")]
    public static class KingdomPoliciesVMOnPolicySelectPatch
    {
        [HarmonyPostfix]
        public static void OnPolicySelectPatch(KingdomPolicyItemVM? policy, KingdomPoliciesVM __instance)
        {
            try
            {
                if (SettingsHelper.SubSystemEnabled(SubSystemType.ElectionCooldownsForPlayer) && policy?.Policy != null)
                {
                    bool hasCooldown = AOCooldownManager.HasDecisionCooldown(new KingdomPolicyDecision(Clan.PlayerClan, policy.Policy, Clan.PlayerClan.Kingdom.ActivePolicies.Contains(policy.Policy)), out float elapsedDaysUntilNow);
                    __instance.CanProposeOrDisavowPolicy = __instance.CanProposeOrDisavowPolicy && !hasCooldown;
                    if (hasCooldown)
                    {
                        string cooldownText = "\n" + StringHelper.GetCooldownText(typeof(KingdomPolicyDecision), elapsedDaysUntilNow).ToString();
                        __instance.ProposeActionExplanationText += !__instance.ProposeActionExplanationText.Contains(cooldownText) ? cooldownText : string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                DebugHelper.HandleException(ex, methodInfo, "Harmony patch for KingdomPoliciesVM. OnPolicySelect");
            }
        }

        public static bool Prepare()
        {
            return SettingsHelper.SubSystemEnabled(SubSystemType.ElectionCooldownsForPlayer);
        }
    }
}