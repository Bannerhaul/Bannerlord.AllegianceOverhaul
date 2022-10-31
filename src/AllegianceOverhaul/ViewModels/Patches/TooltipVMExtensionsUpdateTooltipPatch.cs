using AllegianceOverhaul.Extensions;
using AllegianceOverhaul.Helpers;
using AllegianceOverhaul.LoyaltyRebalance.EnsuredLoyalty;

using HarmonyLib;

using System;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace AllegianceOverhaul.ViewModels.Patches
{
    [HarmonyPatch(typeof(PropertyBasedTooltipVMExtensions), "UpdateTooltip", new[] { typeof(PropertyBasedTooltipVM), typeof(Hero), typeof(bool) })]
    public static class TooltipVMExtensionsUpdateTooltipPatch
    {
        [HarmonyPostfix]
        public static void UpdateTooltipPatch(PropertyBasedTooltipVM propertyBasedTooltipVM, Hero hero, bool isNear)
        {
            var tooltipVM = propertyBasedTooltipVM;
            try
            {
                if (hero.Clan is null || !SettingsHelper.SubSystemEnabled(SubSystemType.LoyaltyTooltips, hero.Clan))
                {
                    return;
                }

                if (hero.Clan.Kingdom != null && !hero.Clan.IsRulingClan() && hero == hero.Clan.Leader)
                {
                    int RelationWithLiege = hero.GetRelation(hero.Clan.Kingdom.Leader);
                    tooltipVM.TooltipPropertyList.Add(new TooltipProperty(TooltipHelper.GetTooltipRelationHeader(hero.Clan.Kingdom.Leader), RelationWithLiege.ToString("N0"), 0, RelationWithLiege < -10 ? Colors.Red : RelationWithLiege > 10 ? Colors.Green : TooltipHelper.DefaultTooltipColor, false, TooltipProperty.TooltipPropertyFlags.None));

                    LoyaltyManager.GetLoyaltyTooltipInfo(hero.Clan, out string LoyaltyText, out Color LoyaltyTextColor);
                    tooltipVM.TooltipPropertyList.Add(new TooltipProperty(TooltipHelper.GetTooltipLoyaltyHeader(), LoyaltyText, 0, LoyaltyTextColor, false, TooltipProperty.TooltipPropertyFlags.None));
                }
            }
            catch (Exception ex)
            {
                MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                DebugHelper.HandleException(ex, methodInfo, "Harmony patch for TooltipVMExtensions.UpdateTooltip");
            }
        }
        public static bool Prepare()
        {
            return SettingsHelper.SubSystemEnabled(SubSystemType.LoyaltyTooltips);
        }
    }
}