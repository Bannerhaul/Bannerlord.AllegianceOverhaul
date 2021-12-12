using AllegianceOverhaul.Helpers;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using TaleWorlds.CampaignSystem.Election;

namespace AllegianceOverhaul.Patches.Politics
{
    [HarmonyPatch(typeof(KingdomElection), "GetAiChoice")]
    public static class GetAiChoicePatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                int searchedIndex = -1;
                List<CodeInstruction> codes = new(instructions);
                for (int i = 1; i < codes.Count - 1; ++i)
                {
                    if (codes[i].Is(OpCodes.Ldc_R4, 10) && codes[i - 1].opcode == OpCodes.Ldloc_S && codes[i + 1].opcode == OpCodes.Ble_Un_S)
                    {
                        searchedIndex = i;
                        break;
                    }
                }
                if (searchedIndex > -1)
                {
                    codes[searchedIndex] = codes[searchedIndex].Clone(Settings.Instance!.OverrideDecisionScoreThreshold);
                }
                return codes.AsEnumerable();
            }
            catch (Exception ex)
            {
                MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                DebugHelper.HandleException(ex, methodInfo, "Harmony transpiler for KingdomElection. GetAiChoice");
                return instructions;
            }
        }

        public static bool Prepare()
        {
            return SettingsHelper.SubSystemEnabled(SubSystemType.ElectionRebalance);
        }
    }
}
