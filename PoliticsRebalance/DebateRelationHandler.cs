using TaleWorlds.CampaignSystem.Election;

namespace AllegianceOverhaul.PoliticsRebalance
{
    public static class DebateRelationHandler
    {
        private delegate KingdomDecision.SupportStatus GetSupportStatusOfDecisionOutcomeDelegate(KingdomElection instance, DecisionOutcome chosenOutcome);

        //private static readonly GetSupportStatusOfDecisionOutcomeDelegate deGetSupportStatusOfDecisionOutcome = AccessHelper.GetDelegate<GetSupportStatusOfDecisionOutcomeDelegate>(typeof(KingdomElection), "GetSupportStatusOfDecisionOutcome");
        /*
        public static void GetShiftOfSupporersInterrelations(KingdomElection kingdomElection)
        {

        }
        */
    }
}
