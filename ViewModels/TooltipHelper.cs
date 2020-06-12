using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem;
using AllegianceOverhaul.Extensions;

namespace AllegianceOverhaul.ViewModels
{
  internal class TooltipHelper
  {
    public static Color DefaultTooltipColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    //1gqZN4op,eABKeDAv,CnfTEQxp,m7kD2tc5,hIPaEV34,vTefFHTT,jqY89njY,xduBOxJW
    private const string TooltipRelationHeader = "{=hlIYCDRA}Relation with {HERO}";
    private const string TooltipLoyaltyHeader = "{=AuJasuJQ}Loyalty";

    public static string GetTooltipRelationHeader (Hero hero)
    {
      if (hero is null)
        return string.Empty;
      TextObject textObject = new TextObject(TooltipRelationHeader);
      textObject.SetTextVariable("HERO", hero.Name);
      return textObject.ToString();
    }
    public static string GetTooltipLoyaltyHeader()
    {
      return TooltipLoyaltyHeader.ToLocalizedString();
    }
  }
}
