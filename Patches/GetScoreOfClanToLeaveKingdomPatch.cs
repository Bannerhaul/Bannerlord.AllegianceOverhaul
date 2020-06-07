using HarmonyLib;
using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;

namespace AllegianceOverhaul.Patches
{
  [HarmonyPatch(typeof(DefaultDiplomacyModel), "GetScoreOfClanToLeaveKingdom")]
  public class GetScoreOfClanToLeaveKingdomPatch
  {
    public static void Postfix(Clan clan, Kingdom kingdom, float __result)
    {
      try
      {
        if (!SettingsHelper.InDebugBranch || !Settings.Instance.EnableTechnicalDebugging || !SettingsHelper.FactionInScope(clan, Settings.Instance.EnsuredLoyaltyDebugScope))
          return;

        int RelationBetweenClans = FactionManager.GetRelationBetweenClans(kingdom.RulingClan, clan);
        float RelationModifier = (float)Math.Min(2.0, Math.Max(0.5, 1.0 + Math.Sqrt((double)Math.Abs(RelationBetweenClans)) * (RelationBetweenClans < 0 ? -0.0599999986588955 : 0.0399999991059303)));
        float CultureModifier = (float)(1.0 + (kingdom.Culture == clan.Culture ? 0.150000005960464 : -0.150000005960464));
        float NewSettlementValue = clan.CalculateSettlementValue((Kingdom)null);
        float CurSettlementValue = clan.CalculateSettlementValue(kingdom);
        int ClanCmndrHeroeCount = clan.CommanderHeroes.Count;
        float StlmntValPerHeroModifier = 0.0f;
        float ValueOfKingdomSettlements = 0.0f;
        if (!clan.IsMinorFaction)
        {
          foreach (Town fief in kingdom.Fiefs)
            ValueOfKingdomSettlements += fief.Owner.Settlement.GetSettlementValueForFaction((IFaction)kingdom);
          int KingdomCmndrHeroeCount = 0;
          foreach (Clan clan1 in kingdom.Clans)
          {
            if (!clan1.IsMinorFaction || clan1 == Clan.PlayerClan)
              KingdomCmndrHeroeCount += clan1.CommanderHeroes.Count;
          }
          StlmntValPerHeroModifier = ValueOfKingdomSettlements / (float)(KingdomCmndrHeroeCount + ClanCmndrHeroeCount);
        }
        float ClanStrengthModifier = (float)(((double)clan.TotalStrength + 150.0 * (double)ClanCmndrHeroeCount) * 10.0);
        float ReliabilityConstant = Helpers.HeroHelper.CalculateReliabilityConstant(clan.Leader, 1f);
        float DaysWithFactionModifier = 2000f * (float)(10.0 - Math.Sqrt(Math.Min(100.0, (CampaignTime.Now - clan.LastFactionChangeTime).ToDays)));
        int ClanFortificationsModifier = 40000 + (clan.Fortifications != null ? clan.Fortifications.Count : 0) * 20000;
        float ComputedResult =
          (float)(-((double)StlmntValPerHeroModifier * Math.Sqrt((double)ClanCmndrHeroeCount) * 0.300000011920929 + (double)ReliabilityConstant * ((double)ClanFortificationsModifier + (double)ClanStrengthModifier) + (double)DaysWithFactionModifier) *
            ((double)RelationModifier * (double)CultureModifier) + ((double)NewSettlementValue - (double)CurSettlementValue) + (kingdom.Ruler == Hero.MainHero ? -70000.0 : 0.0));

        string UnitValueDebugInfo = String.Format("ScoreOfClanToLeaveKingdom. RelationBetweenClans = {0}. RelationModifier = {1}, CultureModifier = {2}. ClanCmndrHeroeCount = {3}. " +
          "ValueOfKingdomSettlements = {4}. StlmntValPerHeroModifier = {5}. CurSettlementValue = {6}. NewSettlementValue = {7}. ClanStrengthModifier = {8}. ReliabilityConstant = {9}. " +
          "DaysWithFactionModifier = {10}. ClanFortificationsModifier = {11}. CalculatedResult = {12}. NativeResult = {13}.",
          RelationBetweenClans.ToString(), RelationModifier.ToString("N"), CultureModifier.ToString("N"), ClanCmndrHeroeCount.ToString("N"),
          ValueOfKingdomSettlements.ToString("N"), StlmntValPerHeroModifier.ToString("N"), CurSettlementValue.ToString("N"), NewSettlementValue.ToString("N"), ClanStrengthModifier.ToString("N"), ReliabilityConstant.ToString("N"),
          DaysWithFactionModifier.ToString("N"), ClanFortificationsModifier.ToString("N"), ComputedResult.ToString("N"), __result.ToString("N"));

        MessageHelper.TechnicalMessage(UnitValueDebugInfo);
      }
      catch (Exception ex)
      {
        MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
        DebugHelper.HandleException(ex, methodInfo, "Harmony patch for GetScoreOfClanToLeaveKingdom");
      }
    }
    public static bool Prepare()
    {
      return Settings.Instance.EnableTechnicalDebugging;
    }
  }
}