using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Localization;

using static HarmonyLib.AccessTools;

namespace AllegianceOverhaul.Helpers
{
    public static class FieldAccessHelper
    {
        public static readonly FieldRef<TextObject, string> TextObjectValueByRef = FieldRefAccess<TextObject, string>("Value");
        public static readonly FieldRef<SettlementClaimantPreliminaryDecision, Clan> annexDecisionInitialOwnerByRef = FieldRefAccess<SettlementClaimantPreliminaryDecision, Clan>("_ownerClan");
    }
}