using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using AllegianceOverhaul.Extensions;
using AllegianceOverhaul.Extensions.Harmony;

namespace AllegianceOverhaul.Helpers
{
  internal static class HarmonyHelper
  {
    public static bool PatchAll(ref Harmony? harmonyInstance, string sectionName, string logMessage, string chatMessage = "")
    {
      try
      {
        if (harmonyInstance is null)
          harmonyInstance = new Harmony("Bannerlord.AllegianceOverhaul");
        harmonyInstance.PatchAll();
        return true;
      }
      catch (Exception ex)
      {
        DebugHelper.HandleException(ex, sectionName, logMessage, chatMessage);
        return false;
      }
    }

    public static bool ReportCompatibilityIssues(Harmony? harmonyInstance, string sectionName)
    {
      IEnumerable<MethodBase> myOriginalMethods = harmonyInstance!.GetPatchedMethods();
      HarmonyLib.Patches patches;
      bool Result = false;
      StringBuilder PossibleConflictsInfo = new StringBuilder("");
      foreach (MethodBase MyMethod in myOriginalMethods)
      {
        // get info about all Patches on OriginalMethod
        patches = Harmony.GetPatchInfo(MyMethod);
        if (patches.CheckForCompetition(harmonyInstance, out string DebugInfo, Settings.Instance!.HarmonyCheckupIgnoreList.ToReadOnlyCollection()))
        {
          Result = true;
          if (PossibleConflictsInfo.Length <= 0)
            PossibleConflictsInfo.Append($"[{DateTime.Now:dd.MM.yyyy HH:mm:ss}] - {sectionName}.\nForeign patches checkup for method {MyMethod.DeclaringType.FullName}.{MyMethod.Name}, used by AllegianceOverhaul, revealed possible conflicts!{DebugInfo}");
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
