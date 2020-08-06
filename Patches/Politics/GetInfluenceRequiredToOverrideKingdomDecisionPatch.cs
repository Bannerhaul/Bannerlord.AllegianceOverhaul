using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using AllegianceOverhaul.Helpers;

namespace AllegianceOverhaul.Patches.Politics
{
  [HarmonyPatch(typeof(DefaultClanPoliticsModel), "GetInfluenceRequiredToOverrideKingdomDecision")]
  public class GetInfluenceRequiredToOverrideKingdomDecisionPatch
  {
    private static string GetModifierApplied(bool condition)
    {
      return condition ? "applied" : "not applied";
    }
    private static float GetBaseCompensationCost (ODCostCalculationMethod calculationMethod, DecisionOutcome popularOption, DecisionOutcome overridingOption, KingdomDecision decision)
    {
      float PopularOptionSupportPoints = popularOption.TotalSupportPoints;
      float OverridingOptionSupportPoints = overridingOption.TotalSupportPoints + 3f;
      switch (calculationMethod)
      {
        case ODCostCalculationMethod.FlatInfluenceOverride:
          return popularOption.SupporterList.Sum(sup => decision.GetInfluenceCostOfSupport(sup.SupportWeight));
        case ODCostCalculationMethod.SlightlyFavor:
          return (PopularOptionSupportPoints - OverridingOptionSupportPoints) * decision.GetInfluenceCostOfSupport(Supporter.SupportWeights.SlightlyFavor);
        case ODCostCalculationMethod.StronglyFavor:
          return (PopularOptionSupportPoints - OverridingOptionSupportPoints) * decision.GetInfluenceCostOfSupport(Supporter.SupportWeights.StronglyFavor);
        case ODCostCalculationMethod.FullyPush:
          return (PopularOptionSupportPoints - OverridingOptionSupportPoints) * decision.GetInfluenceCostOfSupport(Supporter.SupportWeights.FullyPush);
        default:
          throw new ArgumentOutOfRangeException(nameof(Settings.Instance.OverrideDecisionCostCalculationMethod.SelectedValue.EnumValue), Settings.Instance.OverrideDecisionCostCalculationMethod.SelectedValue.EnumValue, null);
      }
    }
    private static float ApplySupport(ref float popularOptionSupportPoints, ref float overridingOptionSupportPoints, KingdomDecision decision)
    {
      if (popularOptionSupportPoints == overridingOptionSupportPoints + 1.0)
      {
        ++overridingOptionSupportPoints;
        return decision.GetInfluenceCostOfSupport(Supporter.SupportWeights.SlightlyFavor);
      }
      else if (popularOptionSupportPoints == overridingOptionSupportPoints + 2.0)
      {
        overridingOptionSupportPoints += 2f;
        return decision.GetInfluenceCostOfSupport(Supporter.SupportWeights.StronglyFavor);
      }
      else if (popularOptionSupportPoints > overridingOptionSupportPoints + 2.0)
      {
        overridingOptionSupportPoints += 3f;
        return decision.GetInfluenceCostOfSupport(Supporter.SupportWeights.FullyPush);
      }
      else return 0.0f;
    }

    public static void Postfix(DecisionOutcome popularOption, DecisionOutcome overridingOption, KingdomDecision decision, ref int __result)
    {
      try
      {
        bool SubSystemEnabled = SettingsHelper.SubSystemEnabled(SubSystemType.ElectionRebalance, decision.Kingdom);
        bool SystemDebugEnabled = SettingsHelper.SystemDebugEnabled(AOSystems.PoliticsRebalance, DebugType.Technical, decision.Kingdom);
        if (!SubSystemEnabled && !SystemDebugEnabled)
        {
          if (SettingsHelper.SubSystemEnabled(SubSystemType.FreeDecisionOverriding))
          {
            __result = 0;
          }
          return;
        }

        float PopularOptionSupportPoints = popularOption.TotalSupportPoints;
        float OverridingOptionSupportPoints = overridingOption.TotalSupportPoints;
        float CalculatedResult = 0.0f;
        if (decision.Kingdom.RulingClan == Clan.PlayerClan)
          CalculatedResult += ApplySupport(ref PopularOptionSupportPoints, ref OverridingOptionSupportPoints, decision);
        float LackingPointsCompensationCost = 0;
        if (PopularOptionSupportPoints > OverridingOptionSupportPoints)
        {
          LackingPointsCompensationCost = GetBaseCompensationCost(Settings.Instance.OverrideDecisionCostCalculationMethod.SelectedValue.EnumValue, popularOption, overridingOption, decision);
          if (decision.Kingdom.ActivePolicies.Contains(DefaultPolicies.RoyalPrivilege))
            LackingPointsCompensationCost *= 0.8f;
          if (decision.Kingdom.RulingClan != Clan.PlayerClan)
            LackingPointsCompensationCost *= 0.8f;
          CalculatedResult += LackingPointsCompensationCost;
        }
        if (SettingsHelper.SubSystemEnabled(SubSystemType.FreeDecisionOverriding))
        {
          CalculatedResult = 0f;
        }

        if (SystemDebugEnabled)
        {
          string InfluenceRequiredDebugInfo = string.Format("DefaultClanPoliticsModel - InfluenceRequiredToOverrideKingdomDecision. Kingdom: {0}. Decision: {1}. PopularOption: {2}. OverridingOption: {3}. " +
            "PopularOptionSupportPoints = {4}. OverridingOptionSupportPoints = {5}. " +
            "OverrideDecisionCostCalculationMethod: {6}. LackingPointsCompensationCost = {7}. " +
            "Royal Privilege modifier: {8}. NPC modifier: {9}. Free of charge cheat modifier: {10} " +
            "CalculatedResult = {11}. NativeResult = {12}",
            decision.Kingdom.Name, decision.GetGeneralTitle(), popularOption.GetDecisionTitle(), overridingOption.GetDecisionTitle(),
            PopularOptionSupportPoints, OverridingOptionSupportPoints,
            Settings.Instance.OverrideDecisionCostCalculationMethod.SelectedValue, LackingPointsCompensationCost, 
            GetModifierApplied(decision.Kingdom.ActivePolicies.Contains(DefaultPolicies.RoyalPrivilege)), GetModifierApplied(decision.Kingdom.RulingClan != Clan.PlayerClan), GetModifierApplied(SettingsHelper.SubSystemEnabled(SubSystemType.FreeDecisionOverriding)),
            CalculatedResult.ToString("N"), __result.ToString("N")); ;

          MessageHelper.TechnicalMessage(InfluenceRequiredDebugInfo);
        }

        if (SubSystemEnabled)
          __result = (int)CalculatedResult;
      }
      catch (Exception ex)
      {
        MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
        DebugHelper.HandleException(ex, methodInfo, "Harmony patch for DefaultClanPoliticsModel. GetInfluenceRequiredToOverrideKingdomDecision");
      }
    }
    public static bool Prepare()
    {
      return SettingsHelper.SubSystemEnabled(SubSystemType.ElectionRebalance) || SettingsHelper.SystemDebugEnabled(AOSystems.PoliticsRebalance, DebugType.Technical) || SettingsHelper.SubSystemEnabled(SubSystemType.FreeDecisionOverriding);
    }
  }
}