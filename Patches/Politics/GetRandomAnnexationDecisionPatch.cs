using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using AllegianceOverhaul.CampaignBehaviors.BehaviorManagers;
using AllegianceOverhaul.Helpers;
using AllegianceOverhaul.PoliticsRebalance;
using AllegianceOverhaul.Extensions;

namespace AllegianceOverhaul.Patches.Politics
{
  [HarmonyPatch(typeof(KingdomDecisionProposalBehavior), "GetRandomAnnexationDecision")]
  public class GetRandomAnnexationDecisionPatch
  {
    private delegate bool ConsiderAnnexDelegate(KingdomDecisionProposalBehavior instance, Clan clan, Kingdom kingdom, Clan targetClan, Town targetSettlement);
    private static readonly ConsiderAnnexDelegate deConsiderAnnex = AccessHelper.GetDelegate<ConsiderAnnexDelegate>(typeof(KingdomDecisionProposalBehavior), "ConsiderAnnex");

    [HarmonyPriority(Priority.VeryHigh)]
    public static bool Prefix(Clan clan, ref KingdomDecision __result, KingdomDecisionProposalBehavior __instance) //Bool prefixes compete with each other and skip others, as well as original, if return false
    {
      try
      {
        bool SubSystemEnabled = SettingsHelper.SubSystemEnabled(SubSystemType.ElectionCooldowns, clan) || SettingsHelper.SubSystemEnabled(SubSystemType.DecisionSupportRebalance, clan);
        bool SystemDebugEnabled = SettingsHelper.SystemDebugEnabled(AOSystems.PoliticsRebalance, DebugType.General, clan);

        if (!SubSystemEnabled && !SystemDebugEnabled)
        {
          return true;
        }

        Kingdom kingdom = clan.Kingdom;
        __result = null;
        if (kingdom.UnresolvedDecisions.FirstOrDefault(x => x is SettlementClaimantPreliminaryDecision) == null && clan.Influence >= 300.0)
        {
          bool possessionsFactorApplied = SettingsHelper.SubSystemEnabled(SubSystemType.AnnexationSupportRebalance, clan)
                                          && Settings.Instance.AnnexSupportCalculationMethod.SelectedValue.EnumValue.HasFlag(FiefOwnershipConsideration.PossessionsFactor)
                                          && Settings.Instance.FiefsDeemedFairBaseline.SelectedValue.EnumValue != NumberOfFiefsCalculationMethod.WithoutRestrictions;
          Clan randomClan = kingdom.Clans.Where(x => x != clan
                                                     && x.Fortifications.Count > 0
                                                     && (x.GetRelationWithClan(clan) < -25 || (possessionsFactorApplied && Campaign.Current.GetAOGameModels().DecisionSupportScoringModel.GetNumberOfFiefsDeemedFair(x) < x.Fortifications.Count))
                                                     && x.Fortifications.FirstOrDefault(f => !(SubSystemEnabled && AOCooldownManager.HasDecisionCooldown(new SettlementClaimantPreliminaryDecision(clan, f.Settlement)))) != null
                                               ).ToArray().GetRandomElement();

          Town randomFortification = SettingsHelper.SubSystemEnabled(SubSystemType.ElectionCooldowns, clan)
              ? clan.Fortifications.Where(f => !(AOCooldownManager.HasDecisionCooldown(new SettlementClaimantPreliminaryDecision(clan, f.Settlement)))).ToArray().GetRandomElement()
              : clan.Fortifications.ToArray().GetRandomElement();

          //ConsiderAnnexDelegate deConsiderAnnex = AccessHelper.GetDelegate<ConsiderAnnexDelegate, KingdomDecisionProposalBehavior>(__instance, "ConsiderAnnex");
          if (randomClan != null && deConsiderAnnex(__instance, clan, kingdom, randomClan, randomFortification))
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
        MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
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
