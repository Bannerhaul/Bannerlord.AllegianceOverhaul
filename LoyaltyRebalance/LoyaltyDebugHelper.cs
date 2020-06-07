using MCM.Abstractions.Data;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Barterables;
using TaleWorlds.Localization;
using AllegianceOverhaul.LoyaltyRebalance.EnsuredLoyalty;

namespace AllegianceOverhaul.LoyaltyRebalance
{
  internal class LoyaltyDebugHelper
  {
    //Fp18vLHE,16kFtifY
    private const string LeaveDecision = "{=KBPBbuE9}leave";
    private const string StayDecision = "{=GmDkOQ7C}stay";

    private const string ClanTypeMinorFaction = "{=uNS0QtqU}Minor faction";
    private const string ClanTypeUsual = "{=zmR6sT03}The clan";

    private const string ServiceTypeMercenary = "{=CYUKnxOH}under mercenary service of";
    private const string ServiceTypeUsual = "{=rdpdUqkQ}of";

    private const string Debug_Leave = "{=K23FeRhj}{CLAN_TYPE} {LEAVING_CLAN} {SERVICE_TYPE} {CURRENT_KINGDOM} is considering leaving. Value of the kingdom for them is {LEAVE_BARTERABLE}, threshold is {LEAVE_THRESHOLD}. Natively they would {CLAN_DECISION}.{ENSURED_LOYALTY_RESULT}";
    private const string Debug_Defect = "{=WNqBu5Ye}{CLAN_TYPE} {LEAVING_CLAN} {SERVICE_TYPE} {CURRENT_KINGDOM} is considering defecting to {TARGET_KINGDOM}. Current kingdom worth {CURRENT_BARTERABLE} for them and other kingdom is valued at {TARGET_BARTERABLE}. Natively they would {CLAN_DECISION}.{ENSURED_LOYALTY_RESULT}";

    public static void LeaveKingdomDebug(Clan clan)
    {
      bool DebugEnsuredLoyalty = Settings.Instance.EnableGeneralDebugging;
      SettingsHelper.InDebugBranch = true;
      if (!DebugEnsuredLoyalty || !SettingsHelper.FactionInScope(clan, Settings.Instance.EnsuredLoyaltyDebugScope) || !SettingsHelper.InDebugBranch)
        return;
      
      LeaveKingdomAsClanBarterable asClanBarterable = new LeaveKingdomAsClanBarterable(clan.Leader, (PartyBase)null);
      int ClanBarterableValueForFaction = asClanBarterable.GetValueForFaction((IFaction)clan);
      int StayThreshold = clan.IsMinorFaction ? 500 : 0;
      bool NativeDecision = ClanBarterableValueForFaction <= StayThreshold;

      TextObject ResultTextObject = new TextObject(Debug_Leave);
      ResultTextObject.SetTextVariable("CLAN_TYPE", clan.IsMinorFaction ? ClanTypeMinorFaction : ClanTypeUsual);
      ResultTextObject.SetTextVariable("LEAVING_CLAN", clan.Name);
      ResultTextObject.SetTextVariable("SERVICE_TYPE", clan.IsUnderMercenaryService ? ServiceTypeMercenary : ServiceTypeUsual);
      ResultTextObject.SetTextVariable("CURRENT_KINGDOM", clan.Kingdom.Name);
      ResultTextObject.SetTextVariable("LEAVE_BARTERABLE", ClanBarterableValueForFaction.ToString("N"));
      ResultTextObject.SetTextVariable("LEAVE_THRESHOLD", StayThreshold.ToString("N"));
      ResultTextObject.SetTextVariable("CLAN_DECISION", NativeDecision ? StayDecision : LeaveDecision);

      bool IsLoyaltyEnsured = LoyaltyManager.CheckLoyalty(clan, out TextObject LoyaltyTextObject);
      LoyaltyTextObject.SetTextVariable("TRANSITION_PART", NativeDecision == IsLoyaltyEnsured ? LoyaltyManager.TransitionFromSame: LoyaltyManager.TransitionFromDifferent);
      ResultTextObject.SetTextVariable("ENSURED_LOYALTY_RESULT", LoyaltyTextObject.ToString());

      MessageHelper.SimpleMessage(ResultTextObject.ToString());
      SettingsHelper.InDebugBranch = false;
    }

    public static void DefectKingdomDebug(Clan clan, Kingdom kingdom)
    {
      bool DebugEnsuredLoyalty = Settings.Instance.EnableGeneralDebugging;
      SettingsHelper.InDebugBranch = true;
      DefaultDropdown<string> DebugScope = Settings.Instance.EnsuredLoyaltyDebugScope;
      if (!DebugEnsuredLoyalty || !(SettingsHelper.FactionInScope(clan, DebugScope) || SettingsHelper.FactionInScope(kingdom, DebugScope)) || !SettingsHelper.InDebugBranch)
        return;

      JoinKingdomAsClanBarterable asClanBarterable = new JoinKingdomAsClanBarterable(clan.Leader, kingdom);
      int ClanBarterableValueForClan = asClanBarterable.GetValueForFaction((IFaction)clan);
      int ClanBarterableValueForKingdom = asClanBarterable.GetValueForFaction((IFaction)kingdom);
      bool NativeDecision = ClanBarterableValueForClan + ClanBarterableValueForKingdom <= 0;

      TextObject ResultTextObject = new TextObject(Debug_Defect);
      ResultTextObject.SetTextVariable("CLAN_TYPE", clan.IsMinorFaction ? ClanTypeMinorFaction : ClanTypeUsual);
      ResultTextObject.SetTextVariable("LEAVING_CLAN", clan.Name);
      ResultTextObject.SetTextVariable("SERVICE_TYPE", clan.IsUnderMercenaryService ? ServiceTypeMercenary : ServiceTypeUsual);
      ResultTextObject.SetTextVariable("CURRENT_KINGDOM", clan.Kingdom.Name);
      ResultTextObject.SetTextVariable("TARGET_KINGDOM", kingdom.Name);
      ResultTextObject.SetTextVariable("CURRENT_BARTERABLE", ClanBarterableValueForClan.ToString("N"));
      ResultTextObject.SetTextVariable("TARGET_BARTERABLE", ClanBarterableValueForKingdom.ToString("N"));
      ResultTextObject.SetTextVariable("CLAN_DECISION", NativeDecision ? StayDecision : LeaveDecision);

      bool IsLoyaltyEnsured = LoyaltyManager.CheckLoyalty(clan, out TextObject LoyaltyTextObject, kingdom);
      LoyaltyTextObject.SetTextVariable("TRANSITION_PART", NativeDecision == IsLoyaltyEnsured ? LoyaltyManager.TransitionFromSame : LoyaltyManager.TransitionFromDifferent);
      ResultTextObject.SetTextVariable("ENSURED_LOYALTY_RESULT", LoyaltyTextObject.ToString());

      MessageHelper.SimpleMessage(ResultTextObject.ToString());
      SettingsHelper.InDebugBranch = false;
    }
  }
}