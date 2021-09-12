using AllegianceOverhaul.Helpers;

using HarmonyLib;

using System;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;

namespace AllegianceOverhaul.Patches.Loyalty
{
    [HarmonyPatch(typeof(DefaultDiplomacyModel), "GetScoreOfClanToJoinKingdom")]
    public static class GetScoreOfClanToJoinKingdomPatch
    {
        public static void Postfix(Clan clan, Kingdom kingdom, ref float __result)
        {
            try
            {
                if (SettingsHelper.SubSystemEnabled(SubSystemType.DestabilizeJoinEvaluation))
                {
                    __result += Settings.Instance!.JoinScoreFlatModifier * 1000000f;
                }
            }
            catch (Exception ex)
            {
                MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                DebugHelper.HandleException(ex, methodInfo, "Harmony patch for GetScoreOfClanToJoinKingdom");
            }
        }
        public static bool Prepare()
        {
            return SettingsHelper.SubSystemEnabled(SubSystemType.DestabilizeJoinEvaluation);
        }
    }
}