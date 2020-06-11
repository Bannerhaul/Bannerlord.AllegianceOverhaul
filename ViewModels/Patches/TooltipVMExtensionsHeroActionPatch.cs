using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using AllegianceOverhaul.ViewModels.Extensions;

namespace AllegianceOverhaul.ViewModels.Patches
{
  [HarmonyPatch(typeof(TooltipVMExtensions), "HeroAction")]
  class TooltipVMExtensionsHeroActionPatch
  {
    [HarmonyPrefix]
    public static bool HeroActionPatch(TooltipVM tooltipVM, object[] args)
    {
      try
      {
        if (!Settings.Instance.UseAdvancedHeroTooltips)
          return true;
        if (args.Cast<Object>().ToList().Count == 2)
          tooltipVM.UpdateTooltip(args[0] as Hero, args[1] as Hero);
        else
          tooltipVM.UpdateTooltip(args[0] as Hero);
        return false;
      }
      catch (Exception ex)
      {
        MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
        DebugHelper.HandleException(ex, methodInfo, "Harmony patch for TooltipVMExtensions.HeroAction");
        return true;
      }
    }
    public static bool Prepare()
    {
      return Settings.Instance.UseAdvancedHeroTooltips;
    }
  }
}
