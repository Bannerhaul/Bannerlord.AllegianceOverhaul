using TaleWorlds.CampaignSystem;

namespace AllegianceOverhaul.PoliticsRebalance
{
  public class DecisionMaker
  {
    public DecisionMaker(Hero hero, double decisionWeight)
    {
      Hero = hero;
      DecisionWeight = decisionWeight;
    }
    public Hero Hero { get; }
    public double DecisionWeight { get; }
  }
}
