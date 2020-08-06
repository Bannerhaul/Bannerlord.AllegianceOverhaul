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

namespace AllegianceOverhaul.Patches.Politics
{
  [HarmonyPatch(typeof(KingdomDecisionProposalBehavior), "GetRandomPolicyDecision")]
  public class GetRandomPolicyDecisionPatch
  {
    private delegate bool ConsiderPolicyDelegate(KingdomDecisionProposalBehavior instance, Clan clan, Kingdom kingdom, PolicyObject policy, bool invert);
    private static readonly ConsiderPolicyDelegate deConsiderPolicy = AccessHelper.GetDelegate<ConsiderPolicyDelegate>(typeof(KingdomDecisionProposalBehavior), "ConsiderPolicy");

    [HarmonyPriority(Priority.VeryHigh)]
    public static bool Prefix(Clan clan, ref KingdomDecision __result, KingdomDecisionProposalBehavior __instance) //Bool prefixes compete with each other and skip others, as well as original, if return false
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
        if (kingdom.UnresolvedDecisions.FirstOrDefault(x => x is KingdomPolicyDecision) == null && clan.Influence >= 200.0)
        {
          PolicyObject randomElement = DefaultPolicies.All.Where(x => !(SubSystemEnabled && AOCooldownManager.HasDecisionCooldown(new KingdomPolicyDecision(clan, x, kingdom.ActivePolicies.Contains(x))))
                                                                ).ToArray().GetRandomElement();
          bool revertPolicy = kingdom.ActivePolicies.Contains(randomElement);

          //ConsiderPolicyDelegate deConsiderPolicy = AccessHelper.GetDelegate<ConsiderPolicyDelegate, KingdomDecisionProposalBehavior>(__instance, "ConsiderPolicy");
          if (randomElement != null && deConsiderPolicy(__instance, clan, kingdom, randomElement, revertPolicy))
            __result = new KingdomPolicyDecision(clan, randomElement, revertPolicy);

          if (SystemDebugEnabled)
          {
            PoliticsDebugHelper.PrepareConsiderationDebugMessage(clan, randomElement, __result, out TextObject debugLogMessage);
            MessageHelper.SimpleMessage(debugLogMessage);
          }
        }

        return false;
      }
      catch (Exception ex)
      {
        MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
        DebugHelper.HandleException(ex, methodInfo, "Harmony patch for KingdomDecisionProposalBehavior. GetRandomPolicyDecision");
        return true;
      }
    }

    public static bool Prepare()
    {
      return SettingsHelper.SubSystemEnabled(SubSystemType.ElectionCooldowns) || SettingsHelper.SystemDebugEnabled(AOSystems.PoliticsRebalance, DebugType.General);
    }
  }
}
