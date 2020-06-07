using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Barterables;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace AllegianceOverhaul.LoyaltyRebalance.EnsuredLoyalty
{
  public class LoyaltyCostManager
  {
    public sealed class ComplexCost
    {
      public int InfluenceCost { get; }
      public int GoldCost { get; }
      public ComplexCost(int influenceCost, int goldCost)
      {
        InfluenceCost = influenceCost;
        GoldCost = goldCost;
      }
      public override string ToString()
      {
        return $"[InfluenceCost = {InfluenceCost}; GoldCost = {GoldCost}]";
      }
    }

    //CPk4UlNb,PHLW0tH1,BbGZQujC,zMaP4T5X,9nAkgARf,Q4U1RxkS,xSYLqZ3q,UxXnDAyP,QvRH6auB,nnWihTh4
    private const string LeaderIsMaleString = "{=w3OMVa7M}he";
    private const string LeaderIsFemaleString = "{=J5SxdaLi}she";
    
    private const string WithholdPricePayed = "{=MBVs126y}{KINGDOM_LEADER} decided to withhold {LEAVING_CLAN} in {CURRENT_KINGDOM}. Initially {KINGDOM_LEADER_PRONOUN} had {INITIAL_INFLUENCE} influence and {INITILAL_GOLD} gold. Withholding required {INFLUENCE_COST} influence and {GOLD_COST} gold. After intervention {KINGDOM_LEADER_PRONOUN} has {RESULT_INFLUENCE} influence and {RESULT_GOLD} gold.";
    private const string WithholdPriceRejected = "{=PJSz7d09}{KINGDOM_LEADER} of {CURRENT_KINGDOM} decided to let {LEAVING_CLAN} go. At that moment {KINGDOM_LEADER_PRONOUN} had {INITIAL_INFLUENCE} influence and {INITILAL_GOLD} gold. Withholding the clan would require {INFLUENCE_COST} influence and {GOLD_COST} gold.";

    private const string ActionLeaving = "{=4GvY6ohR}leaving your kingdom";
    private const string ActionDefecting = "{=soNby6VP}going to leave your kingdom and join the {TARGET_KINGDOM}";
    private const string PlayerInquiryHeader = "{=Jz9zbcJh}Clan {LEAVING_CLAN} is leaving!";
    private const string PlayerInquiryBody = "{=Gy4fCQV4}Clan {LEAVING_CLAN} is {ACTION_DESCRIPTION}! Will you intervene and insist they should stay? That would require {INFLUENCE_COST} influence and {GOLD_COST} gold.";

    private const string ButtonWithholdText = "{=7oJXB1PA}Withhold them";
    private const string ButtonReleaseText = "{=BkX1Q6G6}Let them go";

    public Clan LeavingClan { get; }
    public Kingdom TargetKingdom { get; }
    public ComplexCost WithholdCost { get; }
    public double BarterableSum { get; private set; }
    public double BaseCalculatedCost { get; private set; }
    protected Barterable ClanBarterable { get; private set; }

    public LoyaltyCostManager(Clan clan, Kingdom targetKingdom = null)
    {
      LeavingClan = clan;
      TargetKingdom = targetKingdom;
      BarterableSum = 0;
      BaseCalculatedCost = 0;
      WithholdCost = GetWithholdCost();
    }
    private ComplexCost GetWithholdCost()
    {
      if (!Settings.Instance.UseEnsuredLoyalty || !Settings.Instance.UseRelationForEnsuredLoyalty || !Settings.Instance.UseWithholdPrice || !SettingsHelper.FactionInScope(LeavingClan, Settings.Instance.EnsuredLoyaltyScope))
        return null;

      if (TargetKingdom != null)
      {
        JoinKingdomAsClanBarterable asClanBarterable = new JoinKingdomAsClanBarterable(LeavingClan.Leader, TargetKingdom);
        int ClanBarterableValueForClan = asClanBarterable.GetValueForFaction((IFaction)LeavingClan);
        int ClanBarterableValueForKingdom = asClanBarterable.GetValueForFaction((IFaction)TargetKingdom);
        ClanBarterable = asClanBarterable;
        BarterableSum = ClanBarterableValueForClan + ClanBarterableValueForKingdom;
      } else
      {
        LeaveKingdomAsClanBarterable asClanBarterable = new LeaveKingdomAsClanBarterable(LeavingClan.Leader, (PartyBase)null);
        int ClanBarterableValueForFaction = asClanBarterable.GetValueForFaction((IFaction)LeavingClan);
        ClanBarterable = asClanBarterable;
        BarterableSum = ClanBarterableValueForFaction - (LeavingClan.IsMinorFaction ? 500 : 0);
      }

      if (BarterableSum <= Settings.Instance.WithholdToleranceLimit * 1000000)
        return null;

      BaseCalculatedCost = Math.Sqrt(BarterableSum) / Math.Log10(BarterableSum);

      return
        Settings.Instance.UseWithholdBribing && Settings.Instance.WithholdToleranceLimitForBribes * 1000000 < BarterableSum
          ? new ComplexCost((int)(BaseCalculatedCost * (double)Settings.Instance.WithholdInfluenceMultiplier), (int)BaseCalculatedCost * Settings.Instance.WithholdGoldMultiplier)
          : new ComplexCost((int)(BaseCalculatedCost * (double)Settings.Instance.WithholdInfluenceMultiplier), 0);
    }
    private int GetScoreForKingdomToWithholdClan()
    {
      double RelativeScoreToLeave = new LeaveKingdomAsClanBarterable(LeavingClan.Leader, null).GetValueForFaction(LeavingClan) - (LeavingClan.IsMinorFaction ? 500 : 0);
      RelativeScoreToLeave = Math.Sqrt(Math.Abs(RelativeScoreToLeave)) * (RelativeScoreToLeave >= 0 ? -500 : 250); //invert it as negative is good for us
      double CostScore = WithholdCost.InfluenceCost / Math.Max(0.001, LeavingClan.Kingdom.RulingClan.Influence) * WithholdCost.InfluenceCost * 1000 + WithholdCost.GoldCost / Math.Max(0.001, LeavingClan.Kingdom.Ruler.Gold) * WithholdCost.GoldCost;
      double ClanCountModifier = 0;
      foreach (Clan clan in LeavingClan.Kingdom.Clans)
      {
        if (clan != LeavingClan.Kingdom.RulingClan)
        {
          int relationBetweenClans = FactionManager.GetRelationBetweenClans(LeavingClan.Kingdom.RulingClan, clan);
          ClanCountModifier +=
            1 * (clan.IsUnderMercenaryService ? 0.5 : 1) * (1.0 + (LeavingClan.Kingdom.Culture == clan.Culture ? 0.150000005960464 : -0.150000005960464))
            * Math.Min(1.3, Math.Max(0.5, 1.0 + Math.Sqrt((double)Math.Abs(relationBetweenClans)) * (relationBetweenClans < 0 ? -0.03 : 0.02)));
        }
      }
      double RelativeStrengthModifier = LeavingClan.TotalStrength / (LeavingClan.Kingdom.TotalStrength - LeavingClan.Kingdom.RulingClan.TotalStrength);
      float SettlementValue = (TargetKingdom.IsAtWarWith(LeavingClan.Kingdom) && !LeavingClan.IsUnderMercenaryService) ? LeavingClan.CalculateSettlementValue(LeavingClan.Kingdom) : 0;
      double Result = (Campaign.Current.Models.DiplomacyModel.GetScoreOfKingdomToGetClan(LeavingClan.Kingdom, LeavingClan) + RelativeScoreToLeave) * (RelativeStrengthModifier + (LeavingClan.IsUnderMercenaryService ? 0.1 : 1)) + SettlementValue - CostScore * ClanCountModifier;
      if (Settings.Instance.EnableTechnicalDebugging && SettingsHelper.FactionInScope(LeavingClan, Settings.Instance.EnsuredLoyaltyDebugScope))
        MessageHelper.TechnicalMessage($"Score for Kingdom {LeavingClan.Kingdom.Name} to withhold Clan {LeavingClan.Name}.\nRelativeScoreToLeave = {RelativeScoreToLeave:N}. CostScore = {CostScore:N}. ClanCountModifier = {ClanCountModifier}. ScoreOfKingdomToGetClan = {Campaign.Current.Models.DiplomacyModel.GetScoreOfKingdomToGetClan(LeavingClan.Kingdom, LeavingClan):N}. SettlementValue = {SettlementValue:N}. Result = {Result:N}.");
      return (int)Result;
    }
    public bool CheckAIWithholdDesision()
    {
      if (WithholdCost is null)
        return true;
      if (LeavingClan.Kingdom.RulingClan.Influence > WithholdCost.InfluenceCost && LeavingClan.Kingdom.Ruler.Gold > WithholdCost.GoldCost)
        return (GetScoreForKingdomToWithholdClan() >= 0);
      else
        return false;
    }
    public bool GetAIWithholdDesision()
    {
      bool Desision = CheckAIWithholdDesision();
      if (Settings.Instance.EnableGeneralDebugging && SettingsHelper.FactionInScope(LeavingClan, Settings.Instance.EnsuredLoyaltyDebugScope))
        ApplyAIWithholdDesisionWithLogging(Desision);
      else
        ApplyAIWithholdDesision(Desision);
      return Desision;
    }
    private void ApplyAIWithholdDesisionWithLogging(bool Desision)
    {
      if (WithholdCost is null)
        return;
      float InitialInfluence = LeavingClan.Kingdom.RulingClan.Influence;
      int InitilalGold = LeavingClan.Kingdom.Ruler.Gold;
      TextObject textObject = new TextObject(Desision ? WithholdPricePayed : WithholdPriceRejected);
      textObject.SetTextVariable("KINGDOM_LEADER", LeavingClan.Kingdom.Ruler.Name);
      textObject.SetTextVariable("LEAVING_CLAN", LeavingClan.Name);
      textObject.SetTextVariable("CURRENT_KINGDOM", LeavingClan.Kingdom.Name);
      textObject.SetTextVariable("KINGDOM_LEADER_PRONOUN", LeavingClan.Kingdom.Ruler.IsFemale ? LeaderIsFemaleString : LeaderIsMaleString);
      textObject.SetTextVariable("INITIAL_INFLUENCE", InitialInfluence.ToString("N"));
      textObject.SetTextVariable("INITILAL_GOLD", InitilalGold.ToString("N"));
      textObject.SetTextVariable("INFLUENCE_COST", WithholdCost.InfluenceCost.ToString("N"));
      textObject.SetTextVariable("GOLD_COST", WithholdCost.GoldCost.ToString("N"));
      if (Desision)
      {        
        ApplyAIWithholdDesision(Desision);
        textObject.SetTextVariable("RESULT_INFLUENCE", LeavingClan.Kingdom.RulingClan.Influence.ToString("N"));
        textObject.SetTextVariable("RESULT_GOLD", LeavingClan.Kingdom.Ruler.Gold.ToString("N"));
      }
      MessageHelper.SimpleMessage(textObject.ToString());
    }
    private void ApplyAIWithholdDesision(bool Desision)
    {
      if (!Desision || WithholdCost is null)
        return;
      LeavingClan.Kingdom.RulingClan.Influence = MBMath.ClampFloat(LeavingClan.Kingdom.RulingClan.Influence - WithholdCost.InfluenceCost, 0f, float.MaxValue);
      LeavingClan.Kingdom.Ruler.Gold = MBMath.ClampInt(LeavingClan.Kingdom.Ruler.Gold - WithholdCost.GoldCost, 0, int.MaxValue);
    }

    public void AwaitPlayerDecision()
    {
      TextObject InquiryHeader = new TextObject(PlayerInquiryHeader);
      InquiryHeader.SetTextVariable("LEAVING_CLAN", LeavingClan.Name);

      TextObject InquiryBody = new TextObject(PlayerInquiryBody);
      InquiryBody.SetTextVariable("LEAVING_CLAN", LeavingClan.Name);
      InquiryBody.SetTextVariable("ACTION_DESCRIPTION", TargetKingdom != null ? new TextObject(ActionDefecting).SetTextVariable("TARGET_KINGDOM", TargetKingdom.Name).ToString() : ActionLeaving);
      InquiryBody.SetTextVariable("INFLUENCE_COST", WithholdCost.InfluenceCost.ToString("N0"));
      InquiryBody.SetTextVariable("GOLD_COST", WithholdCost.GoldCost.ToString("N0"));

      TextObject ButtonWithhold = new TextObject(ButtonWithholdText);
      TextObject ButtonRelease = new TextObject(ButtonReleaseText);
      
      InformationManager.ShowInquiry(new InquiryData(InquiryHeader.ToString(), InquiryBody.ToString(), true, true, ButtonWithhold.ToString(), ButtonRelease.ToString(), () => WithholdClan(), () => ReleaseClan()), true);
    }

    public void WithholdClan()
    {
      float InitialInfluence = Clan.PlayerClan.Influence;
      int InitilalGold = Hero.MainHero.Gold;

      Clan.PlayerClan.Influence = MBMath.ClampFloat(Clan.PlayerClan.Influence - WithholdCost.InfluenceCost, 0, float.MaxValue);
      Hero.MainHero.Gold = MBMath.ClampInt(Hero.MainHero.Gold - WithholdCost.GoldCost, 0, int.MaxValue);

      if (Settings.Instance.EnableGeneralDebugging && SettingsHelper.FactionInScope(LeavingClan, Settings.Instance.EnsuredLoyaltyDebugScope))
      {
        TextObject textObject = new TextObject(WithholdPricePayed);
        textObject.SetTextVariable("KINGDOM_LEADER", LeavingClan.Kingdom.Ruler.Name);
        textObject.SetTextVariable("LEAVING_CLAN", LeavingClan.Name);
        textObject.SetTextVariable("CURRENT_KINGDOM", LeavingClan.Kingdom.Name);
        textObject.SetTextVariable("KINGDOM_LEADER_PRONOUN", LeavingClan.Kingdom.Ruler.IsFemale ? LeaderIsFemaleString : LeaderIsMaleString);
        textObject.SetTextVariable("INITIAL_INFLUENCE", InitialInfluence.ToString("N"));
        textObject.SetTextVariable("INITILAL_GOLD", InitilalGold.ToString("N"));
        textObject.SetTextVariable("INFLUENCE_COST", WithholdCost.InfluenceCost.ToString("N"));
        textObject.SetTextVariable("GOLD_COST", WithholdCost.GoldCost.ToString("N"));
        textObject.SetTextVariable("RESULT_INFLUENCE", LeavingClan.Kingdom.RulingClan.Influence.ToString("N"));
        textObject.SetTextVariable("RESULT_GOLD", LeavingClan.Kingdom.Ruler.Gold.ToString("N"));
        MessageHelper.SimpleMessage(textObject.ToString());
      }
    }
    public void ReleaseClan()
    {
      if (Settings.Instance.EnableGeneralDebugging && SettingsHelper.FactionInScope(LeavingClan, Settings.Instance.EnsuredLoyaltyDebugScope))
      {
        TextObject textObject = new TextObject(WithholdPriceRejected);
        textObject.SetTextVariable("KINGDOM_LEADER", LeavingClan.Kingdom.Ruler.Name);
        textObject.SetTextVariable("LEAVING_CLAN", LeavingClan.Name);
        textObject.SetTextVariable("CURRENT_KINGDOM", LeavingClan.Kingdom.Name);
        textObject.SetTextVariable("KINGDOM_LEADER_PRONOUN", LeavingClan.Kingdom.Ruler.IsFemale ? LeaderIsFemaleString : LeaderIsMaleString);
        textObject.SetTextVariable("INITIAL_INFLUENCE", LeavingClan.Kingdom.RulingClan.Influence.ToString("N"));
        textObject.SetTextVariable("INITILAL_GOLD", LeavingClan.Kingdom.Ruler.Gold.ToString("N"));
        textObject.SetTextVariable("INFLUENCE_COST", WithholdCost.InfluenceCost.ToString("N"));
        textObject.SetTextVariable("GOLD_COST", WithholdCost.GoldCost.ToString("N"));
        MessageHelper.SimpleMessage(textObject.ToString());
      }
      if (TargetKingdom is null)
        (ClanBarterable as LeaveKingdomAsClanBarterable).Apply();
      else
        //Campaign.Current.BarterManager.ExecuteAiBarter((IFaction)LeavingClan, (IFaction)TargetKingdom, LeavingClan.Leader, TargetKingdom.Leader, ClanBarterable);
        Patches.ExecuteAiBarterReversePatch.ExecuteAiBarter(Campaign.Current.BarterManager ?? BarterManager.Instance, (IFaction)LeavingClan, (IFaction)TargetKingdom, LeavingClan.Leader, TargetKingdom.Leader, ClanBarterable);        
    }
  }
}
