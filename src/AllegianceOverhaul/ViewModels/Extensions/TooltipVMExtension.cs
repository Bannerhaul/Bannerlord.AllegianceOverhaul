#if v100 || v101 || v102 || v103 || v110 || v111 || v112 || v113 || v114 || v115
using AllegianceOverhaul.Extensions;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Information;

namespace AllegianceOverhaul.ViewModels.Extensions
{
    public static class PropertyBasedTooltipVMExtension
    {
        public static void UpdateTooltip(this PropertyBasedTooltipVM tooltipVM, Hero? hero, Hero? otherHero, bool isNear)
        {
            tooltipVM.UpdateTooltip(hero, isNear);
            if (otherHero != null)
                tooltipVM.TooltipPropertyList.Add(new TooltipProperty(TooltipHelper.GetTooltipRelationHeader(otherHero), hero?.GetModifiedRelation(otherHero).ToString("N0"), 0, false, TooltipProperty.TooltipPropertyFlags.None));
        }
    }
}
#endif