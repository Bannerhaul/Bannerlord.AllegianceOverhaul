using HarmonyLib;

using System;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;

using AllegianceOverhaul.Helpers;
using AllegianceOverhaul.ViewModels.Extensions;

namespace AllegianceOverhaul.ViewModels.Patches
{
  [HarmonyPatch(typeof(EncyclopediaHeroPageVM), "Refresh")]
  public static class EncyclopediaHeroPageVMRefreshPatch
  {
    [HarmonyPostfix]
    public static void RefreshPatch(EncyclopediaHeroPageVM __instance)
    {
      try
      {
        Hero? PageHero = __instance.Obj as Hero;
        if (!Settings.Instance!.UseAdvancedHeroTooltips || PageHero == Hero.MainHero)
        {
          return;
        }
        __instance.Allies.Clear();
        __instance.Enemies.Clear();
        EncyclopediaPage pageOf1 = Campaign.Current.EncyclopediaManager.GetPageOf(typeof(Hero));
        foreach (Hero hero in Hero.All)
        {
          if (pageOf1.IsValidEncyclopediaItem(hero) && !hero.IsNotable && hero != PageHero)
          {
            if (PageHero!.IsFriend(hero))
            {
              __instance.Allies.Add(new HeroVMcontactExtension(hero, PageHero));
            }
            else if (PageHero.IsEnemy(hero))
            {
              __instance.Enemies.Add(new HeroVMcontactExtension(hero, PageHero));
            }
          }
        }
      }
      catch (Exception ex)
      {
        MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
        DebugHelper.HandleException(ex, methodInfo, "Harmony patch for EncyclopediaHeroPageVM.Refresh");
      }
    }
    public static bool Prepare()
    {
      return Settings.Instance!.UseAdvancedHeroTooltips;
    }
  }
}
