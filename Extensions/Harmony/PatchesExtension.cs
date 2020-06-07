using System.Text;
using System.Collections.ObjectModel;
using HarmonyLib;

namespace AllegianceOverhaul.Extensions.Harmony
{
  public static class PatchesExtension
  {
    public static bool CheckForCompetition (this HarmonyLib.Patches patches, HarmonyLib.Harmony DomesticHarmoy, out string DebugInfo)
    {
      DebugInfo = "";

      bool PrefixesHaveCompetitors = CheckCollectionForCompetition(patches.Prefixes, DomesticHarmoy, out string CollectionDebugInfo, true);
      if (PrefixesHaveCompetitors)
        DebugInfo = $"\nPossible conflict found in prefixes.\n" + CollectionDebugInfo;
      
      bool PostfixesHaveCompetitors = CheckCollectionForCompetition(patches.Postfixes, DomesticHarmoy, out CollectionDebugInfo);
      if (PostfixesHaveCompetitors)
        DebugInfo = $"\nPossible conflict found in postfixes.\n" + CollectionDebugInfo;

      return PrefixesHaveCompetitors || PostfixesHaveCompetitors;
    }

    private static bool CheckCollectionForCompetition(ReadOnlyCollection<Patch> patches, HarmonyLib.Harmony DomesticHarmoy, out string DebugInfo, bool CheckResultType = false)
    {
      bool PatchHaveCompetitors = false;
      bool DomesticDoesCompete = false;
      StringBuilder ForeignPatchesInfo = new StringBuilder("");
      StringBuilder DomesticPatchesInfo = new StringBuilder("");
      foreach (Patch patch in patches)
      {
        if (patch.owner != DomesticHarmoy.Id)
        {
          if (!CheckResultType || patch.PatchMethod.ReturnType == typeof(bool))
          {
            PatchHaveCompetitors = true;
            ForeignPatchesInfo.Append($"\n{patch.GetDebugString()}");
            ForeignPatchesInfo.Append($"\t{patch.PatchMethod.ReturnType}");
          }
        }
        else
        {
          if (!CheckResultType || patch.PatchMethod.ReturnType == typeof(bool))
          {
            DomesticDoesCompete = true;
            DomesticPatchesInfo.Append($"\n{patch.GetDebugString()}");
          }
        }
      }

      bool Result = PatchHaveCompetitors && DomesticDoesCompete;
      DebugInfo = Result ? $"\tThese Harmony patches:\n\t{{{ForeignPatchesInfo}\n\t}}\n\tmay have conflicts with below AllegianceOverhaul patches:\n\t{{{DomesticPatchesInfo}\n\t}}" : "";
      return Result;
    }
  }
}
