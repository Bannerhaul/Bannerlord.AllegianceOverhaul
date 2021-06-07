using TaleWorlds.CampaignSystem;

namespace AllegianceOverhaul.Extensions
{
  public static class CampaignExtensions
  {
    public static AOGameModels? GetAOGameModels(this Campaign campaign)
    {
      return campaign.GameStarted ? AOGameModels.Instance : null;
    }
  }
}
