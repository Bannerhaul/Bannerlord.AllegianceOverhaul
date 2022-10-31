using AllegianceOverhaul.Extensions;
using AllegianceOverhaul.Helpers;

using System;
using System.Collections.Generic;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.BarterSystem;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.Library;
using TaleWorlds.Localization;

using static AllegianceOverhaul.Helpers.LocalizationHelper;

namespace AllegianceOverhaul.LoyaltyRebalance.EnsuredLoyalty
{
    public class LoyaltyCostManager
    {
        public sealed class ComplexCost
        {
            public int InfluenceCost { get; }
            public int GoldCost { get; }
            public ComplexCost(int influenceCost, int goldCost)
            {
                InfluenceCost = influenceCost;
                GoldCost = goldCost;
            }
            public override string ToString()
            {
                Dictionary<string, object> attributes = new()
                {
                    ["INFLUENCE_COST"] = new TextObject(InfluenceCost),
                    ["GOLD_COST"] = new TextObject(GoldCost)
                };
                return new TextObject(ComplexCostString, attributes).ToString();
            }
        }

        private const string ComplexCostString = "{=3MafquFdR}[Influence cost = {INFLUENCE_COST}; Gold cost = {GOLD_COST}]";

        private const string WithholdPricePayed = "{=M040sU1dF}{LEAVING_CLAN_KINGDOM_LEADER.NAME} decided to withhold {LEAVING_CLAN.NAME} in {LEAVING_CLAN_KINGDOM.NAME}. Initially {?LEAVING_CLAN_KINGDOM_LEADER.GENDER}she{?}he{\\?} had {INITIAL_INFLUENCE} influence and {INITILAL_GOLD} denars. Withholding required {INFLUENCE_COST} influence and {GOLD_COST} denars. After intervention {?LEAVING_CLAN_KINGDOM_LEADER.GENDER}she{?}he{\\?} has {RESULT_INFLUENCE} influence and {RESULT_GOLD} denars.";
        private const string WithholdPriceRejected = "{=ldZNb5zjx}{LEAVING_CLAN_KINGDOM_LEADER.NAME} of {LEAVING_CLAN_KINGDOM.NAME} decided to let {LEAVING_CLAN.NAME} go. At that moment {?LEAVING_CLAN_KINGDOM_LEADER.GENDER}she{?}he{\\?} had {INITIAL_INFLUENCE} influence and {INITILAL_GOLD} denars. Withholding the clan would require {INFLUENCE_COST} influence and {GOLD_COST} denars.";

        private const string ActionLeaving = "{=8xWUKmRwi}leaving your kingdom";
        private const string ActionDefecting = "{=BD9jEJQgV}going to leave your kingdom and join the {TARGET_KINGDOM.NAME}";
        private const string PlayerInquiryHeader = "{=bnrPhGr8M}Clan {LEAVING_CLAN.NAME} is leaving!";
        private const string PlayerInquiryBody = "{=4MgHKVMDz}Clan {LEAVING_CLAN.NAME} is {ACTION_DESCRIPTION}! Will you intervene and insist they should stay? That would require {INFLUENCE_COST} influence and {GOLD_COST} denars.";

        private const string ButtonWithholdText = "{=iyyixqbbN}Withhold them";
        private const string ButtonReleaseText = "{=E0WvZ3AzI}Let them go";

        public Clan LeavingClan { get; }
        public Kingdom? TargetKingdom { get; }
        public ComplexCost? WithholdCost { get; }
        public double BarterableSum { get; private set; }
        public double BaseCalculatedCost { get; private set; }
        protected Barterable? ClanBarterable { get; private set; }

        public LoyaltyCostManager(Clan clan, Kingdom? targetKingdom = null)
        {
            LeavingClan = clan;
            TargetKingdom = targetKingdom;
            BarterableSum = 0;
            BaseCalculatedCost = 0;
            WithholdCost = GetWithholdCost();
        }
        private ComplexCost? GetWithholdCost()
        {
            if (!SettingsHelper.SubSystemEnabled(SubSystemType.LoyaltyWithholding, LeavingClan))
                return null;

            if (TargetKingdom != null)
            {
                JoinKingdomAsClanBarterable asClanBarterable = new(LeavingClan.Leader, TargetKingdom);
                int ClanBarterableValueForClan = asClanBarterable.GetValueForFaction(LeavingClan);
                int ClanBarterableValueForKingdom = asClanBarterable.GetValueForFaction(TargetKingdom);
                ClanBarterable = asClanBarterable;
                BarterableSum = ClanBarterableValueForClan + ClanBarterableValueForKingdom;
            }
            else
            {
                LeaveKingdomAsClanBarterable asClanBarterable = new(LeavingClan.Leader, null);
                int ClanBarterableValueForFaction = asClanBarterable.GetValueForFaction(LeavingClan);
                ClanBarterable = asClanBarterable;
                BarterableSum = ClanBarterableValueForFaction - (LeavingClan.IsMinorFaction ? 500 : 0);
            }

            if (BarterableSum <= Settings.Instance!.WithholdToleranceLimit * 1000000)
            {
                return null;
            }
            BaseCalculatedCost = Math.Sqrt(BarterableSum) / Math.Log10(BarterableSum);

            return
              new ComplexCost
              (
                (int) (BaseCalculatedCost * Settings.Instance.WithholdInfluenceMultiplier),
                Settings.Instance.UseWithholdBribing && Settings.Instance.WithholdToleranceLimitForBribes * 1000000 < BarterableSum ? (int) BaseCalculatedCost * Settings.Instance.WithholdGoldMultiplier : 0
              );
        }
        private int GetScoreForKingdomToWithholdClan()
        {
            double RelativeScoreToLeave = new LeaveKingdomAsClanBarterable(LeavingClan.Leader, null).GetValueForFaction(LeavingClan) - (LeavingClan.IsMinorFaction ? 500 : 0);
            RelativeScoreToLeave = Math.Sqrt(Math.Abs(RelativeScoreToLeave)) * (RelativeScoreToLeave >= 0 ? -500 : 250); //invert it as negative is good for us
            double CostScore = WithholdCost!.InfluenceCost / Math.Max(0.001, LeavingClan.Kingdom.RulingClan.Influence) * WithholdCost.InfluenceCost * 1000 + WithholdCost.GoldCost / Math.Max(0.001, LeavingClan.Kingdom.Leader.Gold) * WithholdCost.GoldCost;
            double ClanCountModifier = 0;
            foreach (Clan clan in LeavingClan.Kingdom.Clans)
            {
                if (clan != LeavingClan.Kingdom.RulingClan)
                {
                    int relationBetweenClans = FactionManager.GetRelationBetweenClans(LeavingClan.Kingdom.RulingClan, clan);
                    ClanCountModifier +=
                      1 * (clan.IsUnderMercenaryService ? 0.5 : 1) * (1.0 + (LeavingClan.Kingdom.Culture == clan.Culture ? 0.150000005960464 : -0.150000005960464))
                      * Math.Min(1.3, Math.Max(0.5, 1.0 + Math.Sqrt(Math.Abs(relationBetweenClans)) * (relationBetweenClans < 0 ? -0.03 : 0.02)));
                }
            }
            double RelativeStrengthModifier = LeavingClan.TotalStrength / (LeavingClan.Kingdom.TotalStrength - LeavingClan.Kingdom.RulingClan.TotalStrength);
            float SettlementValue = (TargetKingdom != null && TargetKingdom.IsAtWarWith(LeavingClan.Kingdom) && !LeavingClan.IsUnderMercenaryService) ? LeavingClan.CalculateTotalSettlementValueForFaction(LeavingClan.Kingdom) : 0;
            double Result = (Campaign.Current.Models.DiplomacyModel.GetScoreOfKingdomToGetClan(LeavingClan.Kingdom, LeavingClan) + RelativeScoreToLeave) * (RelativeStrengthModifier + (LeavingClan.IsUnderMercenaryService ? 0.1 : 1)) + SettlementValue - CostScore * ClanCountModifier;
            if (SettingsHelper.SystemDebugEnabled(AOSystems.EnsuredLoyalty, DebugType.Technical, LeavingClan))
            {
                MessageHelper.TechnicalMessage($"Score for Kingdom {LeavingClan.Kingdom.Name} to withhold Clan {LeavingClan.Name}.\nRelativeScoreToLeave = {RelativeScoreToLeave:N}. CostScore = {CostScore:N}. ClanCountModifier = {ClanCountModifier}. ScoreOfKingdomToGetClan = {Campaign.Current.Models.DiplomacyModel.GetScoreOfKingdomToGetClan(LeavingClan.Kingdom, LeavingClan):N}. SettlementValue = {SettlementValue:N}. Result = {Result:N}.");
            }
            return (int) Result;
        }
        public bool CheckAIWithholdDecision()
        {
            return
              WithholdCost is null
              || (LeavingClan.Kingdom.RulingClan.Influence > WithholdCost.InfluenceCost && LeavingClan.Kingdom.Leader.Gold > WithholdCost.GoldCost && GetScoreForKingdomToWithholdClan() >= 0);
        }
        public bool GetAIWithholdDecision()
        {
            bool decisionIsWithhold = CheckAIWithholdDecision();
            if (SettingsHelper.SystemDebugEnabled(AOSystems.EnsuredLoyalty, DebugType.General, LeavingClan))
            {
                ApplyAIWithholdDecisionWithLogging(decisionIsWithhold);
            }
            else
            {
                ApplyAIWithholdDecision(decisionIsWithhold);
            }
            return decisionIsWithhold;
        }
        private void ApplyAIWithholdDecisionWithLogging(bool decisionIsWithhold)
        {
            if (WithholdCost is null)
            {
                return;
            }
            float InitialInfluence = LeavingClan.Kingdom.RulingClan.Influence;
            int InitilalGold = LeavingClan.Kingdom.Leader.Gold;
            TextObject textObject = new(decisionIsWithhold ? WithholdPricePayed : WithholdPriceRejected);
            SetEntityProperties(textObject, "LEAVING_CLAN", LeavingClan, true);
            SetNumericVariable(textObject, "INITIAL_INFLUENCE", InitialInfluence, "N0");
            SetNumericVariable(textObject, "INITILAL_GOLD", InitilalGold, "N0");
            SetNumericVariable(textObject, "INFLUENCE_COST", WithholdCost.InfluenceCost, "N0");
            SetNumericVariable(textObject, "GOLD_COST", WithholdCost.GoldCost, "N0");
            if (decisionIsWithhold)
            {
                ApplyAIWithholdDecision(decisionIsWithhold);
                SetNumericVariable(textObject, "RESULT_INFLUENCE", LeavingClan.Kingdom.RulingClan.Influence, "N0");
                SetNumericVariable(textObject, "RESULT_GOLD", LeavingClan.Kingdom.Leader.Gold, "N0");
            }
            MessageHelper.SimpleMessage(textObject);
        }
        private void ApplyAIWithholdDecision(bool decisionIsWithhold)
        {
            if (!decisionIsWithhold || WithholdCost is null)
            {
                return;
            }
            LeavingClan.Kingdom.RulingClan.Influence = MBMath.ClampFloat(LeavingClan.Kingdom.RulingClan.Influence - WithholdCost.InfluenceCost, 0f, float.MaxValue);
            LeavingClan.Kingdom.Leader.Gold = MBMath.ClampInt(LeavingClan.Kingdom.Leader.Gold - WithholdCost.GoldCost, 0, int.MaxValue);
            LeavingClan.Leader.Gold += MBMath.ClampInt(LeavingClan.Kingdom.Leader.Gold - WithholdCost.GoldCost, 0, int.MaxValue);
        }

        public void AwaitPlayerDecision()
        {
            TextObject InquiryHeader = new(PlayerInquiryHeader);
            SetEntityProperties(InquiryHeader, "LEAVING_CLAN", LeavingClan);

            TextObject InquiryBody = new(PlayerInquiryBody);
            SetEntityProperties(InquiryBody, "LEAVING_CLAN", LeavingClan);
            SetEntityProperties(null, "TARGET_KINGDOM", TargetKingdom);
            InquiryBody.SetTextVariable("ACTION_DESCRIPTION", new TextObject(TargetKingdom != null ? ActionDefecting : ActionLeaving));
            SetNumericVariable(InquiryBody, "INFLUENCE_COST", WithholdCost!.InfluenceCost, "N0");
            SetNumericVariable(InquiryBody, "GOLD_COST", WithholdCost.GoldCost, "N0");

            InformationManager.ShowInquiry(new InquiryData(InquiryHeader.ToString(), InquiryBody.ToString(), true, true, ButtonWithholdText.ToLocalizedString(), ButtonReleaseText.ToLocalizedString(), () => ApplyPlayerDecision(true), () => ApplyPlayerDecision(false)), true);
        }

        public void ApplyPlayerDecision(bool decisionIsWithhold)
        {
            if (SettingsHelper.SystemDebugEnabled(AOSystems.EnsuredLoyalty, DebugType.General, LeavingClan))
            {
                ApplyAIWithholdDecisionWithLogging(decisionIsWithhold);
            }
            else
            {
                ApplyAIWithholdDecision(decisionIsWithhold);
            }
            if (!decisionIsWithhold)
            {
                if (TargetKingdom is null)
                {
                    (ClanBarterable as LeaveKingdomAsClanBarterable)!.Apply();
                }
                else
                {
                    Patches.Loyalty.ExecuteAiBarterReversePatch.ExecuteAiBarter(Campaign.Current.BarterManager ?? BarterManager.Instance, LeavingClan, TargetKingdom, LeavingClan.Leader, TargetKingdom.Leader, ClanBarterable!);
                }
            }
        }
    }
}