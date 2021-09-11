using AllegianceOverhaul.CampaignBehaviors.BehaviorManagers;
using AllegianceOverhaul.Helpers;
using AllegianceOverhaul.PoliticsRebalance;

using HarmonyLib;

using System;
using System.Linq;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Localization;


namespace AllegianceOverhaul.Patches.Politics
{

    [HarmonyPatch(typeof(KingdomDecisionProposalBehavior), "GetRandomAnnexationDecision")]
    public static class GetRandomAnnexationDecisionPatch
    {
        private delegate bool ConsiderAnnexDelegate(KingdomDecisionProposalBehavior instance, Clan clan, Kingdom kingdom, Clan targetClan, Town targetSettlement);
        private static readonly ConsiderAnnexDelegate? deConsiderAnnex = AccessHelper.GetDelegate<ConsiderAnnexDelegate>(typeof(KingdomDecisionProposalBehavior), "ConsiderAnnex");

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
                if (!kingdom.UnresolvedDecisions.Any(x => x is SettlementClaimantPreliminaryDecision) && clan.Influence >= 300.0)
                {
                    Clan? randomClan = kingdom.Clans.Where(x => x != clan
                                                               && x.Fiefs.Count > 0
                                                               && (x.GetRelationWithClan(clan) < -25)
                                                               && x.Fiefs.Any(f => !(SubSystemEnabled && AOCooldownManager.HasDecisionCooldown(new SettlementClaimantPreliminaryDecision(clan, f.Settlement))))
                                                         ).ToArray().GetRandomElement();

                    Town? randomFortification = SubSystemEnabled
                        ? randomClan?.Fiefs.Where(f => !(AOCooldownManager.HasDecisionCooldown(new SettlementClaimantPreliminaryDecision(clan, f.Settlement)))).ToArray().GetRandomElement()
                        : randomClan?.Fiefs.ToArray().GetRandomElement();

                    if (randomClan != null && randomFortification != null && deConsiderAnnex!(__instance, clan, kingdom, randomClan, randomFortification))
                        __result = new SettlementClaimantPreliminaryDecision(clan, randomFortification.Settlement);

                    if (SystemDebugEnabled)
                    {
                        PoliticsDebugHelper.PrepareConsiderationDebugMessage(clan, randomFortification, __result, out TextObject debugLogMessage);
                        MessageHelper.SimpleMessage(debugLogMessage);
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                DebugHelper.HandleException(ex, methodInfo, "Harmony patch for KingdomDecisionProposalBehavior. GetRandomAnnexationDecision");
                return true;
            }
        }

        public static bool Prepare()
        {
            return SettingsHelper.SubSystemEnabled(SubSystemType.ElectionRebalance) || SettingsHelper.SystemDebugEnabled(AOSystems.PoliticsRebalance, DebugType.General);
        }
    }
}
