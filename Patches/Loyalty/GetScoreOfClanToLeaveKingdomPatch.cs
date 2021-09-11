extern alias TWCS;

using AllegianceOverhaul.Helpers;

using HarmonyLib;

using System;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;

namespace AllegianceOverhaul.Patches.Loyalty
{
    [HarmonyPatch(typeof(DefaultDiplomacyModel), "GetScoreOfClanToLeaveKingdom")]
    public static class GetScoreOfClanToLeaveKingdomPatch
    {
        public static void Postfix(Clan clan, Kingdom kingdom, ref float __result)
        {
            try
            {
                if (SettingsHelper.SubSystemEnabled(SubSystemType.DestabilizeLeaveEvaluation))
                {
                    __result += Settings.Instance!.LeaveScoreFlatModifier * 1000000f;
                }
            }
            catch (Exception ex)
            {
                MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                DebugHelper.HandleException(ex, methodInfo, "Harmony patch for GetScoreOfClanToLeaveKingdom");
            }
        }
        public static bool Prepare()
        {
            return SettingsHelper.SubSystemEnabled(SubSystemType.DestabilizeLeaveEvaluation);
        }
    }
}