using System;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

using static AllegianceOverhaul.Helpers.LocalizationHelper;

namespace AllegianceOverhaul.PoliticsRebalance
{
    internal static class PoliticsDebugHelper
    {
        private const string OutcomeIsAbstain = "{=8osThex4K}They chose not to bring this matter to the royal court.";
        private const string OutcomeIsElectionStart = "{=MiKzBv8hv}They chose to initiate new kingdom election on this matter.";

        private const string PossibleActionMakingPeace = "{=XB9p2y7ud}are in the good mood and would like to make peace with someone";
        private const string PossibleActionDeclaringWar = "{=jMY459MFu}are in the bad mood and would like to declare war on someone";
        private const string PossibleActionChangingKingdomPolicy = "{=SK0HqxegU}are in the worries about the fates of the kingdom and would like to change some of its policies";
        private const string PossibleActionAnnexingFief = "{=LFrk9H8l1}are vaguely feel a certain injustice in the current assignment of kingdom lands and would like to redistribute some fiefs";

        private const string ActionMakingPeace = "{=zkoVo2k1B}making peace with";
        private const string ActionDeclaringWar = "{=NPVP3pLvJ}declaring war on";
        private const string ActionIntroducingKingdomPolicy = "{=h7u4qQSDf}introducing a new policy of";
        private const string ActionDisavowingKingdomPolicy = "{=Hz1c1I7Ux}disavowing the policy of";
        private const string ActionAnnexingFief = "{=JnzHpQzql}annexing {?ANNEXED_SETTLEMENT.IS_TOWN}town of{?}{?ANNEXED_SETTLEMENT.IS_VILLAGE}village of{?}{\\?}{\\?}";

        private const string AnnexingFiefActionDetails = "{=09ZE4GRyY} from {ANNEXED_SETTLEMENT_CLAN.NAME}";

        private const string NoFactionFound = "{=6LqEiBkdd}{?REFLECTING_CLAN.MINOR_FACTION}Minor faction{?}The clan{\\?} {REFLECTING_CLAN.NAME} {?REFLECTING_CLAN.UNDER_CONTRACT}under mercenary service of{?}of{\\?} {REFLECTING_CLAN_KINGDOM.NAME} {POSSIBLE_ACTION}, but found no suitable candidates.";
        private const string ConsiderationDescription = "{=n3OiPQh4b}{?REFLECTING_CLAN.MINOR_FACTION}Minor faction{?}The clan{\\?} {REFLECTING_CLAN.NAME} {?REFLECTING_CLAN.UNDER_CONTRACT}under mercenary service of{?}of{\\?} {REFLECTING_CLAN_KINGDOM.NAME} considered {CONSIDERED_ACTION} {ACTION_TARGET}{ACTION_DETAILS}. {CONSIDER_OUTCOME}";

        public static void PrepareConsiderationDebugMessage(ConsiderationType considerationType, Clan clan, IFaction? otherFaction, KingdomDecision? clanDecision, out TextObject debugLogMessage)
        {
            if (otherFaction != null)
            {
                SetConsideredAction(out debugLogMessage, otherFaction.Name, clanDecision, considerationType);
            }
            else
            {
                SetPossibleAction(out debugLogMessage, considerationType);
            }
            SetEntityProperties(debugLogMessage, "REFLECTING_CLAN", clan);
        }

        public static void PrepareConsiderationDebugMessage(Clan clan, PolicyObject? policyObject, KingdomDecision? clanDecision, out TextObject debugLogMessage)
        {
            if (policyObject != null)
            {
                SetConsideredAction(out debugLogMessage, policyObject.Name, clanDecision, ConsiderationType.ChangingKingdomPolicy, revertPolicy: clan.Kingdom.ActivePolicies.Contains(policyObject));
            }
            else
            {
                SetPossibleAction(out debugLogMessage, ConsiderationType.ChangingKingdomPolicy);
            }
            SetEntityProperties(debugLogMessage, "REFLECTING_CLAN", clan);
        }

        public static void PrepareConsiderationDebugMessage(Clan clan, Town? fiefBeingAnnexed, KingdomDecision? clanDecision, out TextObject debugLogMessage)
        {
            if (fiefBeingAnnexed != null)
            {
                SetConsideredAction(out debugLogMessage, fiefBeingAnnexed.Name, clanDecision, ConsiderationType.AnnexingFief, annexedSettlement: fiefBeingAnnexed.Settlement);
            }
            else
            {
                SetPossibleAction(out debugLogMessage, ConsiderationType.AnnexingFief);
            }
            SetEntityProperties(debugLogMessage, "REFLECTING_CLAN", clan);
        }

        private static void SetConsideredAction(out TextObject textObject, TextObject actionTargetName, KingdomDecision? clanDecision, ConsiderationType considerationType, bool revertPolicy = false, Settlement? annexedSettlement = null)
        {
            textObject = new TextObject(ConsiderationDescription);
            switch (considerationType)
            {
                case ConsiderationType.MakingPeace:
                    textObject.SetTextVariable("CONSIDERED_ACTION", ActionMakingPeace);
                    textObject.SetTextVariable("ACTION_DETAILS", TextObject.Empty);
                    break;
                case ConsiderationType.DeclaringWar:
                    textObject.SetTextVariable("CONSIDERED_ACTION", ActionDeclaringWar);
                    textObject.SetTextVariable("ACTION_DETAILS", TextObject.Empty);
                    break;
                case ConsiderationType.ChangingKingdomPolicy:
                    textObject.SetTextVariable("CONSIDERED_ACTION", revertPolicy ? ActionDisavowingKingdomPolicy : ActionIntroducingKingdomPolicy);
                    textObject.SetTextVariable("ACTION_DETAILS", TextObject.Empty);
                    break;
                case ConsiderationType.AnnexingFief:
                    SetEntityProperties(null, "ANNEXED_SETTLEMENT", annexedSettlement);
                    textObject.SetTextVariable("CONSIDERED_ACTION", new TextObject(ActionAnnexingFief));
                    textObject.SetTextVariable("ACTION_DETAILS", new TextObject(AnnexingFiefActionDetails));
                    break;
                default:
                    throw new ArgumentException(string.Format("{0} is not supported Consideration type", considerationType.ToString()), nameof(considerationType));
            }
            textObject.SetTextVariable("ACTION_TARGET", actionTargetName);
            textObject.SetTextVariable("CONSIDER_OUTCOME", clanDecision is null ? OutcomeIsAbstain : OutcomeIsElectionStart);
        }

        private static void SetPossibleAction(out TextObject textObject, ConsiderationType considerationType)
        {
            textObject = new TextObject(NoFactionFound);
            switch (considerationType)
            {
                case ConsiderationType.MakingPeace:
                    textObject.SetTextVariable("POSSIBLE_ACTION", PossibleActionMakingPeace);
                    break;
                case ConsiderationType.DeclaringWar:
                    textObject.SetTextVariable("POSSIBLE_ACTION", PossibleActionDeclaringWar);
                    break;
                case ConsiderationType.ChangingKingdomPolicy:
                    textObject.SetTextVariable("POSSIBLE_ACTION", PossibleActionChangingKingdomPolicy);
                    break;
                case ConsiderationType.AnnexingFief:
                    textObject.SetTextVariable("POSSIBLE_ACTION", PossibleActionAnnexingFief);
                    break;
                default:
                    throw new ArgumentException(string.Format("{0} is not supported Consideration type", considerationType.ToString()), nameof(considerationType));
            }
        }
    }

    public enum ConsiderationType : byte
    {
        None,
        MakingPeace,
        DeclaringWar,
        ChangingKingdomPolicy,
        AnnexingFief
    }
}