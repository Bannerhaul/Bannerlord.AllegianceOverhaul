using AllegianceOverhaul.Helpers;
using AllegianceOverhaul.SavableClasses;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.SaveSystem;

namespace AllegianceOverhaul.CampaignBehaviors.BehaviorManagers
{
    //[SaveableClass(100)]
    public class AOCooldownManager
    {
        [SaveableField(1)]
        private Dictionary<KingdomDecision, KingdomDecisionConclusion> _KingdomDecisionHistory;
        [SaveableField(2)]
        private CampaignTime _lastJoinPlayerRequest;

        internal static Dictionary<KingdomDecision, KingdomDecisionConclusion>? KingdomDecisionHistory { get; private set; }
        internal static ReadOnlyCollection<Type> SupportedDecisionTypes => new(new List<Type>() { typeof(MakePeaceKingdomDecision), typeof(DeclareWarDecision), typeof(ExpelClanFromKingdomDecision), typeof(KingdomPolicyDecision), typeof(SettlementClaimantPreliminaryDecision) });

        internal static CampaignTime LastJoinPlayerRequest { get; private set; }

        internal AOCooldownManager()
        {
            _KingdomDecisionHistory = new Dictionary<KingdomDecision, KingdomDecisionConclusion>(new DecisionEqualityComparer());
            KingdomDecisionHistory = _KingdomDecisionHistory;

            _lastJoinPlayerRequest = CampaignTime.Zero;
            LastJoinPlayerRequest = _lastJoinPlayerRequest;
        }

        public void UpdateKingdomDecisionHistory(KingdomDecision decision, DecisionOutcome chosenOutcome, CampaignTime conclusionTime)
        {
            if (SupportedDecisionTypes.Contains(decision.GetType()))
            {
                _KingdomDecisionHistory.Remove(decision);
                _KingdomDecisionHistory[decision] = new KingdomDecisionConclusion(chosenOutcome, conclusionTime);
            }
            else
                throw new ArgumentException(string.Format("{0} is not supported KingdomDecision type", decision.GetType().FullName), nameof(decision));
        }

        public void UpdateJoinPlayerRequestHistory(CampaignTime lastRequestTime)
        {
            _lastJoinPlayerRequest = lastRequestTime;
            LastJoinPlayerRequest = _lastJoinPlayerRequest;
        }

        public static int GetRequiredDecisionCooldown(KingdomDecision decision)
        {
            return decision switch
            {
                MakePeaceKingdomDecision _ => Settings.Instance!.MakePeaceDecisionCooldown,
                DeclareWarDecision _ => Settings.Instance!.DeclareWarDecisionCooldown,
                ExpelClanFromKingdomDecision _ => Settings.Instance!.ExpelClanDecisionCooldown,
                KingdomPolicyDecision _ => Settings.Instance!.KingdomPolicyDecisionCooldown,
                SettlementClaimantPreliminaryDecision _ => Settings.Instance!.AnnexationDecisionCooldown,
                _ => throw new ArgumentException(string.Format("{0} is not supported KingdomDecision type", decision.GetType().FullName), nameof(decision)),
            };
        }
        public static int GetRequiredDecisionCooldown(Type decisionType)
        {
            if (decisionType == typeof(MakePeaceKingdomDecision))
            {
                return Settings.Instance!.MakePeaceDecisionCooldown;
            }
            else if (decisionType == typeof(DeclareWarDecision))
            {
                return Settings.Instance!.DeclareWarDecisionCooldown;
            }
            else if (decisionType == typeof(ExpelClanFromKingdomDecision))
            {
                return Settings.Instance!.ExpelClanDecisionCooldown;
            }
            else if (decisionType == typeof(KingdomPolicyDecision))
            {
                return Settings.Instance!.KingdomPolicyDecisionCooldown;
            }
            else if (decisionType == typeof(SettlementClaimantPreliminaryDecision))
            {
                return Settings.Instance!.AnnexationDecisionCooldown;
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
                   && KingdomDecisionHistory!.TryGetValue(decision, out KingdomDecisionConclusion? decisionConclusion)
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

        public static bool HasPlayerRequestCooldown()
        {
            int requestCooldown = Settings.Instance!.PlayerRequestCooldown;
            return requestCooldown > 0 && LastJoinPlayerRequest > CampaignTime.Zero && LastJoinPlayerRequest.ElapsedDaysUntilNow < requestCooldown;
        }

        public static bool HasDecisionCooldown(KingdomDecision decision, out float elapsedDaysUntilNow)
        {
            return HasPrimaryDecisionCooldown(decision, out elapsedDaysUntilNow) || HasAlternativeDecisionCooldown(decision, out elapsedDaysUntilNow);
        }

        public static bool HasPrimaryDecisionCooldown(KingdomDecision decision, out float elapsedDaysUntilNow)
        {
            elapsedDaysUntilNow = default;
            return SupportedDecisionTypes.Contains(decision.GetType())
                   && KingdomDecisionHistory!.TryGetValue(decision, out KingdomDecisionConclusion? decisionConclusion)
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

            LastJoinPlayerRequest = _lastJoinPlayerRequest;
        }

        private static CampaignTime? GetTimeOfLastAnnexDecisionAgainstClan(Clan clan)
        {
            return KingdomDecisionHistory!.Where(d => d.Key is SettlementClaimantPreliminaryDecision otherPreliminaryDecision
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
                return decision1 switch
                {
                    MakePeaceKingdomDecision peaceDecision1 => peaceDecision1.FactionToMakePeaceWith == ((MakePeaceKingdomDecision) decision2).FactionToMakePeaceWith,
                    DeclareWarDecision warDecision1 => warDecision1.FactionToDeclareWarOn == ((DeclareWarDecision) decision2).FactionToDeclareWarOn,
                    ExpelClanFromKingdomDecision expelDecision1 => expelDecision1.ClanToExpel == ((ExpelClanFromKingdomDecision) decision2).ClanToExpel,
                    KingdomPolicyDecision policyDecision1 => policyDecision1.Policy == ((KingdomPolicyDecision) decision2).Policy,
                    SettlementClaimantPreliminaryDecision annexationDecision1 => annexationDecision1.Settlement == ((SettlementClaimantPreliminaryDecision) decision2).Settlement,
                    _ => throw new ArgumentException(string.Format("{0} is not supported KingdomDecision type", decision1.GetType().FullName), nameof(decision1)),
                };
            }
            public override bool Equals(KingdomDecision? decision1, KingdomDecision? decision2)
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