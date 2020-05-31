﻿using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace AllegianceOverhaul.Patches
{
  [HarmonyPatch(typeof(DefaultDiplomacyModel), "GetScoreOfClanToJoinKingdom")]
  public class GetScoreOfClanToJoinKingdomPatch
  {
    public static void Postfix(Clan clan, Kingdom kingdom, float __result)
    {
      if (clan.Kingdom != null && clan.Kingdom.RulingClan == clan)
        return;
      if (!SettingsHelper.FactionInScope(clan, Settings.Instance.EnsuredLoyaltyDebugScope))
        return;

      //float supportForPolicyInClan = Campaign.Current.Models.ClanPoliticsModel.CalculateSupportForPolicyInClan(clan, this.Policy);
      // supportForPolicyInClan > 0.0 ? wanted : not wanted
      int RelationBetweenClans = FactionManager.GetRelationBetweenClans(kingdom.RulingClan, clan);
      float RelationModifier = (float)Math.Min(2.0, Math.Max(0.05, 1.0 + Math.Sqrt((double)Math.Abs(RelationBetweenClans)) * 0.1 * Math.Sign(RelationBetweenClans)));
      float CultureModifier = (float)(1.0 + (kingdom.Culture == clan.Culture ? 0.150000005960464 : -0.150000005960464));
      float CurSettlementValue = clan.CalculateSettlementValue((Kingdom)null);
      float NewSettlementValue = clan.CalculateSettlementValue(kingdom);
      int ClanCmndrHeroeCount = clan.CommanderHeroes.Count;
      float StlmntValPerHeroModifier = 0.0f;
      float KingdomCmndrHeroeCntModifier = 0.0f;
      float ValueOfTargetKingdomSettlements = 0.0f;
      if (!clan.IsMinorFaction)
      {
        foreach (Settlement settlement in Settlement.All)
        {
          if (settlement.IsFortification && settlement.MapFaction == kingdom)
            ValueOfTargetKingdomSettlements += settlement.GetSettlementValueForFaction((IFaction)kingdom);
        }
        int TargetKingdomCmndrHeroeCount = 0;
        foreach (Clan clan1 in kingdom.Clans)
        {
          if (!clan1.IsMinorFaction || clan1 == Clan.PlayerClan)
            TargetKingdomCmndrHeroeCount += clan1.CommanderHeroes.Count;
        }
        StlmntValPerHeroModifier = ValueOfTargetKingdomSettlements / (float)(TargetKingdomCmndrHeroeCount + ClanCmndrHeroeCount);
        KingdomCmndrHeroeCntModifier = (float)-((double)(TargetKingdomCmndrHeroeCount * TargetKingdomCmndrHeroeCount) * 50.0);
      }
      float ComputedResult = (float)((double)StlmntValPerHeroModifier * Math.Sqrt((double)ClanCmndrHeroeCount) * 0.300000011920929 * ((double)RelationModifier * (double)CultureModifier) + ((double)NewSettlementValue - (double)CurSettlementValue)) + KingdomCmndrHeroeCntModifier;

      string UnitValueDebugInfo = String.Format("ScoreOfClanToJoinKingdom. RelationBetweenClans = {0}. RelationModifier = {1}, CultureModifier = {2}. ClanCmndrHeroeCount = {3}. " +
        "ValueOfTargetKingdomSettlements = {4}. StlmntValPerHeroModifier = {5}. KingdomCmndrHeroeCntModifier = {6}. CurSettlementValue = {7}. NewSettlementValue = {8}. CalculatedResult = {9}. NativeResult = {10}.",
        RelationBetweenClans.ToString(), RelationModifier.ToString("N"), CultureModifier.ToString("N"), ClanCmndrHeroeCount.ToString("N"),
        ValueOfTargetKingdomSettlements.ToString("N"), StlmntValPerHeroModifier.ToString("N"), KingdomCmndrHeroeCntModifier.ToString("N"),
        CurSettlementValue.ToString("N"), NewSettlementValue.ToString("N"), ComputedResult.ToString("N"), __result.ToString("N"));

      if (SettingsHelper.InDebugBranch)
        InformationManager.DisplayMessage(new InformationMessage(UnitValueDebugInfo, Colors.Magenta));
    }
  }
}