using HarmonyLib;

using System;
using System.Reflection;

using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;

using AllegianceOverhaul.Helpers;
using AllegianceOverhaul.ViewModels.Extensions;

namespace AllegianceOverhaul.ViewModels.Patches
{
  [HarmonyPatch(typeof(TooltipVMExtensions), "HeroAction")]
  public static class TooltipVMExtensionsHeroActionPatch
  {
    [HarmonyPrefix]
    public static bool HeroActionPatch(TooltipVM tooltipVM, object[] args)
    {
      try
      {
        if (!Settings.Instance.UseAdvancedHeroTooltips)
          return true;
        if (args.Length == 2 && args[1] != null && args[1] is Hero)
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
