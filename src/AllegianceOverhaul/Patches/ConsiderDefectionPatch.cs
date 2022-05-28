using AllegianceOverhaul.Helpers;
using AllegianceOverhaul.MigrationTweaks;

using HarmonyLib;

using System;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.CampaignSystem.CampaignBehaviors.BarterBehaviors;

namespace AllegianceOverhaul.Patches
{
    [HarmonyPatch(typeof(DiplomaticBartersBehavior), "ConsiderDefection")]
    public static class ConsiderDefectionPatch
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
                if (kingdom.Leader != Hero.MainHero)
                {
                    return true;
                }
                if (!SettingsHelper.SubSystemEnabled(SubSystemType.AllowJoinRequests) || clan1.IsUnderMercenaryService)
                {
                    return false;
                }

                if (!SettingsHelper.SubSystemEnabled(SubSystemType.AlwaysPickPlayerKingdom))
                {
                    JoinKingdomAsClanBarterable asClanBarterable = new(clan1.Leader, kingdom);
                    int valueForClan = asClanBarterable.GetValueForFaction(clan1);
                    int valueForKingdom = asClanBarterable.GetValueForFaction(kingdom);
                    int checkSum = valueForClan + valueForKingdom;
                    int num2 = (valueForClan < 0) ? -valueForClan : 0;
                    if (checkSum <= 0 || num2 > kingdom.Leader.Gold * 0.5)
                    {
                        return false;
                    }
                }

                MigrationManager.AwaitPlayerDecision(clan1);
                return false;
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
            return Settings.Instance!.UseEnsuredLoyalty || Settings.Instance!.UseMigrationTweaks || SettingsHelper.SystemDebugEnabled(AOSystems.EnsuredLoyalty, DebugType.Any);
        }
    }
}