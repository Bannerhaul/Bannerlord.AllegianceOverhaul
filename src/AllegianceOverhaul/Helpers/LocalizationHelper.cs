using AllegianceOverhaul.Extensions;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace AllegianceOverhaul.Helpers
{
    public static class LocalizationHelper
    {
        public const string PLURAL_FORM_TAG = "PLURAL_FORM";
        public const string SPECIFIC_SINGULAR_FORM_TAG = "SPECIFIC_SINGULAR_FORM";
        public const string SPECIFIC_PLURAL_FORM_TAG = "SPECIFIC_PLURAL_FORM";

        private static readonly ReadOnlyCollection<int> EasternSlavicPluralExceptions = new(new List<int>() { 11, 12, 13, 14 });
        private static readonly ReadOnlyCollection<int> EasternSlavicSingularNumerics = new(new List<int>() { 1, 2, 3, 4 });

        private static readonly ReadOnlyCollection<int> WesternSlavicPluralExceptions = new(new List<int>() { 12, 13, 14 });
        private static readonly ReadOnlyCollection<int> WesternSlavicSingularNumerics = new(new List<int>() { 2, 3, 4 });

        private static readonly ReadOnlyCollection<string> EasternSlavicGroupLanguageIDs = new(new List<string>() { "Russian", "Русский", "Ukrainian", "Українська", "Belarusian", "Беларускі" });
        private static readonly ReadOnlyCollection<string> WesternSlavicGroupLanguageIDs = new(new List<string>() { "Polish", "Polski" });

        private static RecursiveCaller GetRecursiveCaller(RecursiveCaller currentCaller, RecursiveCaller receivedCaller)
        {
            return (RecursiveCaller) Math.Max((byte) currentCaller, (byte) receivedCaller);
        }

        private static RecursiveCaller GetCurrentCaller<T>(T entity) where T : class
        {
            return entity switch
            {
                Hero _ => RecursiveCaller.Hero,
                Settlement _ => RecursiveCaller.Settlement,
                Clan _ => RecursiveCaller.Clan,
                Kingdom _ => RecursiveCaller.Kingdom,
                _ => throw new ArgumentException(string.Format("{0} is not supported type", entity.GetType().FullName), nameof(entity)),
            };
        }

        private static TextObject GetEntityTextObject<T>(T entity) where T : class
        {
            switch (entity)
            {
                case Hero hero:
                    TextObject characterProperties = new();
                    characterProperties.SetTextVariable("NAME", hero.Name);
                    characterProperties.SetTextVariable("AGE", (int) hero.Age);
                    characterProperties.SetTextVariable("GENDER", hero.IsFemale ? 1 : 0);
                    characterProperties.SetTextVariable("LINK", hero.EncyclopediaLinkWithName);
                    characterProperties.SetTextVariable("FIRSTNAME", hero.FirstName ?? hero.Name);
                    return characterProperties;
                case Settlement settlement:
                    TextObject settlementProperties = new();
                    settlementProperties.SetTextVariable("NAME", settlement.Name);
                    settlementProperties.SetTextVariable("IS_TOWN", settlement.IsTown ? 1 : 0);
                    settlementProperties.SetTextVariable("IS_CASTLE", settlement.IsCastle ? 1 : 0);
                    settlementProperties.SetTextVariable("IS_VILLAGE", settlement.IsVillage ? 1 : 0);
                    settlementProperties.SetTextVariable("LINK", settlement.EncyclopediaLinkWithName);
                    return settlementProperties;
                case Clan clan:
                    TextObject clanProperties = new();
                    clanProperties.SetTextVariable("NAME", clan.Name);
                    clanProperties.SetTextVariable("MINOR_FACTION", clan.IsMinorFaction ? 1 : 0);
                    clanProperties.SetTextVariable("UNDER_CONTRACT", clan.IsUnderMercenaryService ? 1 : 0);
                    clanProperties.SetTextVariable("IS_MERCENARY", clan.IsMercenary() ? 1 : 0);
                    clanProperties.SetTextVariable("LINK", clan.EncyclopediaLinkWithName);
                    return clanProperties;
                case Kingdom kingdom:
                    TextObject kingdomProperties = new();
                    kingdomProperties.SetTextVariable("NAME", kingdom.Name);
                    kingdomProperties.SetTextVariable("LINK", kingdom.EncyclopediaLinkWithName);
                    return kingdomProperties;
                default:
                    throw new ArgumentException(string.Format("{0} is not supported type", entity.GetType().FullName), nameof(entity));
            }
        }

        private static void SetRelatedProperties<T>(TextObject? parentTextObject, string tag, T entity, bool addLeaderInfo, RecursiveCaller recursiveCaller) where T : class
        {
            switch (entity)
            {
                case Hero hero:
                    SetEntityProperties(parentTextObject, tag + "_CLAN", hero.Clan, addLeaderInfo, GetRecursiveCaller(RecursiveCaller.Hero, recursiveCaller));
                    break;
                case Settlement settlement:
                    SetEntityProperties(parentTextObject, tag + "_CLAN", settlement.OwnerClan, addLeaderInfo, GetRecursiveCaller(RecursiveCaller.Settlement, recursiveCaller));
                    break;
                case Clan clan:
                    if (addLeaderInfo)
                    {
                        SetEntityProperties(parentTextObject, tag + "_LEADER", clan.Leader, false, RecursiveCaller.Clan);
                    }
                    if (clan.Kingdom != null)
                    {
                        SetEntityProperties(parentTextObject, tag + "_KINGDOM", clan.Kingdom, addLeaderInfo, GetRecursiveCaller(RecursiveCaller.Clan, recursiveCaller));
                    }
                    break;
                case Kingdom kingdom:
                    if (addLeaderInfo)
                    {
                        SetEntityProperties(parentTextObject, tag + "_LEADER", kingdom.Leader, false, RecursiveCaller.Kingdom);
                    }
                    break;
                default:
                    throw new ArgumentException(string.Format("{0} is not supported type", entity.GetType().FullName), nameof(entity));
            }
        }

        public static void SetEntityProperties<T>(TextObject? parentTextObject, string tag, T? entity, bool addLeaderInfo = false, RecursiveCaller recursiveCaller = RecursiveCaller.None) where T : class
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

        private static PluralForm GetEasternSlavicPluralFormInternal(int number)
        {
            int absNumber = Math.Abs(number);
            int lastDigit = absNumber % 10;
            return
              EasternSlavicPluralExceptions.Contains(absNumber % 100) || !EasternSlavicSingularNumerics.Contains(lastDigit)
                ? PluralForm.Plural
                : !EasternSlavicPluralExceptions.Contains(absNumber) && EasternSlavicSingularNumerics.Contains(lastDigit) && lastDigit != 1
                ? PluralForm.SpecificSingular : PluralForm.Singular;
        }

        private static PluralForm GetWesternSlavicPluralFormInternal(int number)
        {
            int absNumber = Math.Abs(number);
            int lastDigit = absNumber % 10;
            return
              absNumber > 1 && (WesternSlavicPluralExceptions.Contains(absNumber % 100) || !WesternSlavicSingularNumerics.Contains(lastDigit))
                ? PluralForm.Plural
                : !WesternSlavicPluralExceptions.Contains(absNumber) && WesternSlavicSingularNumerics.Contains(lastDigit)
                ? PluralForm.SpecificPlural : PluralForm.Singular;
        }

        public static PluralForm GetPluralForm(int number)
        {
            if (EasternSlavicGroupLanguageIDs.Contains(BannerlordConfig.Language))
            {
                return GetEasternSlavicPluralFormInternal(number);
            }
            if (WesternSlavicGroupLanguageIDs.Contains(BannerlordConfig.Language))
            {
                return GetWesternSlavicPluralFormInternal(number);
            }
            return number != 1 ? PluralForm.Plural : PluralForm.Singular;
        }

        public static PluralForm GetPluralForm(float number)
        {
            if (EasternSlavicGroupLanguageIDs.Contains(BannerlordConfig.Language))
            {
                return GetEasternSlavicPluralFormInternal((int) Math.Floor(number));
            }
            return number != 1 ? PluralForm.Plural : PluralForm.Singular;
        }

        private static Dictionary<string, object> GetPluralFormAttributes(PluralForm pluralForm) =>
            new()
            {
                [PLURAL_FORM_TAG] = new TextObject(pluralForm == PluralForm.Plural ? 1 : 0),
                [SPECIFIC_PLURAL_FORM_TAG] = new TextObject(pluralForm == PluralForm.SpecificPlural ? 1 : 0),
                [SPECIFIC_SINGULAR_FORM_TAG] = new TextObject(pluralForm == PluralForm.SpecificSingular ? 1 : 0)
            };

        public static void SetNumericVariable(TextObject textObject, string tag, int variableValue, string? format = null)
        {
            if (string.IsNullOrEmpty(tag))
            {
                return;
            }
            var attributes = GetPluralFormAttributes(GetPluralForm(variableValue));
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

        public static void SetNumericVariable(TextObject textObject, string tag, float variableValue, string? format = null)
        {
            if (string.IsNullOrEmpty(tag))
            {
                return;
            }
            var attributes = GetPluralFormAttributes(GetPluralForm(variableValue));
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