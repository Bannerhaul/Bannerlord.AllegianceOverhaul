using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection;

namespace AllegianceOverhaul.ViewModels.Extensions
{
  public static class TooltipVMExtension
  {
    public static void UpdateTooltip(this TooltipVM tooltipVM, Hero? hero, Hero? otherHero)
    {
      tooltipVM.UpdateTooltip(hero);
      if (otherHero != null)
        tooltipVM.TooltipPropertyList.Add(new TooltipProperty(TooltipHelper.GetTooltipRelationHeader(otherHero), hero?.GetRelation(otherHero).ToString("N0"), 0, false, TooltipProperty.TooltipPropertyFlags.None));
    }
  }
}
