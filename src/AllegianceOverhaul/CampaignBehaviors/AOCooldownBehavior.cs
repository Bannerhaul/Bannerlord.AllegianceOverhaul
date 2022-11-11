using AllegianceOverhaul.CampaignBehaviors.BehaviorManagers;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;

namespace AllegianceOverhaul.CampaignBehaviors
{
    public class AOCooldownBehavior : CampaignBehaviorBase
    {
        private AOCooldownManager _cooldownManagerAO;

        public AOCooldownBehavior()
        {
            this._cooldownManagerAO = new AOCooldownManager();
        }

        public override void RegisterEvents()
        {
            CampaignEvents.KingdomDecisionConcluded.AddNonSerializedListener(this, RegisterKingdomDecisionConcluded);
            AOEvents.OnPlayerGotJoinRequestEvent.AddNonSerializedListener(this, RegisterPlayerJoinRequest);
        }

        private void RegisterKingdomDecisionConcluded(KingdomDecision decision, DecisionOutcome chosenOutcome, bool isPlayerInvolved)
        {
            if (AOCooldownManager.SupportedDecisionTypes.Contains(decision.GetType()))
                _cooldownManagerAO.UpdateKingdomDecisionHistory(decision, chosenOutcome, CampaignTime.Now);
        }

        private void RegisterPlayerJoinRequest()
        {
            _cooldownManagerAO.UpdateJoinPlayerRequestHistory(CampaignTime.Now);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("_cooldownManagerAO", ref _cooldownManagerAO);
            if (dataStore.IsLoading)
            {
                if (_cooldownManagerAO == null)
                {
                    _cooldownManagerAO = new AOCooldownManager();
                }
                _cooldownManagerAO.Sync();
            }
        }
    }
}