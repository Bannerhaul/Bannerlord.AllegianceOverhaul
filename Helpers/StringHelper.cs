using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using AllegianceOverhaul.CampaignBehaviors.BehaviorManagers;

namespace AllegianceOverhaul.Helpers
{
  public static class StringHelper
  {
    private const string PLURAL_FORM_TAG = "PLURAL_FORM";
    private const string OTHER_PLURAL_FORM_TAG = "OTHER_PLURAL_FORM";

    public const string DECISION_IS_ON_COOLDOWN = "{=}This proposal is on cooldown for {NUMBER_OF_DAYS} {?NUMBER_OF_DAYS.PLURAL_FORM}days{?}day{\\?}.";

    private static readonly ReadOnlyCollection<int> RussianPluralExceptions = new ReadOnlyCollection<int>(new List<int>() { 11, 12, 13, 14 });
    private static readonly ReadOnlyCollection<int> RussianSingularNumerics = new ReadOnlyCollection<int>(new List<int>() { 1, 2, 3, 4 });
    private static readonly ReadOnlyCollection<string> RussianGroupLanguageIDs = new ReadOnlyCollection<string>(new List<string>() { "Russian", "Русский", "Ukrainian", "Українська" });

    private static RecursiveCaller GetRecursiveCaller(RecursiveCaller currentCaller, RecursiveCaller receivedCaller)
    {
      return (RecursiveCaller)Math.Max((byte)currentCaller, (byte)receivedCaller);
    }

    private static RecursiveCaller GetCurrentCaller<T>(T entity) where T : class
    {
      switch (entity)
      {
        case Hero _:
          return RecursiveCaller.Hero;
        case Settlement _:
          return RecursiveCaller.Settlement;
        case Clan _:
          return RecursiveCaller.Clan;
        case Kingdom _:
          return RecursiveCaller.Kingdom;
        default:
          throw new ArgumentException(string.Format("{0} is not supported type", entity.GetType().FullName), nameof(entity));
      }
    }

    private static TextObject GetEntityTextObject<T>(T entity) where T : class
    {
      switch (entity)
      {
        case Hero hero:
          TextObject characterProperties = new TextObject();
          characterProperties.SetTextVariable("NAME", hero.Name);
          characterProperties.SetTextVariable("GENDER", hero.IsFemale ? 1 : 0);
          characterProperties.SetTextVariable("LINK", hero.EncyclopediaLinkWithName);
          characterProperties.SetTextVariable("FIRSTNAME", hero.FirstName ?? hero.Name);
          return characterProperties;
        case Settlement settlement:
          TextObject settlementProperties = new TextObject();
          settlementProperties.SetTextVariable("NAME", settlement.Name);
          settlementProperties.SetTextVariable("IS_TOWN", settlement.IsTown ? 1 : 0);
          settlementProperties.SetTextVariable("IS_CASTLE", settlement.IsCastle ? 1 : 0);
          settlementProperties.SetTextVariable("IS_VILLAGE", settlement.IsVillage ? 1 : 0);
          settlementProperties.SetTextVariable("LINK", settlement.EncyclopediaLinkWithName);
          return settlementProperties;
        case Clan clan:
          TextObject clanProperties = new TextObject();
          clanProperties.SetTextVariable("NAME", clan.Name);
          clanProperties.SetTextVariable("MINOR_FACTION", clan.IsMinorFaction ? 1 : 0);
          clanProperties.SetTextVariable("UNDER_CONTRACT", clan.IsUnderMercenaryService ? 1 : 0);
          clanProperties.SetTextVariable("LINK", clan.EncyclopediaLinkWithName);
          return clanProperties;
        case Kingdom kingdom:
          TextObject kingdomProperties = new TextObject();
          kingdomProperties.SetTextVariable("NAME", kingdom.Name);
          kingdomProperties.SetTextVariable("LINK", kingdom.EncyclopediaLinkWithName);
          return kingdomProperties;
        default:
          throw new ArgumentException(string.Format("{0} is not supported type", entity.GetType().FullName), nameof(entity));
      }
    }

    private static void SetRelatedProperties<T>(TextObject parentTextObject, string tag, T entity, bool addLeaderInfo, RecursiveCaller recursiveCaller) where T : class
    {
      switch (entity)
      {
        case Hero hero:
          SetEntitiyProperties(parentTextObject, tag + "_CLAN", hero.Clan, addLeaderInfo, GetRecursiveCaller(RecursiveCaller.Hero, recursiveCaller));
          break;
        case Settlement settlement:
          SetEntitiyProperties(parentTextObject, tag + "_CLAN", settlement.OwnerClan, addLeaderInfo, GetRecursiveCaller(RecursiveCaller.Settlement, recursiveCaller));
          break;
        case Clan clan:
          if (addLeaderInfo)
          {
            SetEntitiyProperties(parentTextObject, tag + "_LEADER", clan.Leader, false, RecursiveCaller.Clan);
          }
          SetEntitiyProperties(parentTextObject, tag + "_KINGDOM", clan.Kingdom, addLeaderInfo, GetRecursiveCaller(RecursiveCaller.Clan, recursiveCaller));
          break;
        case Kingdom kingdom:
          if (addLeaderInfo)
          {
            SetEntitiyProperties(parentTextObject, tag + "_LEADER", kingdom.Leader, false, RecursiveCaller.Kingdom);
          }
          break;
        default:
          throw new ArgumentException(string.Format("{0} is not supported type", entity.GetType().FullName), nameof(entity));
      }
    }

    public static void SetEntitiyProperties<T>(TextObject parentTextObject, string tag, T entity, bool addLeaderInfo = false, RecursiveCaller recursiveCaller = RecursiveCaller.None) where T : class
    {
      if (string.IsNullOrEmpty(tag) || entity is null || recursiveCaller == GetCurrentCaller(entity))
      {
        return;
      }
      if (parentTextObject is null)
      {
        MBTextManager.SetTextVariable(tag, GetEntityTextObject(entity));
      }
      else
      {
        parentTextObject.SetTextVariable(tag, GetEntityTextObject(entity));
      }
      SetRelatedProperties(parentTextObject, tag, entity, addLeaderInfo, recursiveCaller);
    }

    private static PluralForm GetRussianPluralFormInternal(int number)
    {
      int absNumner = Math.Abs(number);
      int lastDigit = absNumner % 10;
      return
        RussianPluralExceptions.Contains(absNumner) || !RussianSingularNumerics.Contains(lastDigit)
          ? PluralForm.Plural : !RussianPluralExceptions.Contains(absNumner) && RussianSingularNumerics.Contains(lastDigit) && lastDigit != 1
          ? PluralForm.SpecificSingular : PluralForm.Singular;
    }

    public static PluralForm GetPluralForm(int number)
    {
      if (RussianGroupLanguageIDs.Contains(BannerlordConfig.Language))
      {
        return GetRussianPluralFormInternal(number);
      }
      return number != 1 ? PluralForm.Plural : PluralForm.Singular;
    }

    public static PluralForm GetPluralForm(float number)
    {
      if (RussianGroupLanguageIDs.Contains(BannerlordConfig.Language))
      {
        return GetRussianPluralFormInternal((int)Math.Floor(number));
      }
      return number != 1 ? PluralForm.Plural : PluralForm.Singular;
    }

    public static void SetNumericVariable(TextObject textObject, string tag, int variableValue, string format = null)
    {
      if (string.IsNullOrEmpty(tag))
      {
        return;
      }
      PluralForm pluralForm = GetPluralForm(variableValue);
      var attributes = new Dictionary<string, TextObject>() { [PLURAL_FORM_TAG] = new TextObject(pluralForm == PluralForm.Plural ? 1 : 0), [OTHER_PLURAL_FORM_TAG] = new TextObject(pluralForm != PluralForm.Singular ? 1 : 0) };
      TextObject explainedTextObject = string.IsNullOrEmpty(format) ? new TextObject(variableValue, attributes) : new TextObject(variableValue.ToString(format), attributes);
      if (textObject is null)
      {
        MBTextManager.SetTextVariable(tag, explainedTextObject);
      }
      else
      {
        textObject.SetTextVariable(tag, explainedTextObject);
      }
    }

    public static void SetNumericVariable(TextObject textObject, string tag, float variableValue, string format = null)
    {
      if (string.IsNullOrEmpty(tag))
      {
        return;
      }
      PluralForm pluralForm = GetPluralForm(variableValue);
      var attributes = new Dictionary<string, TextObject>() { [PLURAL_FORM_TAG] = new TextObject(pluralForm == PluralForm.Plural ? 1 : 0), [OTHER_PLURAL_FORM_TAG] = new TextObject(pluralForm != PluralForm.Singular ? 1 : 0) };
      TextObject explainedTextObject = string.IsNullOrEmpty(format) ? new TextObject(variableValue, attributes) : new TextObject(variableValue.ToString(format), attributes);
      if (textObject is null)
      {
        MBTextManager.SetTextVariable(tag, explainedTextObject);
      }
      else
      {
        textObject.SetTextVariable(tag, explainedTextObject);
      }
    }

    public static TextObject GetCooldownText(Type decisionType, float elapsedDaysUntilNow)
    {
      int RemainingDays = (int)Math.Ceiling(AOCooldownManager.GetRequiredDecisionCooldown(decisionType) - elapsedDaysUntilNow);
      PluralForm pluralForm = GetPluralForm(RemainingDays);
      var attributes = new Dictionary<string, TextObject>() { [PLURAL_FORM_TAG] = new TextObject(pluralForm == PluralForm.Plural ? 1 : 0), [OTHER_PLURAL_FORM_TAG] = new TextObject(pluralForm != PluralForm.Singular ? 1 : 0) };
      return new TextObject(DECISION_IS_ON_COOLDOWN, 
                            new Dictionary<string, TextObject>() { ["NUMBER_OF_DAYS"] = new TextObject(RemainingDays, attributes) });
    }

    public enum RecursiveCaller : byte
    {
      None,
      Hero,
      Settlement,
      Clan,
      Kingdom
    }
    public enum PluralForm : byte
    {
      Singular,
      SpecificSingular,
      SpecificPlural,
      Plural
    }
  }
}