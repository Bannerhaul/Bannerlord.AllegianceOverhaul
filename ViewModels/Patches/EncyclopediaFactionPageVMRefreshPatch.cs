using AllegianceOverhaul.Extensions;
using AllegianceOverhaul.Helpers;

using HarmonyLib;

using System;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;

namespace AllegianceOverhaul.ViewModels.Patches
{
    [HarmonyPatch(typeof(EncyclopediaFactionPageVM), "Refresh")]
    public static class EncyclopediaFactionPageVMRefreshPatch
    {
        [HarmonyPostfix]
        public static void RefreshPatch(EncyclopediaFactionPageVM __instance, Kingdom ____faction)
        {
            try
            {
                if (SettingsHelper.SubSystemEnabled(SubSystemType.LeaderDefectionFix) && ____faction.IsEliminated)
                {
                    __instance.LeaderText = "{=DvvoBmrEZ}Former ".ToLocalizedString() + __instance.LeaderText;
                    __instance.NameText += "{=bdIYWo2R8} (eliminated)".ToLocalizedString();
                    __instance.Settlements.Clear();
                    __instance.InformationText += "\n \n" + "{=QLIrm7VAp}This faction was unable to cope with the variety of internal and external problems. It now belongs to history.".ToLocalizedString();
                }
            }
            catch (Exception ex)
            {
                MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                DebugHelper.HandleException(ex, methodInfo, "Harmony patch for EncyclopediaFactionPageVM.Refresh");
            }
        }
        public static bool Prepare()
        {
            return Settings.Instance!.UseMigrationTweaks;
        }
    }
}
