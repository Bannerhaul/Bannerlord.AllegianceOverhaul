using System.Collections.Generic;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

using AllegianceOverhaul.Models;

namespace AllegianceOverhaul
{
  public sealed class AOGameModels : GameModelsManager
  {
    public DecisionSupportScoringModel DecisionSupportScoringModel { get; private set; }

    public AOGameModels(IEnumerable<GameModel> inputComponents) : base(inputComponents)
    {
      GetSpecificGameBehaviors();
      MakeGameComponentBindings();
    }
    private void GetSpecificGameBehaviors()
    {
      if (Campaign.Current.GameMode == CampaignGameMode.Campaign || Campaign.Current.GameMode == CampaignGameMode.Tutorial)
      {
        DecisionSupportScoringModel = GetGameModel<DecisionSupportScoringModel>();
      }
    }
    private void MakeGameComponentBindings()
    {
    }
    public static IEnumerable<GameModel> GetAOGameModels(CampaignGameStarter gameStarter)
    {
      foreach (GameModel gameModel in gameStarter.Models)
      {
        if (gameModel is DecisionSupportScoringModel)
          yield return gameModel;
      }
    }
    public static AOGameModels Instance { get; internal set; }
  }
}
