using AllegianceOverhaul.CampaignBehaviors;
using AllegianceOverhaul.Extensions;
using AllegianceOverhaul.Helpers;

using HarmonyLib;

using System;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace AllegianceOverhaul
{
  public class SubModule : MBSubModuleBase
  {
    private const string SLoaded = "{=D9F5e8huP}Loaded Allegiance Overhaul!";
    private const string SErrorLoading = "{=HZ03jxuzC}Allegiance Overhaul failed to load! See details in the mod log.";
    private const string SErrorInitialising = "{=AE6zntclu}Error initialising Allegiance Overhaul! See details in the mod log. Error text: \"{EXCEPTION_MESSAGE}\"";
    private const string SConflicted = "{=9PyDYijvk}Allegiance Overhaul identified possible conflicts with other mods! See details in the mod log.";

    public bool Patched { get; private set; }
    public bool OnBeforeInitialModuleScreenSetAsRootWasCalled { get; private set; }

    private Harmony? _allegianceOverhaulHarmonyInstance;
    public Harmony? AllegianceOverhaulHarmonyInstance { get => _allegianceOverhaulHarmonyInstance; private set => _allegianceOverhaulHarmonyInstance = value; }

    protected override void OnSubModuleLoad()
    {
      base.OnSubModuleLoad();
      Patched = false;
    }

    protected override void OnBeforeInitialModuleScreenSetAsRoot()
    {
      base.OnBeforeInitialModuleScreenSetAsRoot();
      try
      {
        if (OnBeforeInitialModuleScreenSetAsRootWasCalled)
        {
          return;
        }
        OnBeforeInitialModuleScreenSetAsRootWasCalled = true;

        Patched = HarmonyHelper.PatchAll(ref _allegianceOverhaulHarmonyInstance, "OnSubModuleLoad", "Initialization error - {0}");
        if (Patched)
        {
          InformationManager.DisplayMessage(new InformationMessage(SLoaded.ToLocalizedString(), Color.FromUint(4282569842U)));
        }
        else
        {
          MessageHelper.ErrorMessage(SErrorLoading.ToLocalizedString());
        }

        //check for possible conflicts
        if (Settings.Instance!.EnableHarmonyCheckup && HarmonyHelper.ReportCompatibilityIssues(AllegianceOverhaulHarmonyInstance, "Checkup on initialize"))
        {
          MessageHelper.SimpleMessage(SConflicted.ToLocalizedString());
        }
      }
      catch (Exception ex)
      {
        DebugHelper.HandleException(ex, "OnBeforeInitialModuleScreenSetAsRoot", "Initialization error - {0}", SErrorInitialising);
      }
    }

    protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
    {
      base.OnGameStart(game, gameStarterObject);
      if (game.GameType is Campaign)
      {
        //Events
        AOEvents.Instance = new AOEvents();
        //CampaignGameStarter
        CampaignGameStarter gameStarter = (CampaignGameStarter)gameStarterObject;
        //Behaviors
        gameStarter.AddBehavior(new AOCooldownBehavior());
        //Individual relationships 
      }
    }
  }
}