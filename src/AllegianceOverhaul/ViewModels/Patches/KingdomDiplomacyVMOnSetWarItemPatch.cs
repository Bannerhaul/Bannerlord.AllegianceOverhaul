using AllegianceOverhaul.CampaignBehaviors.BehaviorManagers;
using AllegianceOverhaul.Helpers;

using HarmonyLib;

using System;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
#if e172
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.KingdomDiplomacy;
#else
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Diplomacy;
#endif


namespace AllegianceOverhaul.ViewModels.Patches
{
    [HarmonyPatch(typeof(KingdomDiplomacyVM), "OnSetWarItem")]
    public static class KingdomDiplomacyVMOnSetWarItemPatch
    {
        [HarmonyPostfix]
        public static void OnSetWarItemPatch(KingdomWarItemVM item, KingdomDiplomacyVM __instance)
        {
            try
            {
                if (SettingsHelper.SubSystemEnabled(SubSystemType.ElectionCooldownsForPlayer))
                {
                    bool HasCooldown = AOCooldownManager.HasDecisionCooldown(new MakePeaceKingdomDecision(Clan.PlayerClan, item.Faction2, applyResults: false), out float elapsedDaysUntilNow);
                    __instance.IsActionEnabled = __instance.IsActionEnabled && !HasCooldown;
                    __instance.ProposeActionExplanationText += HasCooldown ? "\n" + StringHelper.GetCooldownText(typeof(MakePeaceKingdomDecision), elapsedDaysUntilNow).ToString() : string.Empty;
                }
            }
            catch (Exception ex)
            {
                MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                DebugHelper.HandleException(ex, methodInfo, "Harmony patch for KingdomDiplomacyVM. OnSetWarItem");
            }
        }

        public static bool Prepare()
        {
            return SettingsHelper.SubSystemEnabled(SubSystemType.ElectionCooldownsForPlayer);
        }
    }
}