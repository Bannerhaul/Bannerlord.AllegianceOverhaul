using HarmonyLib;

using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace AllegianceOverhaul.Extensions.Harmony
{
    public static class PatchesExtensions
    {
        public static bool CheckForCompetition(this HarmonyLib.Patches patches, HarmonyLib.Harmony domesticHarmoy, out string debugInfo, ReadOnlyCollection<string>? ignoreList = null)
        {
            bool Result = CheckForCompetition(patches, domesticHarmoy, out StringBuilder debugInfoBuilder, ignoreList);
            debugInfo = debugInfoBuilder.ToString();
            return Result;
        }
        public static bool CheckForCompetition(this HarmonyLib.Patches patches, HarmonyLib.Harmony domesticHarmoy, out StringBuilder debugInfoBuilder, ReadOnlyCollection<string>? ignoreList = null)
        {
            debugInfoBuilder = new StringBuilder(string.Empty);
            patches.GetBoolPrefixes(out ReadOnlyCollection<Patch> BoolPrefixes);
            return
              CheckCollectionForCompetition(BoolPrefixes, domesticHarmoy, "prefixes", ref debugInfoBuilder, ignoreList)
              || CheckCollectionForCompetition(patches.Postfixes, domesticHarmoy, "postfixes", ref debugInfoBuilder, ignoreList)
              || CheckCollectionForCanceling(patches.Postfixes, BoolPrefixes, "postfixes", ref debugInfoBuilder, ignoreList)
              || CheckCollectionForCompetition(patches.Transpilers, domesticHarmoy, "transpilers", ref debugInfoBuilder, ignoreList)
              || CheckCollectionForCanceling(patches.Transpilers, BoolPrefixes, "transpilers", ref debugInfoBuilder, ignoreList);
        }

        public static void GetBoolPrefixes(this HarmonyLib.Patches patches, out ReadOnlyCollection<Patch> boolPrefixes)
        {
            boolPrefixes = patches.Prefixes.Where(patch => patch.PatchMethod.ReturnType == typeof(bool)).ToList().AsReadOnly();
        }

        private static bool CheckCollectionForCompetition(ReadOnlyCollection<Patch> collection, HarmonyLib.Harmony domesticHarmoy, string patchTypeName, ref StringBuilder debugInfoBuilder, ReadOnlyCollection<string>? ignoreList = null)
        {
            bool PatchHasCompetitors = false;
            bool PatchDoesCompete = false;
            StringBuilder ForeignPatchesInfo = new(string.Empty);
            StringBuilder DomesticPatchesInfo = new(string.Empty);
            foreach (Patch patch in collection)
            {
                if (patch.owner != domesticHarmoy.Id)
                {
                    if (ignoreList is null || !ignoreList.Contains(patch.PatchMethod.DeclaringType.Assembly.GetName().Name))
                    {
                        PatchHasCompetitors = true;
                        if (ForeignPatchesInfo.Length > 0)
                            ForeignPatchesInfo.Append($"\n\n{patch.GetDebugString()}");
                        else
                            ForeignPatchesInfo.Append($"\n{patch.GetDebugString()}");
                    }
                }
                else
                {
                    PatchDoesCompete = true;
                    if (DomesticPatchesInfo.Length > 0)
                        DomesticPatchesInfo.Append($"\n\n{patch.GetDebugString()}");
                    else
                        DomesticPatchesInfo.Append($"\n{patch.GetDebugString()}");
                }
            }

            bool Result = PatchHasCompetitors && PatchDoesCompete;
            if (Result)
            {
                debugInfoBuilder.Append($"\nPossible conflict found in {patchTypeName}.\n");
                debugInfoBuilder.Append($"\tThese Harmony patches:\n\t{{{ForeignPatchesInfo}\n\t}}\n\tmay have conflicts with below AllegianceOverhaul patches:\n\t{{{DomesticPatchesInfo}\n\t}}");
            }
            return Result;
        }

        private static bool CheckCollectionForCanceling(ReadOnlyCollection<Patch> collection, ReadOnlyCollection<Patch> boolPrefixes, string patchTypeName, ref StringBuilder debugInfoBuilder, ReadOnlyCollection<string>? ignoreList = null)
        {
            bool PossibleUnexpectedSkip = false;
            StringBuilder SkippedPatchesInfo = new(string.Empty);
            if (boolPrefixes != null)
                foreach (Patch patch in collection)
                {
                    if (ignoreList is null || !ignoreList.Contains(patch.PatchMethod.DeclaringType.Assembly.GetName().Name))
                    {
                        StringBuilder SkippingPatchesInfo = new(string.Empty);
                        foreach (Patch prefix in boolPrefixes)
                        {
                            if (patch.owner != prefix.owner && (ignoreList is null || !ignoreList.Contains(prefix.PatchMethod.DeclaringType.Assembly.GetName().Name)))
                            {
                                PossibleUnexpectedSkip = true;
                                if (SkippingPatchesInfo.Length > 0)
                                    SkippingPatchesInfo.Append($"\n\n{prefix.GetDebugString()}");
                                else
                                    SkippingPatchesInfo.Append($"\n{prefix.GetDebugString()}");
                            }
                        }
                        if (SkippingPatchesInfo.Length > 0)
                            SkippedPatchesInfo.Append($"\n\tHarmony patch\n\t{{\n{patch.GetDebugString()}\n\t}}\n\tmay be not expecting that original method could be skipped by\n\t{{{SkippingPatchesInfo}\n\t}}");
                    }
                }

            if (PossibleUnexpectedSkip)
            {
                debugInfoBuilder.Append($"\nPossible unexpected skip found in {patchTypeName}.");
                debugInfoBuilder.Append(SkippedPatchesInfo.ToString());
            }
            return PossibleUnexpectedSkip;
        }
    }
}