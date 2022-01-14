using AllegianceOverhaul.Extensions;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace AllegianceOverhaul.ViewModels
{
    internal class TooltipHelper
    {
        public static Color DefaultTooltipColor = new(0.0f, 0.0f, 0.0f, 0.0f);
        private const string TooltipRelationHeader = "{=YCgmy4haM}Relation with {HERO}";
        private const string TooltipLoyaltyHeader = "{=YJmaYz7il}Loyalty";

        public static string GetTooltipRelationHeader(Hero hero)
        {
            if (hero is null)
                return string.Empty;
            TextObject textObject = new(TooltipRelationHeader);
            textObject.SetTextVariable("HERO", hero.Name);
            return textObject.ToString();
        }
        public static string GetTooltipLoyaltyHeader()
        {
            return TooltipLoyaltyHeader.ToLocalizedString();
        }
    }
}