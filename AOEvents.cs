using TaleWorlds.CampaignSystem;

using AllegianceOverhaul.SavableClasses;

using Bannerlord.ButterLib.Common.Extensions;

namespace AllegianceOverhaul
{
  public class AOEvents
  {
    private readonly MbEvent<Hero, Hero, SegmentalFractionalScore> _relationShift = new MbEvent<Hero, Hero, SegmentalFractionalScore>();
    private readonly MbEvent<Hero, Hero, SegmentalFractionalScore, string> _relationShifted = new MbEvent<Hero, Hero, SegmentalFractionalScore, string>();

    public static AOEvents? Instance { get; internal set; }

    public static IMbEvent<Hero, Hero, SegmentalFractionalScore> OnRelationShiftEvent => Instance!._relationShift;
    internal void OnRelationShift(Hero baseHero, Hero otherHero, SegmentalFractionalScore fractionalScore)
    {
      MbEventExtensions.Invoke(Instance!._relationShift, baseHero, otherHero, fractionalScore);
    }

    public static IMbEvent<Hero, Hero, SegmentalFractionalScore, string> OnRelationShiftedEvent => Instance!._relationShifted;
    internal void OnRelationShifted(Hero baseHero, Hero otherHero, SegmentalFractionalScore fractionalScore, string explanation)
    {
      MbEventExtensions.Invoke(Instance!._relationShifted, baseHero, otherHero, fractionalScore, explanation);
    }
  }
}