using TaleWorlds.CampaignSystem;
using AllegianceOverhaul.CampaignBehaviors.BehaviorManagers;
using TaleWorlds.CampaignSystem.Election;

namespace AllegianceOverhaul.CampaignBehaviors
{
  public class AOCooldownBehavior : CampaignBehaviorBase
  {
    private AOCooldownManager _cooldownManager;

    public AOCooldownBehavior()
    {
      this._cooldownManager = new AOCooldownManager();
    }

    public override void RegisterEvents()
    {
      CampaignEvents.KingdomDecisionConcluded.AddNonSerializedListener(this, RegisterKingdomDecisionConcluded);
      AOEvents.OnPlayerGotJoinRequestEvent.AddNonSerializedListener(this, RegisterPlayerJoinRequest);
    }

    private void RegisterKingdomDecisionConcluded(KingdomDecision decision, DecisionOutcome chosenOutcome, bool isPlayerInvolved)
    {
      if (AOCooldownManager.SupportedDecisionTypes.Contains(decision.GetType()))
        _cooldownManager.UpdateKingdomDecisionHistory(decision, chosenOutcome, CampaignTime.Now);
    }

    private void RegisterPlayerJoinRequest()
    {
      _cooldownManager.UpdateJoinPlayerRequestHistory(CampaignTime.Now);
    }

    public override void SyncData(IDataStore dataStore)
    {
      dataStore.SyncData("_cooldownManager", ref _cooldownManager);
      if (dataStore.IsLoading)
      {
        if (_cooldownManager == null)
        {
          _cooldownManager = new AOCooldownManager();
        }
        _cooldownManager.Sync();
      }
    }
  }
}
