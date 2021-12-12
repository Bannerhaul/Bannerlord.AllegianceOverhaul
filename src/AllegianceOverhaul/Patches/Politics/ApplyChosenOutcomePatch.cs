using AllegianceOverhaul.Helpers;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;

namespace AllegianceOverhaul.Patches.Politics
{
    [HarmonyPatch(typeof(KingdomElection), "ApplyChosenOutcome")]
    public static class ApplyChosenOutcomePatch
    {
        private static readonly MethodInfo miApplySecondaryEffects = AccessTools.Method(typeof(KingdomElection), "ApplySecondaryEffects");
        private static readonly MethodInfo miRemoveDecision = AccessTools.Method(typeof(Kingdom), "RemoveDecision");
        private static readonly MethodInfo miApplyRelationChange = AccessTools.Method(typeof(ApplyChosenOutcomePatch), "ApplyRelationChange");

        public static void ApplyRelationChange(KingdomElection kingdomElection)
        {
            //MessageHelper.SimpleMessage("ApplyRelationChange is called!");
            try
            {
                foreach (DecisionOutcome decisionOutcome in kingdomElection.PossibleOutcomes)
                {
                    //AOEvents.Instance!.OnRelationShift(decisionOutcome.SponsorClan?.Leader ?? Hero.MainHero, Hero.MainHero.Spouse, new SavableClasses.SegmentalFractionalScore(decisionOutcome.TotalSupportPoints, 0));
                }
            }
            catch (Exception ex)
            {
                MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                DebugHelper.HandleException(ex, methodInfo, "Harmony patch for KingdomElection. ApplyChosenOutcome");
            }
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                int startIndex = -1;
                int endIndex = -1;
                List<CodeInstruction> codes = new(instructions);
                for (int i = 0; i < codes.Count; ++i)
                {
                    if (codes[i].Calls(miApplySecondaryEffects))
                    {
                        startIndex = i + 1;
                    }
                    if (startIndex > 0 && i > 5 && codes[i].Calls(miRemoveDecision))
                    {
                        endIndex = i - 5;
                        break;
                    }
                }
                if (startIndex > -1 && endIndex > -1)
                {
                    codes.RemoveRange(startIndex, endIndex - startIndex);
                    codes.InsertRange(startIndex, new CodeInstruction[] { new CodeInstruction(opcode: OpCodes.Ldarg_0), new CodeInstruction(opcode: OpCodes.Call, operand: miApplyRelationChange) });
                }
                return codes.AsEnumerable();
            }
            catch (Exception ex)
            {
                MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                DebugHelper.HandleException(ex, methodInfo, "Harmony transpiler for KingdomElection. ApplyChosenOutcome");
                return instructions;
            }
        }

        public static bool Prepare()
        {
            return false; //Its a blank for the Relation overhaul
        }
    }
}
