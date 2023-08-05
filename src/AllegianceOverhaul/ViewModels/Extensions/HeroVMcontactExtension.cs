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
#if v100 || v101 || v102 || v103 || v110 || v111 || v112 || v113 || v114 || v115
                InformationManager.ShowTooltip(typeof(Hero), Hero, OtherHero, false);
#else
                InformationManager.ShowTooltip(typeof(Hero), Hero, false, OtherHero);
#endif
            else
                InformationManager.ShowTooltip(typeof(Hero), Hero, false);
        }
    }
}