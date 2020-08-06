using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Barterables;
using TaleWorlds.Localization;
using AllegianceOverhaul.LoyaltyRebalance.EnsuredLoyalty;
using AllegianceOverhaul.Helpers;

namespace AllegianceOverhaul.LoyaltyRebalance
{
  internal static class LoyaltyDebugHelper
  {
    //Fp18vLHE,16kFtifY
    private const string LeaveDecision = "{=KBPBbuE9}leave";
    private const string StayDecision = "{=GmDkOQ7C}stay";

    private const string Debug_Leave = "{=K23FeRhj}{?LEAVING_CLAN.MINOR_FACTION}Minor faction{?}The clan{\\?} {LEAVING_CLAN.NAME} {?LEAVING_CLAN.UNDER_CONTRACT}under mercenary service of{?}of{\\?} {LEAVING_CLAN_KINGDOM.NAME} is considering leaving. Value of the kingdom for them is {LEAVE_BARTERABLE}, threshold is {LEAVE_THRESHOLD}. Natively they would {CLAN_DECISION}.{ENSURED_LOYALTY_RESULT}";
    private const string Debug_Defect = "{=WNqBu5Ye}{?LEAVING_CLAN.MINOR_FACTION}Minor faction{?}The clan{\\?} {LEAVING_CLAN.NAME} {?LEAVING_CLAN.UNDER_CONTRACT}under mercenary service of{?}of{\\?} {LEAVING_CLAN_KINGDOM.NAME} is considering defecting to {TARGET_KINGDOM.NAME}. Current kingdom worth {CURRENT_BARTERABLE} for them and other kingdom is valued at {TARGET_BARTERABLE}. Natively they would {CLAN_DECISION}.{ENSURED_LOYALTY_RESULT}";

    public static bool InDebugBranch { get;  private set; } = false;

    public static void LeaveKingdomDebug(Clan clan)
    {
      if (!SettingsHelper.SystemDebugEnabled(AOSystems.EnsuredLoyalty, DebugType.General, clan))
        return;
      
      InDebugBranch = true;
      LeaveKingdomAsClanBarterable asClanBarterable = new LeaveKingdomAsClanBarterable(clan.Leader, null);
      int ClanBarterableValueForFaction = asClanBarterable.GetValueForFaction(clan);
      int StayThreshold = (Settings.Instance.FixMinorFactionVassals ? clan.IsUnderMercenaryService : clan.IsMinorFaction) ? 500 : 0;
      bool NativeDecision = ClanBarterableValueForFaction <= StayThreshold;

      TextObject ResultTextObject = new TextObject(Debug_Leave);
      StringHelper.SetEntitiyProperties(ResultTextObject, "LEAVING_CLAN", clan, true);
      StringHelper.SetNumericVariable(ResultTextObject, "LEAVE_BARTERABLE", ClanBarterableValueForFaction, "N0");
      StringHelper.SetNumericVariable(ResultTextObject, "LEAVE_THRESHOLD", StayThreshold, "N0");
      ResultTextObject.SetTextVariable("CLAN_DECISION", NativeDecision ? StayDecision : LeaveDecision);

      bool IsLoyaltyEnsured = LoyaltyManager.CheckLoyalty(clan, out TextObject LoyaltyTextObject);
      LoyaltyTextObject.SetTextVariable("TRANSITION_PART", NativeDecision == IsLoyaltyEnsured ? LoyaltyManager.TransitionFromSame : LoyaltyManager.TransitionFromDifferent);
      ResultTextObject.SetTextVariable("ENSURED_LOYALTY_RESULT", LoyaltyTextObject);

      MessageHelper.SimpleMessage(ResultTextObject);
      InDebugBranch = false;
    }

    public static void DefectKingdomDebug(Clan clan, Kingdom kingdom)
    {
      if (!SettingsHelper.SystemDebugEnabled(AOSystems.EnsuredLoyalty, DebugType.General, new List<IFaction>() { clan, kingdom }))
        return;

      InDebugBranch = true;
      JoinKingdomAsClanBarterable asClanBarterable = new JoinKingdomAsClanBarterable(clan.Leader, kingdom);
      int ClanBarterableValueForClan = asClanBarterable.GetValueForFaction(clan);
      int ClanBarterableValueForKingdom = asClanBarterable.GetValueForFaction(kingdom);
      bool NativeDecision = ClanBarterableValueForClan + ClanBarterableValueForKingdom <= 0;

      TextObject ResultTextObject = new TextObject(Debug_Defect);
      StringHelper.SetEntitiyProperties(ResultTextObject, "LEAVING_CLAN", clan, true);
      StringHelper.SetEntitiyProperties(ResultTextObject, "TARGET_KINGDOM", kingdom);
      StringHelper.SetNumericVariable(ResultTextObject, "CURRENT_BARTERABLE", ClanBarterableValueForClan, "N0");
      StringHelper.SetNumericVariable(ResultTextObject, "TARGET_BARTERABLE", ClanBarterableValueForKingdom, "N0");
      ResultTextObject.SetTextVariable("CLAN_DECISION", NativeDecision ? StayDecision : LeaveDecision);

      bool IsLoyaltyEnsured = LoyaltyManager.CheckLoyalty(clan, out TextObject LoyaltyTextObject, kingdom);
      LoyaltyTextObject.SetTextVariable("TRANSITION_PART", NativeDecision == IsLoyaltyEnsured ? LoyaltyManager.TransitionFromSame : LoyaltyManager.TransitionFromDifferent);
      ResultTextObject.SetTextVariable("ENSURED_LOYALTY_RESULT", LoyaltyTextObject);

      MessageHelper.SimpleMessage(ResultTextObject);
      InDebugBranch = false;
    }
  }
}