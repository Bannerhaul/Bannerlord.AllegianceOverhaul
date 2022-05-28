﻿using AllegianceOverhaul.Helpers;

using HarmonyLib;

using System;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors.BarterBehaviors;

namespace AllegianceOverhaul.Patches.Loyalty
{
    [HarmonyPatch(typeof(DiplomaticBartersBehavior), "ConsiderClanLeaveAsMercenary")]
    public static class ConsiderClanLeaveAsMercenaryPatch
    {
        [HarmonyPrefix]
        [HarmonyPriority(Priority.High)]
        public static void DebugPrefix(Clan clan) //void prefixes are guaranteed to run
        {
            try
            {
                LoyaltyRebalance.LoyaltyDebugHelper.LeaveKingdomDebug(clan);
            }
            catch (Exception ex)
            {
                MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                DebugHelper.HandleException(ex, methodInfo, "Harmony patch for ConsiderClanLeaveAsMercenary");
            }
        }

        [HarmonyPriority(Priority.HigherThanNormal)]
        public static bool Prefix(Clan clan) //Bool prefixes compete with each other and skip others, as well as original, if return false
        {
            try
            {
                return !LoyaltyRebalance.EnsuredLoyalty.LoyaltyManager.CheckLoyalty(clan);
            }
            catch (Exception ex)
            {
                MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                DebugHelper.HandleException(ex, methodInfo, "Harmony patch for ConsiderClanLeaveAsMercenary");
                return true;
            }
        }

        public static bool Prepare()
        {
            return Settings.Instance!.UseEnsuredLoyalty || SettingsHelper.SystemDebugEnabled(AOSystems.EnsuredLoyalty, DebugType.Any);
        }
    }
}