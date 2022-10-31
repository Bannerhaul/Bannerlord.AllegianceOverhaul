using AllegianceOverhaul.Helpers;
using AllegianceOverhaul.ViewModels.Extensions;

using HarmonyLib;

using System;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Information;

namespace AllegianceOverhaul.ViewModels.Patches
{
    [HarmonyPatch(typeof(PropertyBasedTooltipVMExtensions), "HeroAction")]
    public static class TooltipVMExtensionsHeroActionPatch
    {
        [HarmonyPrefix]
        public static bool HeroActionPatch(PropertyBasedTooltipVM propertyBasedTooltipVM, object[] args)
        {
            try
            {
                if (!Settings.Instance!.UseAdvancedHeroTooltips)
                    return true;
                if (args.Length == 3 && args[1] != null && args[1] is Hero)
                    propertyBasedTooltipVM.UpdateTooltip(args[0] as Hero, args[1] as Hero, (bool) args[2]);
                else
                    propertyBasedTooltipVM.UpdateTooltip(args[0] as Hero, (bool) args[1]);
                return false;
            }
            catch (Exception ex)
            {
                MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                DebugHelper.HandleException(ex, methodInfo, "Harmony patch for TooltipVMExtensions.HeroAction");
                return true;
            }
        }
        public static bool Prepare()
        {
            return Settings.Instance!.UseAdvancedHeroTooltips;
        }
    }
}