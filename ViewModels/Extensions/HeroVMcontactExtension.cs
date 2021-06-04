using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;

namespace AllegianceOverhaul.ViewModels.Extensions
{
  public class HeroVMcontactExtension : HeroVM
  {
    public Hero OtherHero { get; }
    public HeroVMcontactExtension(Hero hero, Hero otherHero) : base(hero)
    {
      OtherHero = otherHero;
    }

    public override void ExecuteBeginHint()
    {
      if (Hero == null)
        return;
      if (OtherHero != null)
        InformationManager.AddTooltipInformation(typeof(Hero), Hero, OtherHero);
      else
        InformationManager.AddTooltipInformation(typeof(Hero), Hero);
    }
  }
}
