using AllegianceOverhaul.Helpers;

using HarmonyLib;

using System;
using System.Reflection;

using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;

namespace AllegianceOverhaul.Patches
{
    [HarmonyPatch(typeof(DefaultDiplomacyModel), "DenarsToInfluence")]
    public static class DenarsToInfluencePatch
    {
        public static void Postfix(ref float __result)
        {
            try
            {
                __result = 1f / Settings.Instance!.InfluenceToDenars;
            }
            catch (Exception ex)
            {
                MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                DebugHelper.HandleException(ex, methodInfo, "Harmony patch for DefaultDiplomacyModel. DenarsToInfluence");
            }
        }
    }
}