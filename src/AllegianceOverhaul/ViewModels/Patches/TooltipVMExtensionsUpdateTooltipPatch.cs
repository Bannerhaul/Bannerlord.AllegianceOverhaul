using AllegianceOverhaul.Extensions;
using AllegianceOverhaul.Helpers;
using AllegianceOverhaul.LoyaltyRebalance.EnsuredLoyalty;
using AllegianceOverhaul.ViewModels.Extensions;

using HarmonyLib;

using System;
using System.Reflection;
using System.Runtime.ConstrainedExecution;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace AllegianceOverhaul.ViewModels.Patches
{
#if v100 || v101 || v102 || v103 || v110 || v111 || v112 || v113 || v114 || v115
    [HarmonyPatch(typeof(PropertyBasedTooltipVMExtensions), "UpdateTooltip", new[] { typeof(PropertyBasedTooltipVM), typeof(Hero), typeof(bool) })]
    public static class TooltipVMExtensionsUpdateTooltipPatch
    {
        [HarmonyPostfix]
        public static void UpdateTooltipPatch(PropertyBasedTooltipVM propertyBasedTooltipVM, Hero hero, bool isNear)
        {
#else
    [HarmonyPatch(typeof(TooltipRefresherCollection), "RefreshHeroTooltip")]
    public static class TooltipVMExtensionsUpdateTooltipPatch
    {
        [HarmonyPostfix]
        public static void RefreshHeroTooltip(PropertyBasedTooltipVM propertyBasedTooltipVM, object[] args)
        {
            Hero? hero = args[0] as Hero;
            if (hero is null)
                return;
            //bool isNear = (bool) args[1];
#endif
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
#if v120 || v121 || v122 || v123
                Hero? otherHero = null;
                if (args.Length >= 3 && args[2] != null && args[2] is Hero)
                    otherHero = args[2] as Hero;

                if (otherHero != null)
                    tooltipVM.TooltipPropertyList.Add(new TooltipProperty(TooltipHelper.GetTooltipRelationHeader(otherHero), hero?.GetModifiedRelation(otherHero).ToString("N0"), 0, false, TooltipProperty.TooltipPropertyFlags.None));
#endif
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