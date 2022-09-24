using AllegianceOverhaul.Extensions;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
#if e172
using TaleWorlds.Core.ViewModelCollection;
#else
using TaleWorlds.Core.ViewModelCollection.Information;
#endif

namespace AllegianceOverhaul.ViewModels.Extensions
{
#if e172
    public static class TooltipVMExtension
    {
        public static void UpdateTooltip(this TooltipVM tooltipVM, Hero? hero, Hero? otherHero)
        {
            tooltipVM.UpdateTooltip(hero);
#else
    public static class PropertyBasedTooltipVMExtension
    {
        public static void UpdateTooltip(this PropertyBasedTooltipVM tooltipVM, Hero? hero, Hero? otherHero, bool isNear)
        {
            tooltipVM.UpdateTooltip(hero, isNear);
#endif
            if (otherHero != null)
                tooltipVM.TooltipPropertyList.Add(new TooltipProperty(TooltipHelper.GetTooltipRelationHeader(otherHero), hero?.GetModifiedRelation(otherHero).ToString("N0"), 0, false, TooltipProperty.TooltipPropertyFlags.None));
        }
    }
}