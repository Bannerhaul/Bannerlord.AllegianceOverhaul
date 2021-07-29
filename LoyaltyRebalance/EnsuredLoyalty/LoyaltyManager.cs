using AllegianceOverhaul.Extensions;
using AllegianceOverhaul.Helpers;

using System;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

using static AllegianceOverhaul.Helpers.LocalizationHelper;

namespace AllegianceOverhaul.LoyaltyRebalance.EnsuredLoyalty
{
  public static class LoyaltyManager
  {
    private const string TooltipOathLoyal = "{=BbMvJCYUm}Under {?HERO_CLAN.UNDER_CONTRACT}mercenary service{?}oath of fealty{\\?} at least for {REMAINING_DAYS} {?REMAINING_DAYS.PLURAL_FORM}days{?}day{\\?}";
    private const string TooltipLoyal = "{=oYpHpgDPr}Loyal";
    private const string TooltipRatherLoyal = "{=1TGDgTZq0}Rather loyal";
    private const string TooltipSomewhatLoyal = "{=7XAyqN3Ch}Somewhat loyal";
    private const string TooltipNotLoyal = "{=IoE7Wy6Xl}Not loyal";

    internal const string TransitionFromSame = "{=sppJOtpCT}Furthermore,";
    internal const string TransitionFromDifferent = "{=HXSXF5RyA}But";

    private const string ResultTrue = "{=Ov9Rb3lOw}loyalty will be";
    private const string ResultFalse = "{=7hG7FBx9B}loyalty won't be";
    private const string ResultDepends = "{=2RYEI726V}loyalty might be";

    private const string RelationLow = "{=qeyOXdC7w}too low";
    private const string RelationHigh = "{=Yx08KpCot}high enough";

    private const string LeaderHasResources = "{=saCtWp5d3} and {LEAVING_CLAN_KINGDOM_LEADER.NAME} possesses resourceses to withhold the clan";
    private const string LeaderHasNoResources = "{=ORDtiWoMr} but {LEAVING_CLAN_KINGDOM_LEADER.NAME} does not possess resources to withhold the clan";

    private const string ReasonIsNotEnabled = "{=7LsNUEwPJ}it is not enabled";
    private const string ReasonOutOfScope = "{=uTQ8JIJNf}faction is out of scope";
    private const string ReasonRelationEnabled = "{=xnwSHHbB2}clan leader's relationship with {LEAVING_CLAN_KINGDOM_LEADER.NAME} is {CHECK_RESULT} ({CURRENT_RELATION} out of required {REQUIRED_RELATION}){WITHHOLD_PRICE_INFO}";
    private const string ReasonRelationDisabled = "{=uyOFtcDGq}clan leader's relationship with {LEAVING_CLAN_KINGDOM_LEADER.NAME} does not affect it and clan fulfilled minimal obligations";
    private const string ReasonServicePeriod = "{=hn7Jf5Z2c}clan is under {?LEAVING_CLAN.UNDER_CONTRACT}mercenary service{?}oath of fealty{\\?} for {DAYS_UNDER_SERVICE} {?DAYS_UNDER_SERVICE.PLURAL_FORM}days{?}day{\\?} out of required {REQUIRED_DAYS_UNDER_SERVICE}";

    private const string Debug_EnsuredLoyalty = "{=Ylieuk9mQ} {TRANSITION_PART} {LOYALTY_CHECK_RESULT} ensured, as {REASON}.";

    private static int GetKingdomFortificationsCount(Kingdom kingdom)
    {
      int Count = 0;
      foreach (Clan clan in kingdom.Clans)
      {
        Count += clan.Fiefs?.Count ?? 0;
      }
      return Count;
    }

    private static int GetHonorModifier(Hero leader, bool Defecting = false)
    {
      int HonorLevel = leader.GetTraitLevel(DefaultTraits.Honor);
      return HonorLevel < 0
          ? -HonorLevel * (Defecting ? Settings.Instance!.NegativeHonorEnsuredLoyaltyModifier_Defecting : Settings.Instance!.NegativeHonorEnsuredLoyaltyModifier_Leaving)
          : -HonorLevel * (Defecting ? Settings.Instance!.PositiveHonorEnsuredLoyaltyModifier_Defecting : Settings.Instance!.PositiveHonorEnsuredLoyaltyModifier_Leaving);
    }

    private static bool SetDebugResult(ELState state, TextObject DebugTextObject, Clan? clan = null, Kingdom? kingdom = null, int DaysWithKingdom = 0, int RequiredDays = 0)
    {
      switch (state)
      {
        case ELState.SystemDisabled:
          DebugTextObject.SetTextVariable("LOYALTY_CHECK_RESULT", ResultFalse);
          DebugTextObject.SetTextVariable("REASON", ReasonIsNotEnabled);
          return false;
        case ELState.FactionOutOfScope:
          DebugTextObject.SetTextVariable("LOYALTY_CHECK_RESULT", ResultFalse);
          DebugTextObject.SetTextVariable("REASON", ReasonOutOfScope);
          return false;
        case ELState.UnderRequiredService:
          TextObject ReasonPeriod = new TextObject(ReasonServicePeriod);
          SetNumericVariable(ReasonPeriod, "DAYS_UNDER_SERVICE", DaysWithKingdom);
          SetNumericVariable(ReasonPeriod, "REQUIRED_DAYS_UNDER_SERVICE", RequiredDays);
          DebugTextObject.SetTextVariable("LOYALTY_CHECK_RESULT", ResultTrue);
          DebugTextObject.SetTextVariable("REASON", ReasonPeriod.ToString());
          return true;
        case ELState.UnaffectedByRelations:
          DebugTextObject.SetTextVariable("LOYALTY_CHECK_RESULT", ResultFalse);
          TextObject ReasonDisabled = new TextObject(ReasonRelationDisabled);
          SetEntityProperties(ReasonDisabled, "LEAVING_CLAN", clan, true);
          DebugTextObject.SetTextVariable("REASON", ReasonDisabled);
          return false;
        case ELState.AffectedByRelations:
          int CurrentRelation = clan!.Leader.GetRelation(clan.Kingdom.Ruler);
          int RequiredRelation = GetRelationThreshold(clan, kingdom);
          bool RelationCheckResult = CurrentRelation >= RequiredRelation;
          LoyaltyCostManager costManager = new LoyaltyCostManager(clan, kingdom);
          bool HaveResources = clan.Kingdom.RulingClan.Influence > (costManager.WithholdCost?.InfluenceCost ?? 0) && clan.Kingdom.Ruler.Gold > (costManager.WithholdCost?.GoldCost ?? 0);
          bool ShouldPay = Settings.Instance!.UseWithholdPrice && Settings.Instance.WithholdToleranceLimit * 1000000 < costManager.BarterableSum;
          TextObject WithholdPrice = new TextObject(HaveResources ? LeaderHasResources : LeaderHasNoResources);
          SetEntityProperties(WithholdPrice, "LEAVING_CLAN", clan, true);
          TextObject ReasonRelation = new TextObject(ReasonRelationEnabled);
          ReasonRelation.SetTextVariable("CHECK_RESULT", RelationCheckResult ? RelationHigh : RelationLow);
          SetEntityProperties(ReasonRelation, "LEAVING_CLAN", clan, true);
          SetNumericVariable(ReasonRelation, "CURRENT_RELATION", CurrentRelation);
          SetNumericVariable(ReasonRelation, "REQUIRED_RELATION", RequiredRelation);
          ReasonRelation.SetTextVariable("WITHHOLD_PRICE_INFO", RelationCheckResult && ShouldPay ? WithholdPrice : TextObject.Empty);
          DebugTextObject.SetTextVariable("LOYALTY_CHECK_RESULT", RelationCheckResult ? (ShouldPay ? (HaveResources ? ResultDepends : ResultFalse) : ResultTrue) : ResultFalse);
          DebugTextObject.SetTextVariable("REASON", ReasonRelation.ToString());
          return RelationCheckResult && (!ShouldPay || HaveResources);
        default:
          return false;
      }
    }

    public static int GetRelationThreshold(Clan clan, Kingdom? kingdom = null)
    {
      int RelationThreshold = Settings.Instance!.EnsuredLoyaltyBaseline;

      if (Settings.Instance.UseContextForEnsuredLoyalty)
      {
        RelationThreshold -= RelativesHelper.BloodRelatives(clan.Kingdom.RulingClan, clan) ? Settings.Instance.BloodRelativesEnsuredLoyaltyModifier : 0;
        RelationThreshold +=
          kingdom != null && RelativesHelper.BloodRelatives(kingdom.RulingClan, clan) ? Settings.Instance.BloodRelativesEnsuredLoyaltyModifier : 0 +
          (clan.IsMinorFaction ? Settings.Instance.MinorFactionEnsuredLoyaltyModifier : 0) +
          (kingdom is null ? Settings.Instance.DefectionEnsuredLoyaltyModifier : 0) +
          (clan.Fiefs?.Count < 1 ? Settings.Instance.LandlessClanEnsuredLoyaltyModifier : 0) +
          (GetKingdomFortificationsCount(clan.Kingdom) < 1 ? Settings.Instance.LandlessKingdomEnsuredLoyaltyModifier : 0);
      }

      if (Settings.Instance.UseHonorForEnsuredLoyalty)
        RelationThreshold += GetHonorModifier(clan.Leader, kingdom is null);

      return RelationThreshold;
    }

    public static bool CheckLoyalty(Clan clan, out TextObject DebugTextObject, Kingdom? kingdom = null)
    {
      DebugTextObject = new TextObject(Debug_EnsuredLoyalty);
      if (!Settings.Instance!.UseEnsuredLoyalty)
      {
        return SetDebugResult(ELState.SystemDisabled, DebugTextObject);
      }
      else
      {
        if (!SettingsHelper.FactionInScope(clan, Settings.Instance.EnsuredLoyaltyScope))
        {
          return SetDebugResult(ELState.FactionOutOfScope, DebugTextObject);
        }
        else
        {
          int DaysWithKingdom = (int)(CampaignTime.Now - clan.LastFactionChangeTime).ToDays;
          int RequiredDays = clan.IsUnderMercenaryService ? Settings.Instance.MinorFactionServicePeriod : (clan.IsMinorFaction ? Settings.Instance.MinorFactionOathPeriod : Settings.Instance.FactionOathPeriod);
          if (clan.Kingdom != null && DaysWithKingdom <= RequiredDays)
          {
            return SetDebugResult(ELState.UnderRequiredService, DebugTextObject, DaysWithKingdom: DaysWithKingdom, RequiredDays: RequiredDays);
          }
          else if (Settings.Instance.UseRelationForEnsuredLoyalty && !clan.IsUnderMercenaryService)
          {
            return SetDebugResult(ELState.AffectedByRelations, DebugTextObject, clan, kingdom);
          }
          else
          {
            return SetDebugResult(ELState.UnaffectedByRelations, DebugTextObject, clan);
          }
        }
      }
    }

    public static bool CheckLoyalty(Clan clan, Kingdom? kingdom = null)
    {
      if (!SettingsHelper.SubSystemEnabled(SubSystemType.EnsuredLoyalty, clan))
        return false;

      int DaysWithKingdom = (int)(CampaignTime.Now - clan.LastFactionChangeTime).ToDays;
      if
        (
          (clan.IsUnderMercenaryService && DaysWithKingdom <= Settings.Instance!.MinorFactionServicePeriod) ||
          (!clan.IsUnderMercenaryService && clan.Kingdom != null && DaysWithKingdom <= (clan.IsMinorFaction ? Settings.Instance!.MinorFactionOathPeriod : Settings.Instance!.FactionOathPeriod))
        )
        return true;

      if (Settings.Instance!.UseRelationForEnsuredLoyalty && !clan.IsUnderMercenaryService)
      {
        if (!(clan.Leader.GetRelation(clan.Kingdom?.Ruler) >= GetRelationThreshold(clan, kingdom)))
          return false;
        else
        if (Settings.Instance.UseWithholdPrice)
        {
          LoyaltyCostManager costManager = new LoyaltyCostManager(clan, kingdom);
          if (clan.Kingdom?.RulingClan == Clan.PlayerClan)
          {
            if (costManager.WithholdCost != null)
            {
              if (!(clan.Kingdom?.RulingClan.Influence > costManager.WithholdCost.InfluenceCost) || !(clan.Kingdom?.Ruler.Gold > costManager.WithholdCost.GoldCost))
                return false;
              costManager.AwaitPlayerDecision();
            }
            return true;
          }
          else
            return costManager.GetAIWithholdDecision();
        }
        else
          return true;
      }
      else
        return false;
    }

    public static void GetLoyaltyTooltipInfo(Clan clan, out string text, out Color color)
    {
      if (clan.Kingdom is null)
      {
        GetBlankLoyaltyTooltip(out text, out color);
        return;
      }

      int DaysWithKingdom = (int)(CampaignTime.Now - clan.LastFactionChangeTime).ToDays;
      int RequiredDays = clan.IsUnderMercenaryService ? Settings.Instance!.MinorFactionServicePeriod : (clan.IsMinorFaction ? Settings.Instance!.MinorFactionOathPeriod : Settings.Instance!.FactionOathPeriod);
      if (clan.Kingdom != null && DaysWithKingdom <= RequiredDays)
      {
        TextObject ReasonPeriod = new TextObject(TooltipOathLoyal);
        SetEntityProperties(ReasonPeriod, "HERO_CLAN", clan);
        SetNumericVariable(ReasonPeriod, "REMAINING_DAYS", RequiredDays - DaysWithKingdom);
        text = ReasonPeriod.ToString();
        color = Colors.Green;
        return;
      }
      if (!Settings.Instance.UseRelationForEnsuredLoyalty)
      {
        GetBlankLoyaltyTooltip(out text, out color);
        return;
      }
      GetAveragedLoyaltyStatus(clan, out text, out color);
    }

    private static void GetBlankLoyaltyTooltip(out string text, out Color color)
    {
      text = "-";
      color = ViewModels.TooltipHelper.DefaultTooltipColor;
    }

    private static void GetRelationThresholds(Clan clan, out int MinRelationThreshold, out int MaxRelationThreshold, out int LeaveRelationThreshold)
    {
      MinRelationThreshold = -101;
      MaxRelationThreshold = 101;
      LeaveRelationThreshold = 0;
      foreach (Kingdom kingdom in Kingdom.All)
      {
        if (kingdom == clan.Kingdom)
          LeaveRelationThreshold = GetRelationThreshold(clan, null);
        else
        {
          MinRelationThreshold = MinRelationThreshold < -100 ? GetRelationThreshold(clan, kingdom) : Math.Min(MinRelationThreshold, GetRelationThreshold(clan, kingdom));
          MaxRelationThreshold = MaxRelationThreshold > 100 ? GetRelationThreshold(clan, kingdom) : Math.Max(MaxRelationThreshold, GetRelationThreshold(clan, kingdom));
        }
      }
    }

    private static void GetAveragedLoyaltyStatus(Clan clan, out string text, out Color color)
    {
      GetRelationThresholds(clan, out int MinRelationThreshold, out int MaxRelationThreshold, out int LeaveRelationThreshold);
      int RelationWithLiege = clan.Leader.GetRelation(clan.Kingdom.Ruler);
      if (RelationWithLiege > LeaveRelationThreshold && RelationWithLiege > MaxRelationThreshold)
      {
        text = TooltipLoyal.ToLocalizedString();
        color = Colors.Green;
      }
      else
      if (RelationWithLiege > LeaveRelationThreshold && RelationWithLiege > MinRelationThreshold)
      {
        text = TooltipRatherLoyal.ToLocalizedString();
        color = ViewModels.TooltipHelper.DefaultTooltipColor;
      }
      else
      if (RelationWithLiege > MinRelationThreshold)
      {
        text = TooltipSomewhatLoyal.ToLocalizedString();
        color = ViewModels.TooltipHelper.DefaultTooltipColor;
      }
      else
      {
        text = TooltipNotLoyal.ToLocalizedString();
        color = Colors.Red;
      }
    }

    public enum ELState : byte
    {
      SystemDisabled,
      FactionOutOfScope,
      UnderRequiredService,
      UnaffectedByRelations,
      AffectedByRelations
    }
  }
}
