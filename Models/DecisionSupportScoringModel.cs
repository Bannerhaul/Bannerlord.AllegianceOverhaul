using System.Collections.Generic;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Core;

using AllegianceOverhaul.PoliticsRebalance;

namespace AllegianceOverhaul.Models
{
  public abstract class DecisionSupportScoringModel : GameModel
  {
    public virtual float DetermineSupport(Clan clan, KingdomDecision decision, DecisionOutcome possibleOutcome)
    {
      return GetGeneralSupportScore(clan, decision, possibleOutcome) + GetCollectiveSupportScore(clan, decision, possibleOutcome);
    }

    public virtual float DetermineSupport(Hero hero, KingdomDecision decision, DecisionOutcome possibleOutcome)
    {
      return (GetGeneralSupportScore(hero.Clan, decision, possibleOutcome) + (float)GetSupportScoreOfDecisionMaker(new DecisionMaker(hero, 1), decision, possibleOutcome));
    }

    public abstract IEnumerable<DecisionMaker> GetDecisionMakers(Clan clan);

    public abstract float CalculateWarSuccessScore(Kingdom decidingKingdom, IFaction factionAtWar);

    public abstract int GetNumberOfFiefsDesired(DecisionMaker decisionMaker);

    public abstract int GetNumberOfFiefsDeemedFair(Clan clan);

    protected abstract float GetGeneralSupportScore(Clan clan, KingdomDecision decision, DecisionOutcome possibleOutcome);

    protected virtual float GetCollectiveSupportScore(Clan clan, KingdomDecision decision, DecisionOutcome possibleOutcome)
    {
      Dictionary<DecisionMaker, double> personalScores = GetDecisionMakersSupportScores(clan, decision, possibleOutcome);
      return personalScores.Count <= 1
          ? (float)personalScores.FirstOrDefault().Value
          : (float)(personalScores.Sum(x => x.Value) / personalScores.Sum(x => x.Key.DecisionWeight));
    }

    protected virtual Dictionary<DecisionMaker, double> GetDecisionMakersSupportScores(Clan clan, KingdomDecision decision, DecisionOutcome possibleOutcome)
    {
      return GetDecisionMakers(clan).ToDictionary(keySelector: dm => dm, elementSelector: dm => GetSupportScoreOfDecisionMaker(dm, decision, possibleOutcome));
    }

    protected abstract double GetSupportScoreOfDecisionMaker(DecisionMaker decisionMaker, KingdomDecision decision, DecisionOutcome possibleOutcome);

    protected abstract int CalculateBaseNumberOfFiefs(Clan clan, NumberOfFiefsCalculationMethod calculationMethod);
  }
}
