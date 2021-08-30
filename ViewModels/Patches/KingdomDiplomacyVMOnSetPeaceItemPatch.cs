using AllegianceOverhaul.CampaignBehaviors.BehaviorManagers;
using AllegianceOverhaul.Helpers;

using HarmonyLib;

using System;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.KingdomDiplomacy;

namespace AllegianceOverhaul.ViewModels.Patches
{
    [HarmonyPatch(typeof(KingdomDiplomacyVM), "OnSetPeaceItem")]
    public static class KingdomDiplomacyVMOnSetPeaceItemPatch
    {
        [HarmonyPostfix]
        public static void OnSetPeaceItemPatch(KingdomTruceItemVM item, KingdomDiplomacyVM __instance)
        {
            try
            {
                if (SettingsHelper.SubSystemEnabled(SubSystemType.ElectionCooldownsForPlayer))
                {
                    bool HasCooldown = AOCooldownManager.HasDecisionCooldown(new DeclareWarDecision(Clan.PlayerClan, item.Faction2), out float elapsedDaysUntilNow);
                    __instance.IsActionEnabled = __instance.IsActionEnabled && !HasCooldown;
                    __instance.ProposeActionExplanationText += HasCooldown ? "\n" + StringHelper.GetCooldownText(typeof(DeclareWarDecision), elapsedDaysUntilNow).ToString() : string.Empty;
                }
            }
            catch (Exception ex)
            {
                MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                DebugHelper.HandleException(ex, methodInfo, "Harmony patch for KingdomDiplomacyVM. OnSetPeaceItem");
            }
        }

        public static bool Prepare()
        {
            return SettingsHelper.SubSystemEnabled(SubSystemType.ElectionCooldownsForPlayer);
        }
    }
}
