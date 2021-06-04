using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

using AllegianceOverhaul.Helpers;
using AllegianceOverhaul.SavableClasses;

using Bannerlord.ButterLib.Common.Helpers;

namespace AllegianceOverhaul.CampaignBehaviors.BehaviorManagers
{
  //[SaveableClass(101)]
  public class AORelationManager
  {
    [SaveableField(1)]
    private Dictionary<ulong, SegmentalFractionalScore> _fractionalHeroRelations;
    [SaveableField(2)]
    private Dictionary<ulong, int> _AOHeroRelations;

    public bool HeroRelationsInitialized => _AOHeroRelations.Count > 0;

    internal AORelationManager()
    {
      _fractionalHeroRelations = new Dictionary<ulong, SegmentalFractionalScore>();
      _AOHeroRelations = new Dictionary<ulong, int>();
    }

    public void UpdatePersonalRelation(Hero baseHero, Hero otherHero, ref SegmentalFractionalScore fractionalScore)
    {
      if (fractionalScore.ReadyToExtract())
      {
        fractionalScore = fractionalScore.ExtractWholeParts(out SegmentalFractionalScore integerScrore);
        UpdatePersonalRelation(baseHero, otherHero, (int)(integerScrore.PositiveScore - integerScrore.NegativeScore));
      }
    }

    public void UpdateFractionalRelationScore(Hero baseHero, Hero otherHero, SegmentalFractionalScore fractionalScore)
    {
      ulong heroTuple = ElegantPairHelper.Pair(baseHero.Id, otherHero.Id);
      fractionalScore += GetFractionalRelationScore(heroTuple);
      UpdatePersonalRelation(baseHero, otherHero, ref fractionalScore);
      SetFractionalRelationScore(heroTuple, fractionalScore);
    }

    public SegmentalFractionalScore GetFractionalRelationScore(Hero baseHero, Hero otherHero)
    {
      return GetFractionalRelationScore(ElegantPairHelper.Pair(baseHero.Id, otherHero.Id));
    }

    public void SetFractionalRelationScore(Hero baseHero, Hero otherHero, SegmentalFractionalScore fractionalScore)
    {
      UpdatePersonalRelation(baseHero, otherHero, ref fractionalScore);
      SetFractionalRelationScore(ElegantPairHelper.Pair(baseHero.Id, otherHero.Id), fractionalScore);
    }

    //Internals
    private SegmentalFractionalScore GetFractionalRelationScore(ulong heroTuple)
    {
      return _fractionalHeroRelations.TryGetValue(heroTuple, out SegmentalFractionalScore currentScore) ? currentScore : new SegmentalFractionalScore(0, 0);
    }

    private void SetFractionalRelationScore(ulong heroTuple, SegmentalFractionalScore fractionalScore)
    {
      _fractionalHeroRelations[heroTuple] = fractionalScore;
    }

    private void UpdatePersonalRelation(Hero baseHero, Hero otherHero, int relationChange)
    {
      int newValue = MBMath.ClampInt(CharacterRelationManager.GetHeroRelation(baseHero, otherHero) + relationChange, -100, 100);
      CharacterRelationManager.SetHeroRelation(baseHero, otherHero, newValue);
    }

    internal void InitializeAOHeroRelations()
    {
      Dictionary<long, int> nativeRelations = new Dictionary<long, int>(FieldAccessHelper.heroRelationsByRef(FieldAccessHelper.heroRelationsInstanceByRef(CharacterRelationManager.Instance)));
      Dictionary<ulong, int> heroRelations = new Dictionary<ulong, int>();
      foreach (Hero baseHero in Hero.All.Where(h => (h.IsNoble || h.IsWanderer) && h != Hero.MainHero))
      {
        foreach (Hero otherHero in Hero.All.Where(h => (h.IsNoble || h.IsWanderer) && h != baseHero))
        {
          heroRelations.Add(ElegantPairHelper.Pair(baseHero.Id, otherHero.Id), nativeRelations.TryGetValue(MBGUID.GetHash2(baseHero.Id, otherHero.Id), out int relation) ? relation : 0);
        }
      }
      _AOHeroRelations = heroRelations;
    }

    internal void Sync()
    {
      if (_fractionalHeroRelations == null)
      {
        _fractionalHeroRelations = new Dictionary<ulong, SegmentalFractionalScore>();
      }
      if (_AOHeroRelations == null)
      {
        _AOHeroRelations = new Dictionary<ulong, int>();
      }
    }
  }
}
