using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.SaveSystem;

namespace AllegianceOverhaul.SavableClasses
{
  //[SaveableClass(1)]
  internal class KingdomDecisionConclusion
  {
    [SaveableProperty(1)]
    public DecisionOutcome ChosenOutcome { get; private set; }
    [SaveableProperty(2)]
    public CampaignTime ConclusionTime { get; private set; }

    public KingdomDecisionConclusion (DecisionOutcome outcome, CampaignTime conclusionTime)
    {
      ChosenOutcome = outcome;
      ConclusionTime = conclusionTime;
    }
  }
}
