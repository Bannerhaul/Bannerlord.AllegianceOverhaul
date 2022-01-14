using TaleWorlds.CampaignSystem;

namespace AllegianceOverhaul.Extensions
{
    public static class ClanExtensions
    {
        public static bool IsMercenary(this Clan clan)
        {
            return clan.Kingdom is null ? clan.IsMinorFaction : clan.IsUnderMercenaryService;
        }
        public static bool IsRulingClan(this Clan clan)
        {
            return clan.Kingdom?.RulingClan == clan;
        }
    }
}