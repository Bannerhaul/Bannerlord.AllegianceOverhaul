using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
#if e172
using TaleWorlds.Core;
#else
using TaleWorlds.Library;
#endif

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
#if e172
                InformationManager.AddTooltipInformation(typeof(Hero), Hero, OtherHero);
            else
                InformationManager.AddTooltipInformation(typeof(Hero), Hero);
#else
                InformationManager.ShowTooltip(typeof(Hero), Hero, OtherHero, false);
            else
                InformationManager.ShowTooltip(typeof(Hero), Hero, false);
#endif
        }
    }
}