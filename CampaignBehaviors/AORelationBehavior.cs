using System;
using System.Linq;
using System.Collections.Generic;
using AllegianceOverhaul.Extensions;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;

using AllegianceOverhaul.CampaignBehaviors.BehaviorManagers;
using AllegianceOverhaul.Helpers;
using AllegianceOverhaul.SavableClasses;

namespace AllegianceOverhaul.CampaignBehaviors
{
  public class AORelationBehavior : CampaignBehaviorBase
  {
    private AORelationManager _relationManager;

    public AORelationBehavior()
    {
      _relationManager = new AORelationManager();
    }

    public override void RegisterEvents()
    {
      AOEvents.OnRelationShiftEvent.AddNonSerializedListener(this, RegisterRelationShifted);
      CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, InitializeRelationManager);
    }

    private void InitializeRelationManager(CampaignGameStarter gameStarterObject)
    {
      if (!_relationManager.HeroRelationsInitialized)
      {
        _relationManager.InitializeAOHeroRelations();
      }
    }

    private void RegisterRelationShifted(Hero baseHero, Hero otherHero, SegmentalFractionalScore fractionalScore)
    {

      MessageHelper.SimpleMessage(string.Format("RegisterRelationShifted. baseHero: {0}. otherHero: {1}. fractionalScore = {2}.", baseHero.Name, otherHero.Name, fractionalScore));
    }

    public override void SyncData(IDataStore dataStore)
    {
      dataStore.SyncData("_relationManager", ref _relationManager);
      if (dataStore.IsLoading)
      {
        if (_relationManager == null)
        {
          _relationManager = new AORelationManager();
        }
        _relationManager.Sync();
      }
    }
  }
}
