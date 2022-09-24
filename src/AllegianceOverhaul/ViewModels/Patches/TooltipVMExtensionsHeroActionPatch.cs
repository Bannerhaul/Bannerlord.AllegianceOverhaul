using AllegianceOverhaul.Helpers;
using AllegianceOverhaul.ViewModels.Extensions;

using HarmonyLib;

using System;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
#if e172
using TaleWorlds.Core.ViewModelCollection;
#else
using TaleWorlds.Core.ViewModelCollection.Information;
#endif

namespace AllegianceOverhaul.ViewModels.Patches
{
#if e172
    [HarmonyPatch(typeof(TooltipVMExtensions), "HeroAction")]
#else
    [HarmonyPatch(typeof(PropertyBasedTooltipVMExtensions), "HeroAction")]
#endif
    public static class TooltipVMExtensionsHeroActionPatch
    {
        [HarmonyPrefix]
#if e172
        public static bool HeroActionPatch(TooltipVM tooltipVM, object[] args)
#else
        public static bool HeroActionPatch(PropertyBasedTooltipVM propertyBasedTooltipVM, object[] args)
#endif
        {
            try
            {
                if (!Settings.Instance!.UseAdvancedHeroTooltips)
                    return true;
#if e172
                if (args.Length == 2 && args[1] != null && args[1] is Hero)
                    tooltipVM.UpdateTooltip(args[0] as Hero, args[1] as Hero);
#else
                if (args.Length == 3 && args[1] != null && args[1] is Hero)
                    propertyBasedTooltipVM.UpdateTooltip(args[0] as Hero, args[1] as Hero, (bool) args[2]);
                else
                    propertyBasedTooltipVM.UpdateTooltip(args[0] as Hero, (bool) args[1]);
#endif
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