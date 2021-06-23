using System;

using TaleWorlds.Localization;

using AllegianceOverhaul.CampaignBehaviors.BehaviorManagers;
using static AllegianceOverhaul.Helpers.LocalizationHelper;

namespace AllegianceOverhaul.Helpers
{
  public static class StringHelper
  {
    public const string DECISION_IS_ON_COOLDOWN = "{=7GoBlY9o4}This proposal is on cooldown for {NUMBER_OF_DAYS} {?NUMBER_OF_DAYS.PLURAL_FORM}days{?}day{\\?}.";

    public static TextObject GetCooldownText(Type decisionType, float elapsedDaysUntilNow)
    {
      int RemainingDays = (int)Math.Ceiling(AOCooldownManager.GetRequiredDecisionCooldown(decisionType) - elapsedDaysUntilNow);
      TextObject cooldownText = new TextObject(DECISION_IS_ON_COOLDOWN);
      SetNumericVariable(cooldownText, "NUMBER_OF_DAYS", RemainingDays);
      return cooldownText;
    }
  }
}