using System;
using System.Collections.Generic;

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

        public static readonly Type heroRelationsType = TypeByName("TaleWorlds.CampaignSystem.CharacterRelationManager+HeroRelations");
        public static readonly FieldRef<CharacterRelationManager, object> heroRelationsInstanceByRef = FieldRefAccess<CharacterRelationManager, object>("_heroRelations");
        public static readonly FieldRef<object, Dictionary<long, int>> heroRelationsByRef = FieldRefAccess<Dictionary<long, int>>(heroRelationsType, "_relations");

        public static readonly Type makePeaceDecisionOutcomeType = TypeByName("TaleWorlds.CampaignSystem.Election.MakePeaceKingdomDecision+MakePeaceDecisionOutcome");
        public static readonly FieldRef<object, bool> ShouldPeaceBeDeclaredByRef = FieldRefAccess<bool>(makePeaceDecisionOutcomeType, "ShouldPeaceBeDeclared");

        public static readonly Type declareWarDecisionOutcomeType = TypeByName("TaleWorlds.CampaignSystem.Election.DeclareWarDecision+DeclareWarDecisionOutcome");
        public static readonly FieldRef<object, bool> ShouldWarBeDeclaredByRef = FieldRefAccess<bool>(declareWarDecisionOutcomeType, "ShouldWarBeDeclared");

        public static readonly Type clanAsDecisionOutcomeType = TypeByName("TaleWorlds.CampaignSystem.Election.SettlementClaimantDecision+ClanAsDecisionOutcome");
        public static readonly FieldRef<object, Clan> ClanAsDecisionOutcomeByRef = FieldRefAccess<Clan>(clanAsDecisionOutcomeType, "Clan");
    }
}
