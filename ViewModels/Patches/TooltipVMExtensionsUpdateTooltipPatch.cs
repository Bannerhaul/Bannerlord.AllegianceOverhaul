using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Library;
using AllegianceOverhaul.LoyaltyRebalance.EnsuredLoyalty;

namespace AllegianceOverhaul.ViewModels.Patches
{
  [HarmonyPatch(typeof(TooltipVMExtensions), "UpdateTooltip", new[] { typeof(TooltipVM), typeof(Hero) })]
  public class TooltipVMExtensionsUpdateTooltipPatch
  {
    [HarmonyPostfix]
    public static void UpdateTooltipPatch(TooltipVM tooltipVM, Hero hero)
    {
      try
      {
        if (!Settings.Instance.UseAdvancedHeroTooltips || !Settings.Instance.UseEnsuredLoyalty || hero.Clan is null || !SettingsHelper.FactionInScope(hero.Clan, Settings.Instance.EnsuredLoyaltyScope))
          return;

        if (hero.Clan.Kingdom != null && hero.Clan.Kingdom.RulingClan != hero.Clan && hero == hero.Clan.Leader)
        {
          int RelationWithLiege = hero.GetRelation(hero.Clan.Kingdom.Ruler);
          tooltipVM.TooltipPropertyList.Add(new TooltipProperty(TooltipHelper.GetTooltipRelationHeader(hero.Clan.Kingdom.Ruler), RelationWithLiege.ToString("N0"), 0, RelationWithLiege < -10 ? Colors.Red : RelationWithLiege > 10 ? Colors.Green : TooltipHelper.DefaultTooltipColor, false, TooltipProperty.TooltipPropertyFlags.None));

          LoyaltyManager.GetLoyaltyTooltipInfo(hero.Clan, out string LoyaltyText, out Color LoyaltyTextColor);
          tooltipVM.TooltipPropertyList.Add(new TooltipProperty(TooltipHelper.GetTooltipLoyaltyHeader(), LoyaltyText, 0, LoyaltyTextColor, false, TooltipProperty.TooltipPropertyFlags.None));
        }
      }
      catch (Exception ex)
      {
        MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
        DebugHelper.HandleException(ex, methodInfo, "Harmony patch for TooltipVMExtensions.UpdateTooltip");
      }
    }
    public static bool Prepare()
    {
      return Settings.Instance.UseAdvancedHeroTooltips && Settings.Instance.UseEnsuredLoyalty;
    }
  }
}
