using HarmonyLib;
using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Barterables;

namespace AllegianceOverhaul.Patches
{
  [HarmonyPatch(typeof(JoinKingdomAsClanBarterable), "GetUnitValueForFaction")]
  public class GetUnitValueForFactionJoinKingdomPatch
  {
    public static void Postfix(IFaction factionForEvaluation, int __result, JoinKingdomAsClanBarterable __instance)
    {
      try
      {
        Hero iOriginalOwner = __instance.OriginalOwner;
        Clan iOriginalOwnerClan = iOriginalOwner.Clan;

        if
        (
          !SettingsHelper.InDebugBranch || !Settings.Instance.EnableTechnicalDebugging
          || (!SettingsHelper.FactionInScope(factionForEvaluation, Settings.Instance.EnsuredLoyaltyDebugScope) && !SettingsHelper.FactionInScope((IFaction)iOriginalOwnerClan, Settings.Instance.EnsuredLoyaltyDebugScope))
        )
          return;

        Kingdom iOriginalOwnerKingdom = iOriginalOwnerClan.Kingdom;
        Kingdom iTargetKingdom = __instance.TargetKingdom;
        PartyBase iOriginalParty = __instance.OriginalParty;
        //Hero leader = iOriginalOwner.MapFaction.Leader;

        float CalculatedResult = -1000000f;
        int valueForFaction = 0;
        float settlementValue = 0f;

        float ScoreOfClanToJoinKingdom = Campaign.Current.Models.DiplomacyModel.GetScoreOfClanToJoinKingdom(iOriginalOwnerClan, iTargetKingdom);
        float ScoreOfKingdomToGetClan = Campaign.Current.Models.DiplomacyModel.GetScoreOfKingdomToGetClan(iTargetKingdom, iOriginalOwnerClan);

        if (iTargetKingdom.IsKingdomFaction)
        {
          if (factionForEvaluation == iOriginalOwnerClan)
          {
            CalculatedResult = ScoreOfClanToJoinKingdom;
            if (iOriginalOwnerKingdom != null)
            {
              valueForFaction = new LeaveKingdomAsClanBarterable(iOriginalOwner, iOriginalParty).GetValueForFaction(factionForEvaluation);
              if (!iTargetKingdom.IsAtWarWith((IFaction)iOriginalOwnerKingdom))
              {
                settlementValue = iOriginalOwnerClan.CalculateSettlementValue(iOriginalOwnerKingdom);
                CalculatedResult -= settlementValue;
              }
              CalculatedResult += (float)valueForFaction;
            }
          }
          else if (factionForEvaluation.MapFaction == iTargetKingdom)
            CalculatedResult = ScoreOfKingdomToGetClan;
        }
        string UnitValueDebugInfo = String.Format("JoinKingdom - UnitValueForFaction. factionForEvaluation: {0}." +
          " ScoreOfClanToJoinKingdom = {1}. GetScoreOfKingdomToGetClan = {2}. ValueForFaction = {3}. SettlementValue = {4}. CalculatedResult = {5}. NativeResult = {6}",
          (factionForEvaluation != null) ? factionForEvaluation.Name.ToString() : "is null",
          ScoreOfClanToJoinKingdom.ToString("N"),
          ScoreOfKingdomToGetClan.ToString("N"),
          valueForFaction.ToString("N"), settlementValue.ToString("N"), CalculatedResult.ToString("N"), __result.ToString("N"));

        MessageHelper.TechnicalMessage(UnitValueDebugInfo);
      }
      catch (Exception ex)
      {
        MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
        DebugHelper.HandleException(ex, methodInfo, "Harmony patch for JoinKingdomAsClanBarterable.GetUnitValueForFaction");
      }
    }
    public static bool Prepare()
    {
      return Settings.Instance.EnableTechnicalDebugging;
    }
  }
}