using AllegianceOverhaul.Helpers;
using AllegianceOverhaul.MigrationTweaks;

using HarmonyLib;

using System;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Barterables;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.BarterBehaviors;

namespace AllegianceOverhaul.Patches.Migration
{
    [HarmonyPatch(typeof(DiplomaticBartersBehavior), "ConsiderClanJoin")]
    public static class ConsiderClanJoinPatch
    {
        public static bool Prefix(Clan clan, Kingdom kingdom) //Bool prefixes compete with each other and skip others, as well as original, if return false
        {
            try
            {
                if (kingdom.Leader != Hero.MainHero)
                {
                    return true;
                }
                if (!SettingsHelper.SubSystemEnabled(SubSystemType.AllowJoinRequests) || clan.IsMinorFaction)
                {
                    return false;
                }

                if (!SettingsHelper.SubSystemEnabled(SubSystemType.AlwaysPickPlayerKingdom))
                {
                    JoinKingdomAsClanBarterable asClanBarterable = new(clan.Leader, kingdom);
                    int valueForClan = asClanBarterable.GetValueForFaction(clan);
                    if ((valueForClan <= 0) && (valueForClan + asClanBarterable.GetValueForFaction(kingdom) <= 0))
                    {
                        return false;
                    }
                }

                MigrationManager.AwaitPlayerDecision(clan);
                return false;
            }
            catch (Exception ex)
            {
                MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                DebugHelper.HandleException(ex, methodInfo, "Harmony patch for ConsiderClanJoin");
                return true;
            }
        }

        public static bool Prepare()
        {
            return Settings.Instance!.UseMigrationTweaks;
        }
    }
}