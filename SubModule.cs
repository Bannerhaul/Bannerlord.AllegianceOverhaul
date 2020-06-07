using System;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using AllegianceOverhaul.Extensions.Harmony;

namespace AllegianceOverhaul
{
  class SubModule : MBSubModuleBase
  {
    public Harmony AllegianceOverhaulHarmonyInstance { get; private set; }
    protected override void OnBeforeInitialModuleScreenSetAsRoot()
    {
      base.OnBeforeInitialModuleScreenSetAsRoot();
      try
      {
        AllegianceOverhaulHarmonyInstance = new Harmony("Bannerlord.AllegianceOverhaul");
        AllegianceOverhaulHarmonyInstance.PatchAll();
        
        InformationManager.DisplayMessage(new InformationMessage("Loaded Allegiance Overhaul!", Color.FromUint(4282569842U)));

        //check for possible conflicts
        if (InitialHarmonyCheck())
          InformationManager.DisplayMessage(new InformationMessage("Allegiance Overhaul identified possible conflicts with other mods! See details in the mod log.", Colors.Yellow));
      }
      catch (Exception ex)
      {
        InformationManager.DisplayMessage(new InformationMessage($"Error initialising Allegiance Overhaul! See details in the mod log.\nError text: {ex.Message}", Colors.Red));
        LoggingHelper.Log(string.Format("Initialization error - {0}", ex.ToString()), "OnBeforeInitialModuleScreenSetAsRoot");
      }
    }

    private bool InitialHarmonyCheck()
    {
      IEnumerable<MethodBase> myOriginalMethods = AllegianceOverhaulHarmonyInstance.GetPatchedMethods();
      HarmonyLib.Patches patches;
      bool Result = false;
      StringBuilder PossibleConflictsInfo = new StringBuilder("");
      foreach (MethodBase MyMethod in myOriginalMethods)
      {
        // get info about all Patches on OriginalMethod
        patches = Harmony.GetPatchInfo(MyMethod);
        if (patches.CheckForCompetition(AllegianceOverhaulHarmonyInstance, out string DebugInfo))
        {
          Result = true;
          if (PossibleConflictsInfo.Length <= 0)
            PossibleConflictsInfo.Append($"[{DateTime.Now:dd.MM.yyyy HH:mm:ss}] - Checkup on initialize.\nForeign patches checkup for method {MyMethod.DeclaringType.FullName}.{MyMethod.Name}, used by AllegianceOverhaul, revealed possible conflicts!{DebugInfo}");
          else
            PossibleConflictsInfo.Append($"\nPatches checkup for method {MyMethod.DeclaringType.FullName}.{MyMethod.Name}, used by AllegianceOverhaul, revealed possible conflicts!{DebugInfo}");
        }
      }
      if (Result)
        LoggingHelper.Log(PossibleConflictsInfo.ToString());
      return Result;
    }
  }
}
