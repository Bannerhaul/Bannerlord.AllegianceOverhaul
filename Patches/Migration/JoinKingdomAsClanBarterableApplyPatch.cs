extern alias TWCS;

using AllegianceOverhaul.Extensions.Harmony;
using AllegianceOverhaul.Helpers;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Barterables;

using TWCS::TaleWorlds.CampaignSystem.Actions;

namespace AllegianceOverhaul.Patches.Migration
{
    [HarmonyPatch(typeof(JoinKingdomAsClanBarterable), "Apply")]
    public static class JoinKingdomAsClanBarterableApplyPatch
    {
        private static readonly MethodInfo miApplyByLeaveKingdom = AccessTools.Method(typeof(ChangeKingdomAction), "ApplyByLeaveKingdom");
        private static readonly MethodInfo miLeaveOriginalKingdom = AccessTools.Method(typeof(JoinKingdomAsClanBarterableApplyPatch), "LeaveOriginalKingdom");

        public static void LeaveOriginalKingdom(JoinKingdomAsClanBarterable instance)
        {
            try
            {
                Clan originalClan = instance.OriginalOwner.Clan;
                Kingdom? originalKingdom = originalClan.Kingdom;

                bool kingIsLeaving = SettingsHelper.SubSystemEnabled(SubSystemType.LeaderDefectionFix) && (originalClan == originalKingdom?.RulingClan);
                bool shouldRebel = originalKingdom != null && instance.TargetKingdom != null && (kingIsLeaving || originalKingdom.IsAtWarWith(instance.TargetKingdom));

                if (shouldRebel)
                {
                    ChangeKingdomAction.ApplyByLeaveWithRebellionAgainstKingdom(originalClan, instance.TargetKingdom, true);
                    if (kingIsLeaving)
                    {
                        DestroyKingdomAction.Apply(originalKingdom);
                    }
                }
                else
                {
                    ChangeKingdomAction.ApplyByLeaveKingdom(originalClan, true);
                }
            }
            catch (Exception ex)
            {
                MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                DebugHelper.HandleException(ex, methodInfo, "Harmony patch for JoinKingdomAsClanBarterable. Apply");
            }
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                int originalKingdomConditionIndex = -1, replaceStartIndex = -1, replaceEndIndex = -1;
                List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
                for (int i = 0; i < codes.Count; ++i)
                {
                    if (FindOriginalKingdomCondition(originalKingdomConditionIndex, codes, i))
                    {
                        originalKingdomConditionIndex = i;
                        continue;
                    }
                    if (originalKingdomConditionIndex > 0 && replaceStartIndex < 0 && codes[i].opcode == OpCodes.Ldarg_0)
                    {
                        replaceStartIndex = i;
                        continue;
                    }
                    if (replaceStartIndex > 0 && replaceEndIndex < 0 && codes[i].opcode == OpCodes.Ldc_I4_1 && codes[i + 1].Calls(miApplyByLeaveKingdom))
                    {
                        replaceEndIndex = i + 2;
                        break;
                    }
                }
                if (replaceStartIndex < 0 || replaceEndIndex < 0)
                {
                    LogNoHooksIssue(originalKingdomConditionIndex, replaceStartIndex, replaceEndIndex, codes);
                }
                ReplaceCodeInstructions(replaceStartIndex, replaceEndIndex, codes);
                return codes.AsEnumerable();
            }
            catch (Exception ex)
            {
                MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                DebugHelper.HandleException(ex, methodInfo, "Harmony transpiler for JoinKingdomAsClanBarterable. Apply");
                return instructions;
            }

            //local methods
            static bool FindOriginalKingdomCondition(int originalKingdomConditionIndex, List<CodeInstruction> codes, int i)
            {
                return originalKingdomConditionIndex < 0 && i > 2 && i < codes.Count - 2
                       && codes[i].opcode == OpCodes.Callvirt
                       && codes[i].operand.ToString().Contains("get_Kingdom")
                       && codes[i - 1].opcode == OpCodes.Callvirt
                       && codes[i - 1].operand.ToString().Contains("get_Clan")
                       && codes[i - 2].opcode == OpCodes.Call
                       && codes[i - 2].operand.ToString().Contains("get_OriginalOwner")
                       && codes[i - 3].opcode == OpCodes.Ldarg_0
                       && codes[i + 1].opcode == OpCodes.Brfalse_S;
            }
            static void ReplaceCodeInstructions(int startIndex, int endIndex, List<CodeInstruction> codes)
            {
                if (startIndex > -1 && endIndex > -1)
                {
                    CodeInstruction[] codesToInsert = new CodeInstruction[]
                        {
                            new CodeInstruction(opcode: OpCodes.Ldarg_0),
                            new CodeInstruction(opcode: OpCodes.Call, operand: miLeaveOriginalKingdom)
                        };

                    codes.RemoveRange(startIndex, endIndex - startIndex);
                    codes.InsertRange(startIndex, codesToInsert);
                }
                else
                {
                    MessageHelper.ErrorMessage("Harmony transpiler for JoinKingdomAsClanBarterable. Apply could not find code hooks!");
                }
            }

            static void LogNoHooksIssue(int originalKingdomConditionIndex, int replaceStartIndex, int replaceEndIndex, List<CodeInstruction> codes)
            {
                LoggingHelper.Log("Indexes:", "Transpiler for JoinKingdomAsClanBarterable.Apply");
                StringBuilder issueInfo = new StringBuilder("");
                issueInfo.Append($"\toriginalKingdomConditionIndex = {originalKingdomConditionIndex}.\n\treplaceStartIndex={replaceStartIndex}.\n\treplaceEndIndex={replaceEndIndex}.");
                issueInfo.Append($"\nMethodInfos:");
                issueInfo.Append($"\n\tmiApplyByLeaveKingdom={(miApplyByLeaveKingdom != null ? miApplyByLeaveKingdom.ToString() : "not found")}");
                issueInfo.Append($"\nIL:");
                for (int i = 0; i < codes.Count; ++i)
                {
                    issueInfo.Append($"\n\t{i:D4}:\t{codes[i]}");
                }
                // get info about other transpilers on OriginalMethod        
                HarmonyLib.Patches patches;
                patches = Harmony.GetPatchInfo(MethodBase.GetCurrentMethod());
                if (patches != null)
                {
                    issueInfo.Append($"\nOther transpilers:");
                    foreach (Patch patch in patches.Transpilers)
                    {
                        issueInfo.Append(patch.GetDebugString());
                    }
                }
                LoggingHelper.Log(issueInfo.ToString());
            }
        }

        public static bool Prepare()
        {
            return SettingsHelper.SubSystemEnabled(SubSystemType.MigrationTweaks);
        }
    }
}
