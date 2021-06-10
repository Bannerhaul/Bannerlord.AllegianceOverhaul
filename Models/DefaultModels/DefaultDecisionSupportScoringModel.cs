using System;
using System.Collections.Generic;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Barterables;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Library;

using AllegianceOverhaul.Extensions;
using AllegianceOverhaul.Helpers;
using AllegianceOverhaul.PoliticsRebalance;

namespace AllegianceOverhaul.Models.DefaultModels
{
  public class DefaultDecisionSupportScoringModel : DecisionSupportScoringModel
  {
    private delegate float CalculateShareFactorDelegate(Clan clan);
    private static readonly CalculateShareFactorDelegate? deCalculateShareFactor = AccessHelper.GetDelegate<CalculateShareFactorDelegate>(typeof(DefaultClanFinanceModel), "CalculateShareFactor");

    //Public overrides
    public override IEnumerable<DecisionMaker> GetDecisionMakers(Clan clan)
    {
      yield return new DecisionMaker(clan.Leader, 1);
    }

    public override float CalculateWarSuccessScore(Kingdom decidingKingdom, IFaction factionAtWar)
    {
      if (factionAtWar.IsKingdomFaction)
      {
        StanceLink stanceWith = decidingKingdom.GetStanceWith(factionAtWar);
        int decidingKingdomScore = stanceWith.GetSuccessfulSieges(decidingKingdom) * 1000
                                   + factionAtWar.Heroes.Count(h => h.IsPrisoner && h.PartyBelongedToAsPrisoner?.MapFaction == decidingKingdom) * 100
                                   + stanceWith.GetSuccessfulRaids(decidingKingdom) * 25
                                   + stanceWith.GetCasualties(factionAtWar);
        int factionAtWarScore = stanceWith.GetSuccessfulSieges(factionAtWar) * 1000
                                + decidingKingdom.Heroes.Count(h => h.IsPrisoner && h.PartyBelongedToAsPrisoner?.MapFaction == factionAtWar) * 100
                                + stanceWith.GetSuccessfulRaids(factionAtWar) * 25
                                + stanceWith.GetCasualties(decidingKingdom);
        return decidingKingdomScore - factionAtWarScore;
      }
      return 0;
    }
    
    /*
    public override int GetNumberOfFiefsDesired(DecisionMaker decisionMaker)
    {
      int baseNumber = CalculateBaseNumberOfFiefs(decisionMaker.Hero.Clan, Settings.Instance!.DesiredFiefsBaseline.SelectedValue.EnumValue);
      return baseNumber >= 0
          ? Math.Max(0, baseNumber + Settings.Instance.DesiredFiefsModifier + CalculateTraitsModifierForDesiredFiefs(decisionMaker, Settings.Instance.DesiredFiefsBaseline.SelectedValue.EnumValue))
          : baseNumber;
    }

    public override int GetNumberOfFiefsDeemedFair(Clan clan)
    {
      int baseNumber = CalculateBaseNumberOfFiefs(clan, Settings.Instance!.FiefsDeemedFairBaseline.SelectedValue.EnumValue);
      return baseNumber >= 0
          ? Math.Max(0, baseNumber + Settings.Instance.FiefsDeemedFairModifier)
          : baseNumber;
    }
    */

    //Protected overrides
    protected override float GetGeneralSupportScore(Clan clan, KingdomDecision decision, DecisionOutcome possibleOutcome)
    {
      return decision switch
      {
        MakePeaceKingdomDecision makePeaceDecision => GetGeneralSupportScore(clan, makePeaceDecision, possibleOutcome),
        DeclareWarDecision declareWarDecision => GetGeneralSupportScore(clan, declareWarDecision, possibleOutcome),
        SettlementClaimantDecision claimantDecision => GetGeneralSupportScore(clan, claimantDecision, possibleOutcome),
        SettlementClaimantPreliminaryDecision annexationDecision => GetGeneralSupportScore(clan, annexationDecision, possibleOutcome),
        _ => throw new ArgumentOutOfRangeException(nameof(decision), string.Format("Kingdom decision of type {0} is not supported.", decision.GetType().FullName)),
      };
    }

    protected override double GetSupportScoreOfDecisionMaker(DecisionMaker decisionMaker, KingdomDecision decision, DecisionOutcome possibleOutcome)
    {
      return decision switch
      {
        MakePeaceKingdomDecision makePeaceDecision => GetSupportScoreOfDecisionMaker(decisionMaker, makePeaceDecision, possibleOutcome) * decisionMaker.DecisionWeight,
        DeclareWarDecision declareWarDecision => GetSupportScoreOfDecisionMaker(decisionMaker, declareWarDecision, possibleOutcome) * decisionMaker.DecisionWeight,
        SettlementClaimantDecision claimantDecision => GetSupportScoreOfDecisionMaker(decisionMaker, claimantDecision, possibleOutcome) * decisionMaker.DecisionWeight,
        SettlementClaimantPreliminaryDecision annexationDecision => GetSupportScoreOfDecisionMaker(decisionMaker, annexationDecision, possibleOutcome) * decisionMaker.DecisionWeight,
        _ => throw new ArgumentOutOfRangeException(nameof(decision), string.Format("Kingdom decision of type {0} is not supported.", decision.GetType().FullName)),
      };
    }

    /*
    protected override int CalculateBaseNumberOfFiefs(Clan clan, NumberOfFiefsCalculationMethod calculationMethod)
    {
      switch (calculationMethod)
      {
        case NumberOfFiefsCalculationMethod.WithoutRestrictions: return -1;
        case NumberOfFiefsCalculationMethod.ByClanTier: return 1 + (clan.Kingdom != null && clan.Kingdom.RulingClan == clan ? 1 : 0) + clan.Tier / 2;
        case NumberOfFiefsCalculationMethod.ByClanMembers: return (int)Math.Ceiling(clan.Heroes.Where(h => h.IsAlive).Sum(h => clan.Tier < 5 ? (h.IsChild ? 0.5 : 1) : Math.Min(1.0, h.Age / Campaign.Current.Models.AgeModel.HeroComesOfAge))) + (clan.Kingdom != null && clan.Kingdom.RulingClan == clan ? 1 : 0);
        default:
          throw new ArgumentOutOfRangeException(nameof(calculationMethod), calculationMethod, "Is not supported NumberOfFiefsCalculationMethod value.");
      }
    }
    */

    //GetGeneralSupportScore internal - per decision type
    private float GetGeneralSupportScore(Clan clan, MakePeaceKingdomDecision makePeaceDecision, DecisionOutcome possibleOutcome)
    {
      int valueForClan = new PeaceBarterable(makePeaceDecision.Kingdom, makePeaceDecision.FactionToMakePeaceWith, CampaignTime.Years(1f)).GetValueForFaction(clan) - Campaign.Current.Models.DiplomacyModel.GetValueOfDailyTribute(makePeaceDecision.DailyTributeToBePaid);

      float situationalFactorValue = 0;
      if (Settings.Instance!.PeaceSupportCalculationMethod.SelectedValue.EnumValue.HasFlag(PeaceAndWarConsideration.SituationalFactor))
      {
        situationalFactorValue = ApplySituationalFactor(makePeaceDecision, ref valueForClan);
      }

      return FieldAccessHelper.ShouldPeaceBeDeclaredByRef(possibleOutcome)
          ? valueForClan * Campaign.Current.Models.DiplomacyModel.DenarsToInfluence() + situationalFactorValue
          : -valueForClan * Campaign.Current.Models.DiplomacyModel.DenarsToInfluence() - situationalFactorValue;
    }

    private float GetGeneralSupportScore(Clan clan, DeclareWarDecision declareWarDecision, DecisionOutcome possibleOutcome)
    {
      int valueForClan = new DeclareWarBarterable(declareWarDecision.Kingdom, declareWarDecision.FactionToDeclareWarOn).GetValueForFaction(clan);

      float situationalFactorValue = 0;
      if (Settings.Instance!.WarSupportCalculationMethod.SelectedValue.EnumValue.HasFlag(PeaceAndWarConsideration.SituationalFactor))
      {
        situationalFactorValue = ApplySituationalFactor(declareWarDecision, ref valueForClan);
      }

      return FieldAccessHelper.ShouldWarBeDeclaredByRef(possibleOutcome)
          ? valueForClan * Campaign.Current.Models.DiplomacyModel.DenarsToInfluence() + situationalFactorValue
          : -valueForClan * Campaign.Current.Models.DiplomacyModel.DenarsToInfluence() - situationalFactorValue;
    }

    private float GetGeneralSupportScore(Clan clan, SettlementClaimantDecision claimantDecision, DecisionOutcome possibleOutcome)
    {
      return 0f;
    }

    private float GetGeneralSupportScore(Clan clan, SettlementClaimantPreliminaryDecision annexationDecision, DecisionOutcome possibleOutcome)
    {
      throw new NotImplementedException();
    }

    //GetSupportScoreOfDecisionMaker internal - per decision type
    private double GetSupportScoreOfDecisionMaker(DecisionMaker decisionMaker, MakePeaceKingdomDecision makePeaceDecision, DecisionOutcome possibleOutcome)
    {
      double traitScore = decisionMaker.Hero.GetTraitLevel(DefaultTraits.Mercy) * 10;

      double relationshipFactorValue = Settings.Instance!.PeaceSupportCalculationMethod.SelectedValue.EnumValue.HasFlag(PeaceAndWarConsideration.RelationshipFactor)
          ? CalculateRelationshipFactor(decisionMaker, makePeaceDecision.FactionToMakePeaceWith) * Settings.Instance.MakePeaceRelationshipFactorStrength
          : 0;

      double tributeFactorValue = Settings.Instance!.PeaceSupportCalculationMethod.SelectedValue.EnumValue.HasFlag(PeaceAndWarConsideration.TributeFactor)
          ? CalculateTributeFactor(decisionMaker, makePeaceDecision.FactionToMakePeaceWith, makePeaceDecision.DailyTributeToBePaid) * Settings.Instance.MakePeaceTributeFactorStrength
          : 0;

      return FieldAccessHelper.ShouldPeaceBeDeclaredByRef(possibleOutcome)
          ? traitScore + relationshipFactorValue - tributeFactorValue
          : -traitScore - relationshipFactorValue + tributeFactorValue;
    }

    private double GetSupportScoreOfDecisionMaker(DecisionMaker decisionMaker, DeclareWarDecision declareWarDecision, DecisionOutcome possibleOutcome)
    {
      double traitScore = decisionMaker.Hero.GetTraitLevel(DefaultTraits.Valor) * 20 - decisionMaker.Hero.GetTraitLevel(DefaultTraits.Mercy) * 10;

      double relationshipFactorValue = Settings.Instance!.WarSupportCalculationMethod.SelectedValue.EnumValue.HasFlag(PeaceAndWarConsideration.RelationshipFactor)
          ? CalculateRelationshipFactor(decisionMaker, declareWarDecision.FactionToDeclareWarOn) * Settings.Instance.DeclareWarRelationshipFactorStrength
          : 0;

      double tributeFactorValue = Settings.Instance!.WarSupportCalculationMethod.SelectedValue.EnumValue.HasFlag(PeaceAndWarConsideration.TributeFactor)
          ? CalculateTributeFactor(decisionMaker, declareWarDecision.FactionToDeclareWarOn) * Settings.Instance.DeclareWarTributeFactorStrength
          : 0;

      return FieldAccessHelper.ShouldWarBeDeclaredByRef(possibleOutcome)
          ? traitScore - relationshipFactorValue + tributeFactorValue
          : -traitScore + relationshipFactorValue - tributeFactorValue;
    }

    private double GetSupportScoreOfDecisionMaker(DecisionMaker decisionMaker, SettlementClaimantDecision claimantDecision, DecisionOutcome possibleOutcome)
    {
      double nativeScore = CalculateNativeForSettlementClaimantDecision(decisionMaker, claimantDecision, possibleOutcome);
      /*
      double possessionsFactorValue = Settings.Instance.WarSupportCalculationMethod.SelectedValue.EnumValue.HasFlag(FiefOwnershipConsideration.PossessionsFactor)
          ? CalculateRelationshipFactor(decisionMaker, declareWarDecision.FactionToDeclareWarOn) * Settings.Instance.FiefOwnershipPossessionsFactorStrength
          : 0;
      */
      return nativeScore;
    }

    private double GetSupportScoreOfDecisionMaker(DecisionMaker decisionMaker, SettlementClaimantPreliminaryDecision annexationDecision, DecisionOutcome possibleOutcome)
    {
      throw new NotImplementedException();
    }

    //ApplySituationalFactor - per decision type
    private float ApplySituationalFactor(MakePeaceKingdomDecision makePeaceDecision, ref int valueForClan)
    {
      (float value, int multiplier) = CalculateSituationalFactor(makePeaceDecision.Kingdom, makePeaceDecision.FactionToMakePeaceWith);
      valueForClan /= multiplier;
      return value * Settings.Instance!.MakePeaceSituationalFactorStrength;
    }

    private float ApplySituationalFactor(DeclareWarDecision declareWarDecision, ref int valueForClan)
    {
      (float value, int multiplier) = CalculateSituationalFactor(declareWarDecision.Kingdom, declareWarDecision.FactionToDeclareWarOn);
      valueForClan /= multiplier;
      return value * Settings.Instance!.DeclareWarSituationalFactorStrength;
    }

    //CalculateSituationalFactor
    private (float value, int multiplier) CalculateSituationalFactor(Kingdom decidingKingdom, IFaction factionToChangeStateWith)
    {
      bool atWar = decidingKingdom.IsAtWarWith(factionToChangeStateWith);
      IEnumerable<Kingdom> warringKingdoms = decidingKingdom.GetAdversaries();

      float currentEffectiveStrength = decidingKingdom.TotalStrength / Math.Max(1, warringKingdoms.Count());
      float currentPowerScore = currentEffectiveStrength - warringKingdoms.Sum(k => k.GetEffectiveStrength()) - Clan.All.Where(c => c.Kingdom is null && c.IsAtWarWith(decidingKingdom)).Sum(c => c.TotalStrength / 10);

      float newEffectiveStrength = factionToChangeStateWith.IsKingdomFaction ? decidingKingdom.TotalStrength / Math.Max(1, warringKingdoms.Count() + (atWar ? -1 : 1)) : currentEffectiveStrength;
      float newPowerScore = factionToChangeStateWith.IsKingdomFaction
          ? newEffectiveStrength - decidingKingdom.GetAdversaries((Kingdom)factionToChangeStateWith).Sum(k => k.TotalStrength / Math.Max(1, k == factionToChangeStateWith ? k.GetNumberOfWars(decidingKingdom) : k.GetNumberOfWars()))
            - Clan.All.Where(c => c.Kingdom is null && c.IsAtWarWith(decidingKingdom)).Sum(c => c.TotalStrength / 10)
          : currentPowerScore + (atWar ? factionToChangeStateWith.TotalStrength / 10 : -factionToChangeStateWith.TotalStrength / 10);

      int multiplier = newPowerScore < 0 ? 2 + (int)Math.Round(-newPowerScore / newEffectiveStrength, MidpointRounding.AwayFromZero) : 1;
      float value = multiplier * (newPowerScore - currentPowerScore) + (atWar ? CalculateWarSuccessScore(decidingKingdom, factionToChangeStateWith) : newPowerScore);
      return (value * 0.001f, multiplier);
    }

    //CalculateRelationshipFactor
    private double CalculateRelationshipFactor(DecisionMaker decisionMaker, IFaction factionToChangeStateWith)
    {
      bool atWar = decisionMaker.Hero.MapFaction.IsAtWarWith(factionToChangeStateWith);
      Dictionary<Hero, int> otherFactionHeroRelations = factionToChangeStateWith.Heroes.Where(h => decisionMaker.Hero.IsFriend(h) || decisionMaker.Hero.IsEnemy(h)).ToDictionary(keySelector: h => h, elementSelector: h => decisionMaker.Hero.GetModifiedRelation(h) / 10);
      return otherFactionHeroRelations.Count() > 0 ? otherFactionHeroRelations.Sum(f => f.Value) / (double)otherFactionHeroRelations.Count() * 10 : 0;
    }

    //CalculateTributeFactor
    private double CalculateTributeFactor(DecisionMaker decisionMaker, IFaction factionToChangeStateWith, int dailyTributeToBePaid = 0)
    {
      bool atWar = decisionMaker.Hero.MapFaction.IsAtWarWith(factionToChangeStateWith);
      StanceLink stanceWith = decisionMaker.Hero.MapFaction.GetStanceWith(factionToChangeStateWith);
      dailyTributeToBePaid = atWar ? dailyTributeToBePaid : stanceWith.GetDailyTributePaid(decisionMaker.Hero.MapFaction);

      double GetReputationScoreModifierAtWar() =>
        dailyTributeToBePaid * CalculateWarSuccessScore(decisionMaker.Hero.Clan.Kingdom, factionToChangeStateWith) >= 0 //Tribute is being demanded from the winning faction
            ? CalculateWarSuccessScore(decisionMaker.Hero.Clan.Kingdom, factionToChangeStateWith) >= 0 //Deciding kingdom is winning
              ? GetTraitLevelModifier(decisionMaker.Hero.GetTraitLevel(DefaultTraits.Valor) * 2 + decisionMaker.Hero.GetTraitLevel(DefaultTraits.Calculating) - decisionMaker.Hero.GetTraitLevel(DefaultTraits.Generosity), 2)
              : GetTraitLevelModifier(-decisionMaker.Hero.GetTraitLevel(DefaultTraits.Honor) - decisionMaker.Hero.GetTraitLevel(DefaultTraits.Generosity))
            : 1.0;
      double GetReputationScoreModifierAtPeace() => GetTraitLevelModifier(-decisionMaker.Hero.GetTraitLevel(DefaultTraits.Honor) - decisionMaker.Hero.GetTraitLevel(DefaultTraits.Generosity));

      double weightedDailyTribute = dailyTributeToBePaid * (atWar ? GetReputationScoreModifierAtWar() : GetReputationScoreModifierAtPeace()) * 0.25 //This is reputational consideration part (uses faction tribute as whole)
                                    + (dailyTributeToBePaid * deCalculateShareFactor!(decisionMaker.Hero.Clan) //This is clan financial consideration part (assesses clan income)
                                       * GetTraitLevelModifier(decisionMaker.Hero.GetTraitLevel(DefaultTraits.Calculating) - decisionMaker.Hero.GetTraitLevel(DefaultTraits.Generosity)));

      return Campaign.Current.Models.DiplomacyModel.GetValueOfDailyTribute((int)weightedDailyTribute) * Campaign.Current.Models.DiplomacyModel.DenarsToInfluence();
    }

    private static double GetTraitLevelModifier(int traitLevel, double weight = 1)
    {
      if (weight < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(weight), weight, "Argument is negative.");
      }

      if (traitLevel == 1 || traitLevel == -1)
      {
        return 1 + 0.5 * traitLevel * weight;
      }
      if (traitLevel > 1)
      {
        return GetTraitLevelModifier(traitLevel - 1, weight) + (1 / Math.Pow(2, traitLevel) * weight);
      }
      if (traitLevel < -1)
      {
        return weight >= 1 ? 1 / (Math.Pow(2, Math.Abs(traitLevel)) * weight) : weight == 0 ? 1 : GetTraitLevelModifier(traitLevel + 1, weight) - 1 / Math.Pow(2, Math.Abs(traitLevel)) * weight;
      }
      return 1; //if (traitLevel == 0)
    }

    /*
    //CalculateTraitsModifierForDesiredFiefs
    private int CalculateTraitsModifierForDesiredFiefs(DecisionMaker decisionMaker, NumberOfFiefsCalculationMethod calculationMethod)
    {
      int fairFiefNumber = GetNumberOfFiefsDeemedFair(decisionMaker.Hero.Clan);
      int baseFiefNumber = CalculateBaseNumberOfFiefs(decisionMaker.Hero.Clan, calculationMethod);

      if (fairFiefNumber <= baseFiefNumber + Settings.Instance!.DesiredFiefsModifier
          && decisionMaker.Hero.GetTraitLevel(DefaultTraits.Honor) > 0
          && decisionMaker.Hero.GetTraitLevel(DefaultTraits.Honor) > GetTraitLevelModifier(decisionMaker.Hero.GetTraitLevel(DefaultTraits.Calculating) - decisionMaker.Hero.GetTraitLevel(DefaultTraits.Generosity)))
      {
        return fairFiefNumber - (baseFiefNumber + Settings.Instance.DesiredFiefsModifier);
      }

      double modifierValue = (baseFiefNumber * GetTraitLevelModifier(decisionMaker.Hero.GetTraitLevel(DefaultTraits.Calculating) - decisionMaker.Hero.GetTraitLevel(DefaultTraits.Generosity), 0.5)) - baseFiefNumber;
      return (int)Math.Truncate(modifierValue);
    }
    */

    //CalculateNativeForSettlementClaimantDecision
    private double CalculateNativeForSettlementClaimantDecision(DecisionMaker decisionMaker, SettlementClaimantDecision claimantDecision, DecisionOutcome possibleOutcome)
    {
      float initialMeritScore = possibleOutcome.InitialMerit * MathF.Clamp(1f + decisionMaker.Hero.GetTraitLevel(DefaultTraits.Honor), 0f, 2f);
      float basicScoreForOutcome;
      int calculatingTraitLevel = MBMath.ClampInt(decisionMaker.Hero.GetTraitLevel(DefaultTraits.Calculating), -2, 2);
      bool outcomeIsClanOfDM = FieldAccessHelper.ClanAsDecisionOutcomeByRef(possibleOutcome) == decisionMaker.Hero.Clan;
      if (outcomeIsClanOfDM)
      {
        float settlementValueForFaction = claimantDecision.Settlement.GetSettlementValueForFaction(decisionMaker.Hero.Clan);
        basicScoreForOutcome = initialMeritScore + 0.2f * settlementValueForFaction * Campaign.Current.Models.DiplomacyModel.DenarsToInfluence();
      }
      else
      {
        float relationBetweenClans = !outcomeIsClanOfDM ? FactionManager.GetRelationBetweenClans(FieldAccessHelper.ClanAsDecisionOutcomeByRef(possibleOutcome), decisionMaker.Hero.Clan) : 100f;
        basicScoreForOutcome = initialMeritScore * MathF.Clamp(1f + calculatingTraitLevel, 0f, 2f) + relationBetweenClans * 0.2f * calculatingTraitLevel;
      }
      double calculatingModifier = (1.0 - (calculatingTraitLevel > 0 ? 0.4 - Math.Min(2f, calculatingTraitLevel) * 0.1 : 0.4 + Math.Min(2f, Math.Abs(calculatingTraitLevel)) * 0.1) * 1.5);
      return basicScoreForOutcome * calculatingModifier * (outcomeIsClanOfDM ? 2f : 1f);
    }

    /*
    //CalculateTributeFactor
    private double CalculatePossessionsFactor(DecisionMaker decisionMaker, Clan clanToAssess, Settlement settlementToAssess, bool isBeingAnnexed)
    {
      //
      if decisionMaker.Hero.Clan
      clanToAssess.Settlements.Count(s => !s.Town.IsOwnerUnassigned)
      //

      return 0;
    }
    */
  }
}
