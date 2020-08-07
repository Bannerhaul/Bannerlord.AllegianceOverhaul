using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.SaveSystem;
using AllegianceOverhaul.Helpers;
using AllegianceOverhaul.SavableClasses;

namespace AllegianceOverhaul.CampaignBehaviors.BehaviorManagers
{
  [SaveableClass(100)]
  public class AOCooldownManager
  {
    [SaveableField(1)]
    private Dictionary<KingdomDecision, KingdomDecisionConclusion> _KingdomDecisionHistory;

    internal static Dictionary<KingdomDecision, KingdomDecisionConclusion> KingdomDecisionHistory { get; private set; }
    internal static ReadOnlyCollection<Type> SupportedDecisionTypes => new ReadOnlyCollection<Type>
      (new List<Type>() { typeof(MakePeaceKingdomDecision), typeof(DeclareWarDecision), typeof(ExpelClanFromKingdomDecision), typeof(KingdomPolicyDecision), typeof(SettlementClaimantPreliminaryDecision) });

    internal AOCooldownManager()
    {
      _KingdomDecisionHistory = new Dictionary<KingdomDecision, KingdomDecisionConclusion>(new DecisionEqualityComparer());
      KingdomDecisionHistory = _KingdomDecisionHistory;
    }

    public void UpdateKingdomDecisionHistory(KingdomDecision decision, DecisionOutcome chosenOutcome, CampaignTime conclusionTime)
    {
      if (SupportedDecisionTypes.Contains(decision.GetType()))
      {
        if (_KingdomDecisionHistory.ContainsKey(decision))
          _KingdomDecisionHistory.Remove(decision);
        _KingdomDecisionHistory[decision] = new KingdomDecisionConclusion(chosenOutcome, conclusionTime);
      }
      else
        throw new ArgumentException(string.Format("{0} is not supported KingdomDecision type", decision.GetType().FullName), nameof(decision));
    }

    public static int GetRequiredDecisionCooldown(KingdomDecision decision)
    {
      switch (decision)
      {
        case MakePeaceKingdomDecision _:
          return Settings.Instance.MakePeaceDecisionCooldown;
        case DeclareWarDecision _:
          return Settings.Instance.DeclareWarDecisionCooldown;
        case ExpelClanFromKingdomDecision _:
          return Settings.Instance.ExpelClanDecisionCooldown;
        case KingdomPolicyDecision _:
          return Settings.Instance.KingdomPolicyDecisionCooldown;
        case SettlementClaimantPreliminaryDecision _:
          return Settings.Instance.AnnexationDecisionCooldown;
        default:
          throw new ArgumentException(string.Format("{0} is not supported KingdomDecision type", decision.GetType().FullName), nameof(decision));
      }
    }
    public static int GetRequiredDecisionCooldown(Type decisionType)
    {
      if (decisionType == typeof(MakePeaceKingdomDecision))
      {
        return Settings.Instance.MakePeaceDecisionCooldown;
      }
      else if (decisionType == typeof(DeclareWarDecision))
      {
        return Settings.Instance.DeclareWarDecisionCooldown;
      }
      else if (decisionType == typeof(ExpelClanFromKingdomDecision))
      {
        return Settings.Instance.ExpelClanDecisionCooldown;
      }
      else if (decisionType == typeof(KingdomPolicyDecision))
      {
        return Settings.Instance.KingdomPolicyDecisionCooldown;
      }
      else if (decisionType == typeof(SettlementClaimantPreliminaryDecision))
      {
        return Settings.Instance.AnnexationDecisionCooldown;
      }
      else
      {
        throw new ArgumentException(string.Format("{0} is not supported KingdomDecision type", decisionType.FullName), nameof(decisionType));
      }
    }

    public static bool HasDecisionCooldown(KingdomDecision decision)
    {
      return HasPrimaryDecisionCooldown(decision) || HasAlternativeDecisionCooldown(decision);
    }

    public static bool HasPrimaryDecisionCooldown(KingdomDecision decision)
    {
      return SupportedDecisionTypes.Contains(decision.GetType())
             && KingdomDecisionHistory.TryGetValue(decision, out KingdomDecisionConclusion decisionConclusion)
             && decisionConclusion.ConclusionTime.ElapsedDaysUntilNow < GetRequiredDecisionCooldown(decision);
    }
    public static bool HasAlternativeDecisionCooldown(KingdomDecision decision)
    {
      switch (decision)
      {
        case SettlementClaimantPreliminaryDecision preliminaryDecision:
          CampaignTime? campaignTime = GetTimeOfLastAnnexDecisionAgainstClan(preliminaryDecision.Settlement.OwnerClan);
          return campaignTime.HasValue && campaignTime.Value.ElapsedDaysUntilNow < GetRequiredDecisionCooldown(decision);
        default:
          return false;
      }
    }

    public static bool HasDecisionCooldown(KingdomDecision decision, out float elapsedDaysUntilNow)
    {
      return HasPrimaryDecisionCooldown(decision, out elapsedDaysUntilNow) || HasAlternativeDecisionCooldown(decision, out elapsedDaysUntilNow);
    }

    public static bool HasPrimaryDecisionCooldown(KingdomDecision decision, out float elapsedDaysUntilNow)
    {
      elapsedDaysUntilNow = default;
      return SupportedDecisionTypes.Contains(decision.GetType())
             && KingdomDecisionHistory.TryGetValue(decision, out KingdomDecisionConclusion decisionConclusion)
             && (elapsedDaysUntilNow = decisionConclusion.ConclusionTime.ElapsedDaysUntilNow) < GetRequiredDecisionCooldown(decision);
    }
    public static bool HasAlternativeDecisionCooldown(KingdomDecision decision, out float elapsedDaysUntilNow)
    {
      elapsedDaysUntilNow = default;
      switch (decision)
      {
        case SettlementClaimantPreliminaryDecision preliminaryDecision:
          CampaignTime? campaignTime = GetTimeOfLastAnnexDecisionAgainstClan(preliminaryDecision.Settlement.OwnerClan);
          return campaignTime.HasValue && (elapsedDaysUntilNow = campaignTime.Value.ElapsedDaysUntilNow) < GetRequiredDecisionCooldown(decision) / 2;
        default:
          return false;
      }
    }

    internal void Sync()
    {
      _KingdomDecisionHistory = _KingdomDecisionHistory == null
          ? new Dictionary<KingdomDecision, KingdomDecisionConclusion>(new DecisionEqualityComparer())
          : new Dictionary<KingdomDecision, KingdomDecisionConclusion>(_KingdomDecisionHistory.Where(d => d.Value.ConclusionTime.ElapsedYearsUntilNow < 2).ToDictionary(nd => nd.Key, nd => nd.Value), new DecisionEqualityComparer());
      KingdomDecisionHistory = _KingdomDecisionHistory;
    }

    private static CampaignTime? GetTimeOfLastAnnexDecisionAgainstClan(Clan clan)
    {
      return KingdomDecisionHistory.Where(d => d.Key is SettlementClaimantPreliminaryDecision otherPreliminaryDecision
                                               && otherPreliminaryDecision.Kingdom == clan.Kingdom
                                               && FieldAccessHelper.annexDecisionInitialOwnerByRef(otherPreliminaryDecision) == clan)
                                   .OrderByDescending(d => d.Value.ConclusionTime).FirstOrDefault().Value?.ConclusionTime;
    }

    internal class DecisionEqualityComparer : EqualityComparer<KingdomDecision>
    {
      private bool InternalEquals(KingdomDecision decision1, KingdomDecision decision2)
      {
        if (decision1.GetType() != decision2.GetType())
          return false;
        switch (decision1)
        {
          case MakePeaceKingdomDecision peaceDecision1:
            return peaceDecision1.FactionToMakePeaceWith == ((MakePeaceKingdomDecision)decision2).FactionToMakePeaceWith;
          case DeclareWarDecision warDecision1:
            return warDecision1.FactionToDeclareWarOn == ((DeclareWarDecision)decision2).FactionToDeclareWarOn;
          case ExpelClanFromKingdomDecision expelDecision1:
            return expelDecision1.ClanToExpel == ((ExpelClanFromKingdomDecision)decision2).ClanToExpel;
          case KingdomPolicyDecision policyDecision1:
            return policyDecision1.Policy == ((KingdomPolicyDecision)decision2).Policy;
          case SettlementClaimantPreliminaryDecision annexationDecision1:
            return annexationDecision1.Settlement == ((SettlementClaimantPreliminaryDecision)decision2).Settlement;
          default:
            throw new ArgumentException(string.Format("{0} is not supported KingdomDecision type", decision1.GetType().FullName), nameof(decision1));
        }
      }
      public override bool Equals(KingdomDecision decision1, KingdomDecision decision2)
      {
        return (decision1 == null && decision2 == null) || (decision1 != null && decision2 != null && decision1.Kingdom == decision2.Kingdom && InternalEquals(decision1, decision2));
      }
      public override int GetHashCode(KingdomDecision decision)
      {
        return decision.Kingdom.GetHashCode() ^ decision.GetType().GetHashCode();
      }
    }
  }
}
