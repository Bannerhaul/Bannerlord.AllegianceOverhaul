extern alias TWCS;

using AllegianceOverhaul.Helpers;
using AllegianceOverhaul.LoyaltyRebalance;

using HarmonyLib;

using System;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;

namespace AllegianceOverhaul.Patches.Loyalty
{
  [HarmonyPatch(typeof(DefaultDiplomacyModel), "GetScoreOfClanToLeaveKingdom")]
  public class GetScoreOfClanToLeaveKingdomPatch
  {
    public static void Postfix(Clan clan, Kingdom kingdom, ref float __result)
    {
      try
      {
        if (!LoyaltyDebugHelper.InDebugBranch || !SettingsHelper.SystemDebugEnabled(AOSystems.EnsuredLoyalty, DebugType.Technical, clan))
        {
          if (SettingsHelper.SubSystemEnabled(SubSystemType.DestabilizeLeaveEvaluation))
          {
            __result += Settings.Instance!.LeaveScoreFlatModifier * 1000000f;
          }
          return;
        }

        int RelationBetweenClans = FactionManager.GetRelationBetweenClans(kingdom.RulingClan, clan);
        float RelationModifier = (float)Math.Min(2.0, Math.Max(0.5, 1.0 + Math.Sqrt(Math.Abs(RelationBetweenClans)) * (RelationBetweenClans < 0 ? -0.0599999986588955 : 0.0399999991059303)));
        float CultureModifier = (float)(1.0 + (kingdom.Culture == clan.Culture ? 0.150000005960464 : -0.150000005960464));
        float NewSettlementValue = clan.CalculateSettlementValue(null);
        float CurSettlementValue = clan.CalculateSettlementValue(kingdom);
        int ClanCmndrHeroeCount = clan.CommanderLimit;
        float StlmntValPerHeroModifier = 0.0f;
        float ValueOfKingdomSettlements = 0.0f;
        if (!clan.IsMinorFaction)
        {
          foreach (Town fief in kingdom.Fiefs)
          {
            ValueOfKingdomSettlements += fief.Owner.Settlement.GetSettlementValueForFaction(kingdom);
          }
          int KingdomCmndrHeroeCount = 0;
          foreach (Clan clan1 in kingdom.Clans)
          {
            if (!clan1.IsMinorFaction || clan1 == Clan.PlayerClan)
              KingdomCmndrHeroeCount += clan1.CommanderLimit;
          }
          StlmntValPerHeroModifier = ValueOfKingdomSettlements / (KingdomCmndrHeroeCount + ClanCmndrHeroeCount);
        }
        float ClanStrengthModifier = (float)((clan.TotalStrength + 150.0 * ClanCmndrHeroeCount) * 10.0);
        float ReliabilityConstant = TWCS::Helpers.HeroHelper.CalculateReliabilityConstant(clan.Leader, 1f);
        float DaysWithFactionModifier = 2000f * (float)(10.0 - Math.Sqrt(Math.Min(100.0, (CampaignTime.Now - clan.LastFactionChangeTime).ToDays)));
        int ClanFortificationsModifier = 40000 + (clan.Fiefs != null ? clan.Fiefs.Count : 0) * 20000;
        float ComputedResult =
          (float)(-(StlmntValPerHeroModifier * Math.Sqrt(ClanCmndrHeroeCount) * 0.300000011920929 + ReliabilityConstant * (ClanFortificationsModifier + (double)ClanStrengthModifier) + DaysWithFactionModifier) *
            (RelationModifier * (double)CultureModifier) + (NewSettlementValue - (double)CurSettlementValue) + (kingdom.Ruler == Hero.MainHero ? -70000.0 : 0.0));

        if (SettingsHelper.SubSystemEnabled(SubSystemType.DestabilizeLeaveEvaluation))
        {
          __result += Settings.Instance!.LeaveScoreFlatModifier * 1000000f;
        }

        string UnitValueDebugInfo = string.Format("ScoreOfClanToLeaveKingdom. RelationBetweenClans = {0}. RelationModifier = {1}, CultureModifier = {2}. ClanCmndrHeroeCount = {3}. " +
          "ValueOfKingdomSettlements = {4}. StlmntValPerHeroModifier = {5}. CurSettlementValue = {6}. NewSettlementValue = {7}. ClanStrengthModifier = {8}. ReliabilityConstant = {9}. " +
          "DaysWithFactionModifier = {10}. ClanFortificationsModifier = {11}. CalculatedResult = {12}. NativeResult = {13}.",
          RelationBetweenClans.ToString(), RelationModifier.ToString("N"), CultureModifier.ToString("N"), ClanCmndrHeroeCount.ToString("N"),
          ValueOfKingdomSettlements.ToString("N"), StlmntValPerHeroModifier.ToString("N"), CurSettlementValue.ToString("N"), NewSettlementValue.ToString("N"), ClanStrengthModifier.ToString("N"), ReliabilityConstant.ToString("N"),
          DaysWithFactionModifier.ToString("N"), ClanFortificationsModifier.ToString("N"), ComputedResult.ToString("N"), __result.ToString("N"));

        MessageHelper.TechnicalMessage(UnitValueDebugInfo);
      }
      catch (Exception ex)
      {
        MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
        DebugHelper.HandleException(ex, methodInfo, "Harmony patch for GetScoreOfClanToLeaveKingdom");
      }
    }
    public static bool Prepare()
    {
      return SettingsHelper.SystemDebugEnabled(AOSystems.EnsuredLoyalty, DebugType.Technical) || SettingsHelper.SubSystemEnabled(SubSystemType.DestabilizeLeaveEvaluation);
    }
  }
}