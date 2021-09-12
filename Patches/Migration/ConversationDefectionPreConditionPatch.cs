using AllegianceOverhaul.Helpers;
using AllegianceOverhaul.MigrationTweaks;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Barterables;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.BarterBehaviors;
using TaleWorlds.Core;

namespace AllegianceOverhaul.Patches.Migration
{
    [HarmonyPatch(typeof(LordDefectionCampaignBehavior), "conversation_lord_persuade_option_reaction_pre_reject_on_condition")]
    public static class ConversationDefectionPreConditionPatch
    {
        private delegate int CalculateAverageItemValueInNearbySettlementsDelegate(ItemBarterBehavior instance, EquipmentElement itemRosterElement, PartyBase involvedParty, List<Settlement> nearbySettlements);
        private static readonly CalculateAverageItemValueInNearbySettlementsDelegate? deCalculateAverageItemValueInNearbySettlements = AccessHelper.GetDelegate<CalculateAverageItemValueInNearbySettlementsDelegate>(typeof(ItemBarterBehavior), "CalculateAverageItemValueInNearbySettlements");

        public static bool Prefix(ref bool __result) //Bool prefixes compete with each other and skip others, as well as original, if return false
        {
            try
            {
                if (SettingsHelper.SubSystemEnabled(SubSystemType.PersuasionLockoutTweak))
                {
                    double valueForFaction = new JoinKingdomAsClanBarterable(Hero.OneToOneConversationHero, (Kingdom)Hero.MainHero.MapFaction).GetValueForFaction(Hero.OneToOneConversationHero.Clan);

                    if (Settings.Instance!.PLDemandThreshold > 0f && valueForFaction < -Settings.Instance!.PLDemandThreshold * 1000000)
                    {
                        __result = true;
                    }
                    else
                    {
                        if (Hero.MainHero.GetPerkValue(DefaultPerks.Trade.EverythingHasAPrice))
                        {
                            __result = false;
                        }
                        else
                        {
                            int availableBarterables = Hero.MainHero.Gold;
                            PartyBase? mainParty = MobileParty.MainParty?.Party;
                            PartyBase? otherParty = Hero.OneToOneConversationHero.PartyBelongedTo?.Party;

                            if (mainParty != null && otherParty != null)
                            {
                                AOSettlementDistance settlementDistance = new();
                                List<Settlement>? closestSettlements = settlementDistance.GetClosestSettlements(Hero.OneToOneConversationHero.GetPosition().AsVec2);
                                ItemBarterBehavior itemBarterBehavior = Campaign.Current.GetCampaignBehavior<ItemBarterBehavior>();
                                ItemRoster playerItemRoster = MobileParty.MainParty!.ItemRoster;

                                for (var i = 0; i < playerItemRoster.Count; i++)
                                {
                                    ItemRosterElement elementCopyAtIndex = playerItemRoster.GetElementCopyAtIndex(i);
                                    if (elementCopyAtIndex.Amount > 0 && elementCopyAtIndex.EquipmentElement.GetBaseValue() > 100)
                                    {                                                                                
                                        int valueInNearbySettlements = deCalculateAverageItemValueInNearbySettlements!(itemBarterBehavior, elementCopyAtIndex.EquipmentElement, mainParty, closestSettlements);
                                        ItemBarterable barterable = new ItemBarterable(Hero.MainHero, Hero.OneToOneConversationHero, mainParty, otherParty, elementCopyAtIndex, valueInNearbySettlements);

                                        availableBarterables += Math.Max(0, barterable.GetUnitValueForFaction(Hero.OneToOneConversationHero.Clan)) * elementCopyAtIndex.Amount;
                                    }
                                }
                            }
                            __result = valueForFaction < -availableBarterables;
                        }
                    }
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                DebugHelper.HandleException(ex, methodInfo, "Harmony patch for conversation_lord_persuade_option_reaction_pre_reject_on_condition");
                return true;
            }
        }
        public static bool Prepare()
        {
            return SettingsHelper.SubSystemEnabled(SubSystemType.PersuasionLockoutTweak);
        }
    }
}
