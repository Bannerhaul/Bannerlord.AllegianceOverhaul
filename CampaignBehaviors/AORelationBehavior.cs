using TaleWorlds.CampaignSystem;
using AllegianceOverhaul.CampaignBehaviors.BehaviorManagers;
using TaleWorlds.CampaignSystem.Election;
using AllegianceOverhaul.Helpers;
using AllegianceOverhaul.SavableClasses;
using System;
using System.Linq;
using System.Collections.Generic;
using AllegianceOverhaul.Extensions;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
      /*
      List<Hero> tst = Hero.All.OrderByDescending(h => h.Id).ToList();
      CampaignDescriptor cd = Campaign.Current.GetCampaignDescriptor();
      cd = new CampaignDescriptor(Hero.MainHero.Father);
      string s = cd.Descriptor;
      cd = new CampaignDescriptor(Hero.MainHero.Spouse);
      s = cd.Descriptor;

      CampaignDescriptor NewCampaignDescriptor;
      byte[] serialisedInfo;
      using (MemoryStream file = new MemoryStream())
      {
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, cd);
        serialisedInfo = file.ToArray();
      }
      using (MemoryStream file = new MemoryStream(serialisedInfo))
      {
        BinaryFormatter bf = new BinaryFormatter();
        NewCampaignDescriptor = (CampaignDescriptor)bf.Deserialize(file);
      }
      */
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
