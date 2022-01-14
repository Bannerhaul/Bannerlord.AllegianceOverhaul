using AllegianceOverhaul.Helpers;

using HarmonyLib;

using System;
using System.Linq;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;

namespace AllegianceOverhaul.Patches.Politics
{
    [HarmonyPatch(typeof(DefaultClanPoliticsModel), "GetInfluenceRequiredToOverrideKingdomDecision")]
    public static class GetInfluenceRequiredToOverrideKingdomDecisionPatch
    {
        private static string GetModifierApplied(bool condition)
        {
            return condition ? "applied" : "not applied";
        }
        private static float GetBaseCompensationCost(ODCostCalculationMethod calculationMethod, DecisionOutcome popularOption, DecisionOutcome overridingOption, KingdomDecision decision)
        {
            float PopularOptionSupportPoints = popularOption.TotalSupportPoints;
            float OverridingOptionSupportPoints = overridingOption.TotalSupportPoints + 3f;
#if e170 || e171
            Clan rulingClan = decision.Kingdom.RulingClan;
            return calculationMethod switch
            {
                ODCostCalculationMethod.FlatInfluenceOverride => popularOption.SupporterList.Sum(sup => decision.GetInfluenceCostOfSupport(rulingClan, sup.SupportWeight)),
                ODCostCalculationMethod.SlightlyFavor => (PopularOptionSupportPoints - OverridingOptionSupportPoints) * decision.GetInfluenceCostOfSupport(rulingClan, Supporter.SupportWeights.SlightlyFavor),
                ODCostCalculationMethod.StronglyFavor => (PopularOptionSupportPoints - OverridingOptionSupportPoints) * decision.GetInfluenceCostOfSupport(rulingClan, Supporter.SupportWeights.StronglyFavor),
                ODCostCalculationMethod.FullyPush => (PopularOptionSupportPoints - OverridingOptionSupportPoints) * decision.GetInfluenceCostOfSupport(rulingClan, Supporter.SupportWeights.FullyPush),
                _ => throw new ArgumentOutOfRangeException(nameof(Settings.Instance.OverrideDecisionCostCalculationMethod.SelectedValue.EnumValue), Settings.Instance!.OverrideDecisionCostCalculationMethod.SelectedValue.EnumValue, null),
            };
#else
            return calculationMethod switch
            {
                ODCostCalculationMethod.FlatInfluenceOverride => popularOption.SupporterList.Sum(sup => decision.GetInfluenceCostOfSupport(sup.SupportWeight)),
                ODCostCalculationMethod.SlightlyFavor => (PopularOptionSupportPoints - OverridingOptionSupportPoints) * decision.GetInfluenceCostOfSupport(Supporter.SupportWeights.SlightlyFavor),
                ODCostCalculationMethod.StronglyFavor => (PopularOptionSupportPoints - OverridingOptionSupportPoints) * decision.GetInfluenceCostOfSupport(Supporter.SupportWeights.StronglyFavor),
                ODCostCalculationMethod.FullyPush => (PopularOptionSupportPoints - OverridingOptionSupportPoints) * decision.GetInfluenceCostOfSupport(Supporter.SupportWeights.FullyPush),
                _ => throw new ArgumentOutOfRangeException(nameof(Settings.Instance.OverrideDecisionCostCalculationMethod.SelectedValue.EnumValue), Settings.Instance!.OverrideDecisionCostCalculationMethod.SelectedValue.EnumValue, null),
            };
#endif
        }
        private static float ApplySupport(ref float popularOptionSupportPoints, ref float overridingOptionSupportPoints, KingdomDecision decision)
        {
#if e170 || e171
            Clan rulingClan = decision.Kingdom.RulingClan;
#endif
            if (popularOptionSupportPoints == overridingOptionSupportPoints + 1.0)
            {
                ++overridingOptionSupportPoints;
#if e170 || e171
                return decision.GetInfluenceCostOfSupport(rulingClan, Supporter.SupportWeights.SlightlyFavor);
#else
                return decision.GetInfluenceCostOfSupport(Supporter.SupportWeights.SlightlyFavor);
#endif
            }
            else if (popularOptionSupportPoints == overridingOptionSupportPoints + 2.0)
            {
                overridingOptionSupportPoints += 2f;
#if e170 || e171
                return decision.GetInfluenceCostOfSupport(rulingClan, Supporter.SupportWeights.StronglyFavor);
#else
                return decision.GetInfluenceCostOfSupport(Supporter.SupportWeights.StronglyFavor);
#endif
            }
            else if (popularOptionSupportPoints > overridingOptionSupportPoints + 2.0)
            {
                overridingOptionSupportPoints += 3f;
#if e170 || e171
                return decision.GetInfluenceCostOfSupport(rulingClan, Supporter.SupportWeights.FullyPush);
#else
                return decision.GetInfluenceCostOfSupport(Supporter.SupportWeights.FullyPush);
#endif
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
                    LackingPointsCompensationCost = GetBaseCompensationCost(Settings.Instance!.OverrideDecisionCostCalculationMethod.SelectedValue.EnumValue, popularOption, overridingOption, decision);
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
                      Settings.Instance!.OverrideDecisionCostCalculationMethod.SelectedValue, LackingPointsCompensationCost,
                      GetModifierApplied(decision.Kingdom.ActivePolicies.Contains(DefaultPolicies.RoyalPrivilege)), GetModifierApplied(decision.Kingdom.RulingClan != Clan.PlayerClan), GetModifierApplied(SettingsHelper.SubSystemEnabled(SubSystemType.FreeDecisionOverriding)),
                      CalculatedResult.ToString("N"), __result.ToString("N"));

                    MessageHelper.TechnicalMessage(InfluenceRequiredDebugInfo);
                }

                if (SubSystemEnabled)
                    __result = (int)CalculatedResult;
            }
            catch (Exception ex)
            {
                MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                DebugHelper.HandleException(ex, methodInfo, "Harmony patch for DefaultClanPoliticsModel. GetInfluenceRequiredToOverrideKingdomDecision");
            }
        }
        public static bool Prepare()
        {
            return SettingsHelper.SubSystemEnabled(SubSystemType.ElectionRebalance) || SettingsHelper.SystemDebugEnabled(AOSystems.PoliticsRebalance, DebugType.Technical) || SettingsHelper.SubSystemEnabled(SubSystemType.FreeDecisionOverriding);
        }
    }
}