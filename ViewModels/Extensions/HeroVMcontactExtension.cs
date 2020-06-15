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

    protected override void ExecuteBeginHint()
    {
      if (this.Hero == null)
        return;
      if (this.OtherHero != null)
        InformationManager.AddTooltipInformation(typeof(Hero), this.Hero, this.OtherHero);
      else
        InformationManager.AddTooltipInformation(typeof(Hero), this.Hero);
    }
  }
}
