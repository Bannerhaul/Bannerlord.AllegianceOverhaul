using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace AllegianceOverhaul.LoyaltyRebalance
{
  public class EnsuredLoyalty
  {
    //BB49Q65v,xJ9hsFLS,DQ2ATMdL,qGEalMT7,adoPNE7R,vhJZj4an
    internal const string TransitionFromSame = "{=5EjuUUvH}Furthermore,";
    internal const string TransitionFromDifferent = "{=PKqNif5j}But";

    private const string ResultTrue = "{=rpZFLb2V}loyalty will be";
    private const string ResultFalse = "{=9Ml0rRQw}loyalty won't be";

    private const string ServiceTypeOath = "{=h3RHyeGO}oath of fealty";
    private const string ServiceTypeMercenary = "{=s3RAXuiB}mercenary service";

    private const string RelationLow = "{=nixfDSWU}too low";
    private const string RelationHigh = "{=GYctexwu}high enough";

    private const string ReasonIsNotEnabled = "{=ZNEdXaUc}it is not enabled";
    private const string ReasonOutOfScope = "{=RrhbXipK}faction is out of scope";
    private const string ReasonRelationEnabled = "{=TJBHPB3s}clan leader's relationship with you is {CHECK_RESULT} ({CURRENT_RELATION} out of required {REQUIRED_RELATION})";
    private const string ReasonRelationDisabled = "{=jPA16DTJ}clan leader's relationship with you does not affect it and clan fulfilled minimal obligations";
    private const string ReasonServicePeriod = "{=7jtTAw2k}clan is under {SERVICE_TYPE} for {DAYS_UNDER_SERVICE} days out of required {REQUIRED_DAYS_UNDER_SERVICE}";

    private const string Debug_EnsuredLoyalty = "{=4R4kwdpa} {TRANSITION_PART} {LOYALTY_CHECK_RESULT} ensured, as {REASON}.";

    private static int KingdomFortificationsCount(Kingdom kingdom)
    {
      int Count = 0;
      foreach (Clan clan in kingdom.Clans)
      {
        Count += clan.Fortifications?.Count ?? 0;
      }
      return Count;
    }

    private static int GetHonorModifier(Hero leader, bool Defecting = false)
    {
      int HonorLevel = leader.GetTraitLevel(DefaultTraits.Honor);
      if (HonorLevel < 0)
        return -HonorLevel * (Defecting ? Settings.Instance.NegativeHonorEnsuredLoyaltyModifier_Defecting : Settings.Instance.NegativeHonorEnsuredLoyaltyModifier_Leaving);
      else
        return -HonorLevel * (Defecting ? Settings.Instance.PositiveHonorEnsuredLoyaltyModifier_Defecting : Settings.Instance.PositiveHonorEnsuredLoyaltyModifier_Leaving);
    }

    public static int GetRelationThreshold(Clan clan, bool Defecting = false)
    {
      int RelationThreshold = Settings.Instance.EnsuredLoyaltyBaseline;
      
      if (Settings.Instance.UseContextForEnsuredLoyalty)
        RelationThreshold +=
          (clan.IsMinorFaction ? Settings.Instance.MinorFactionEnsuredLoyaltyModifier : 0) +
          (Defecting ? Settings.Instance.DefectionEnsuredLoyaltyModifier : 0) +
          (clan.Fortifications?.Count < 1 ? Settings.Instance.LandlessClanEnsuredLoyaltyModifier : 0) +
          (KingdomFortificationsCount(clan.Kingdom) < 1 ? Settings.Instance.LandlessKingdomEnsuredLoyaltyModifier : 0);

      if (Settings.Instance.UseHonorForEnsuredLoyalty)
        RelationThreshold += GetHonorModifier(clan.Leader, Defecting);

      return RelationThreshold;
    }

    public static bool CheckLoyalty(Clan clan, out TextObject DebugTextObject, bool Defecting = false)
    {
      DebugTextObject = new TextObject(Debug_EnsuredLoyalty);
      if (!Settings.Instance.UseEnsuredLoyalty)
      {
        DebugTextObject.SetTextVariable("LOYALTY_CHECK_RESULT", ResultFalse);
        DebugTextObject.SetTextVariable("REASON", ReasonIsNotEnabled);
        return false;
      } else
      {
        if (!SettingsHelper.FactionInScope(clan, Settings.Instance.EnsuredLoyaltyScope))
        {
          DebugTextObject.SetTextVariable("LOYALTY_CHECK_RESULT", ResultFalse);
          DebugTextObject.SetTextVariable("REASON", ReasonOutOfScope);
          return false;
        } else
        {
          int DaysWithKingdom = (int)(CampaignTime.Now - clan.LastFactionChangeTime).ToDays;
          int RequiredDays = clan.IsUnderMercenaryService ? Settings.Instance.MinorFactionServicePeriod : (clan.IsMinorFaction ? Settings.Instance.MinorFactionOathPeriod : Settings.Instance.FactionOathPeriod);
          if (clan.Kingdom != null && DaysWithKingdom <= RequiredDays)
          {
            TextObject ReasonPeriod = new TextObject(ReasonServicePeriod);
            ReasonPeriod.SetTextVariable("SERVICE_TYPE", clan.IsUnderMercenaryService ? ServiceTypeMercenary : ServiceTypeOath);
            ReasonPeriod.SetTextVariable("DAYS_UNDER_SERVICE", DaysWithKingdom);
            ReasonPeriod.SetTextVariable("REQUIRED_DAYS_UNDER_SERVICE", RequiredDays);
            DebugTextObject.SetTextVariable("LOYALTY_CHECK_RESULT", ResultTrue);
            DebugTextObject.SetTextVariable("REASON", ReasonPeriod.ToString());
            return true;
          } else
          {
            if (Settings.Instance.UseRelationForEnsuredLoyalty)
            {
              int CurrentRelation = (int)clan.Leader.GetRelationWithPlayer();
              int RequiredRelation = GetRelationThreshold(clan, Defecting);
              bool RelationCheckResult = CurrentRelation >= RequiredRelation;
              TextObject ReasonRelation = new TextObject(ReasonRelationEnabled);
              ReasonRelation.SetTextVariable("CHECK_RESULT", RelationCheckResult ? RelationHigh : RelationLow);
              ReasonRelation.SetTextVariable("CURRENT_RELATION", CurrentRelation);
              ReasonRelation.SetTextVariable("REQUIRED_RELATION", RequiredRelation);
              DebugTextObject.SetTextVariable("LOYALTY_CHECK_RESULT", RelationCheckResult ? ResultTrue : ResultFalse);
              DebugTextObject.SetTextVariable("REASON", ReasonRelation.ToString());
              return RelationCheckResult;
            } else
            {
              DebugTextObject.SetTextVariable("LOYALTY_CHECK_RESULT", ResultFalse);
              DebugTextObject.SetTextVariable("REASON", ReasonRelationDisabled);
              return false;
            }
          }
        }
      }
    }
    public static bool CheckLoyalty(Clan clan, bool Defecting = false)
    {
      if (!Settings.Instance.UseEnsuredLoyalty || !SettingsHelper.FactionInScope(clan, Settings.Instance.EnsuredLoyaltyScope))
        return false;

      int DaysWithKingdom = (int)(CampaignTime.Now - clan.LastFactionChangeTime).ToDays;
      if
        (
          (clan.IsUnderMercenaryService && DaysWithKingdom <= Settings.Instance.MinorFactionServicePeriod) ||
          (!clan.IsUnderMercenaryService && clan.Kingdom != null && DaysWithKingdom <= (clan.IsMinorFaction ? Settings.Instance.MinorFactionOathPeriod : Settings.Instance.FactionOathPeriod))
        )
        return true;

      if (Settings.Instance.UseRelationForEnsuredLoyalty)
        return (int)clan.Leader.GetRelationWithPlayer() >= GetRelationThreshold(clan, Defecting);
      else
        return false;
    }
  }
}
