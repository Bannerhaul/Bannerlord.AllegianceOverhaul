using AllegianceOverhaul.CampaignBehaviors.BehaviorManagers;
using AllegianceOverhaul.Helpers;
using AllegianceOverhaul.PoliticsRebalance;

using HarmonyLib;

using System;
using System.Linq;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace AllegianceOverhaul.Patches.Politics
{
    [HarmonyPatch(typeof(KingdomDecisionProposalBehavior), "GetRandomWarDecision")]
    public static class GetRandomWarDecisionPatch
    {
        private delegate bool ConsiderWarDelegate(KingdomDecisionProposalBehavior instance, Clan clan, Kingdom kingdom, IFaction otherFaction);
        private static readonly ConsiderWarDelegate? deConsiderWar = AccessHelper.GetDelegate<ConsiderWarDelegate>(typeof(KingdomDecisionProposalBehavior), "ConsiderWar");

        [HarmonyPriority(Priority.VeryHigh)]
        public static bool Prefix(Clan clan, ref KingdomDecision? __result, KingdomDecisionProposalBehavior __instance) //Bool prefixes compete with each other and skip others, as well as original, if return false
        {
            try
            {
                bool SubSystemEnabled = SettingsHelper.SubSystemEnabled(SubSystemType.ElectionCooldowns, clan);
                bool SystemDebugEnabled = SettingsHelper.SystemDebugEnabled(AOSystems.PoliticsRebalance, DebugType.General, clan);

                if (!SubSystemEnabled && !SystemDebugEnabled)
                {
                    return true;
                }

                Kingdom kingdom = clan.Kingdom;
                __result = null;
                if (kingdom.UnresolvedDecisions.FirstOrDefault(x => x is DeclareWarDecision) == null)
                {
                    Kingdom randomElement = Kingdom.All.Where(x => x != kingdom
                                                                   && !x.IsAtWarWith(kingdom)
                                                                   && x.GetStanceWith(kingdom).PeaceDeclarationDate.ElapsedDaysUntilNow > 20.0
                                                                   && !(SubSystemEnabled && AOCooldownManager.HasDecisionCooldown(new DeclareWarDecision(clan, x)))
                                                             ).ToArray().GetRandomElement();

                    //ConsiderWarDelegate deConsiderWar = AccessHelper.GetDelegate<ConsiderWarDelegate, KingdomDecisionProposalBehavior>(__instance, "ConsiderWar");
                    if (randomElement != null && deConsiderWar!(__instance, clan, kingdom, randomElement))
                        __result = new DeclareWarDecision(clan, randomElement);

                    if (SystemDebugEnabled)
                    {
                        PoliticsDebugHelper.PrepareConsiderationDebugMessage(ConsiderationType.DeclaringWar, clan, randomElement, __result, out TextObject debugLogMessage);
                        MessageHelper.SimpleMessage(debugLogMessage);
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                DebugHelper.HandleException(ex, methodInfo, "Harmony patch for KingdomDecisionProposalBehavior. GetRandomWarDecision");
                return true;
            }
        }

        public static bool Prepare()
        {
            return SettingsHelper.SubSystemEnabled(SubSystemType.ElectionCooldowns) || SettingsHelper.SystemDebugEnabled(AOSystems.PoliticsRebalance, DebugType.General);
        }
    }
}