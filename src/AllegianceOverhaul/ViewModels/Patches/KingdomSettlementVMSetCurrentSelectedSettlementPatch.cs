using AllegianceOverhaul.CampaignBehaviors.BehaviorManagers;
using AllegianceOverhaul.Helpers;

using HarmonyLib;

using System;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Settlements;

namespace AllegianceOverhaul.ViewModels.Patches
{
    [HarmonyPatch(typeof(KingdomSettlementVM), "SetCurrentSelectedSettlement")]
    public static class KingdomSettlementVMSetCurrentSelectedSettlementPatch
    {
        [HarmonyPostfix]
        public static void SetCurrentSelectedSettlementPatch(KingdomSettlementItemVM settlementItem, KingdomSettlementVM __instance)
        {
            try
            {
                if (SettingsHelper.SubSystemEnabled(SubSystemType.ElectionCooldownsForPlayer))
                {
                    bool HasCooldown = AOCooldownManager.HasDecisionCooldown(new SettlementClaimantPreliminaryDecision(Clan.PlayerClan, settlementItem.Settlement), out float elapsedDaysUntilNow);
                    __instance.CanAnnexCurrentSettlement = __instance.CanAnnexCurrentSettlement && !HasCooldown;
                    __instance.AnnexActionExplanationText += HasCooldown ? "\n" + StringHelper.GetCooldownText(typeof(SettlementClaimantPreliminaryDecision), elapsedDaysUntilNow).ToString() : string.Empty;
                }
            }
            catch (Exception ex)
            {
                MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                DebugHelper.HandleException(ex, methodInfo, "Harmony patch for KingdomSettlementVM. SetCurrentSelectedSettlement");
            }
        }

        public static bool Prepare()
        {
            return SettingsHelper.SubSystemEnabled(SubSystemType.ElectionCooldownsForPlayer);
        }
    }
}