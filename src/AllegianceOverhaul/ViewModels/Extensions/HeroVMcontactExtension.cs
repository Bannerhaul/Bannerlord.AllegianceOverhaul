using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Library;

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
                InformationManager.ShowTooltip(typeof(Hero), Hero, OtherHero, false);
            else
                InformationManager.ShowTooltip(typeof(Hero), Hero, false);
        }
    }
}