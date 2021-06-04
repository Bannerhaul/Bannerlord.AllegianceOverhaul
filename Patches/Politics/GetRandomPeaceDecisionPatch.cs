using HarmonyLib;

using System;
using System.Linq;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Localization;

using AllegianceOverhaul.CampaignBehaviors.BehaviorManagers;
using AllegianceOverhaul.Helpers;
using AllegianceOverhaul.PoliticsRebalance;

namespace AllegianceOverhaul.Patches.Politics
{
  [HarmonyPatch(typeof(KingdomDecisionProposalBehavior), "GetRandomPeaceDecision")]
  public static class GetRandomPeaceDecisionPatch
  {
    private delegate bool ConsiderPeaceDelegate(KingdomDecisionProposalBehavior instance, Clan clan, Clan otherClan, Kingdom kingdom, IFaction otherFaction, out MakePeaceKingdomDecision decision);
    private static readonly ConsiderPeaceDelegate deConsiderPeace = AccessHelper.GetDelegate<ConsiderPeaceDelegate>(typeof(KingdomDecisionProposalBehavior), "ConsiderPeace");

    [HarmonyPriority(Priority.VeryHigh)]
    public static bool Prefix(Clan clan, ref KingdomDecision __result, KingdomDecisionProposalBehavior __instance) //Bool prefixes compete with each other and skip others, as well as original, if return false
    {
      try
      {
        bool SubSystemEnabled = SettingsHelper.SubSystemEnabled(SubSystemType.ElectionCooldowns, clan);
        bool SystemDebugEnabled = SettingsHelper.SystemDebugEnabled(AOSystems.PoliticsRebalance, DebugType.General, clan);

        if (!SubSystemEnabled && !SystemDebugEnabled)
          return true;

        Kingdom kingdom = clan.Kingdom;
        __result = null;
        if (kingdom.UnresolvedDecisions.FirstOrDefault(x => x is MakePeaceKingdomDecision) == null)
        {
          Kingdom randomElement = Kingdom.All.Where(x => x.IsAtWarWith(kingdom)
                                                         && x != Clan.PlayerClan.Kingdom
                                                         && !(SubSystemEnabled && AOCooldownManager.HasDecisionCooldown(new MakePeaceKingdomDecision(clan, x, applyResults: false)))
                                                   ).ToArray().GetRandomElement();

          //ConsiderPeaceDelegate deConsiderPeace = AccessHelper.GetDelegate<ConsiderPeaceDelegate, KingdomDecisionProposalBehavior>(__instance, "ConsiderPeace");
          if (randomElement != null && deConsiderPeace(__instance, clan, randomElement.RulingClan, kingdom, randomElement, out MakePeaceKingdomDecision decision))
            __result = decision;

          if (SystemDebugEnabled)
          {
            PoliticsDebugHelper.PrepareConsiderationDebugMessage(ConsiderationType.MakingPeace, clan, randomElement, __result, out TextObject debugLogMessage);
            MessageHelper.SimpleMessage(debugLogMessage);
          }
        }
        return false;
      }
      catch (Exception ex)
      {
        MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
        DebugHelper.HandleException(ex, methodInfo, "Harmony patch for KingdomDecisionProposalBehavior. GetRandomPeaceDecision");
        return true;
      }
    }

    public static bool Prepare()
    {
      return SettingsHelper.SubSystemEnabled(SubSystemType.ElectionCooldowns) || SettingsHelper.SystemDebugEnabled(AOSystems.PoliticsRebalance, DebugType.General);
    }
  }
}
