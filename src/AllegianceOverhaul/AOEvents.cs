using TaleWorlds.CampaignSystem;

namespace AllegianceOverhaul
{
    public class AOEvents
    {
        private readonly MbEvent _playerGotJoinRequest = new();

        public static AOEvents? Instance { get; internal set; }

        public static IMbEvent OnPlayerGotJoinRequestEvent => Instance!._playerGotJoinRequest;
        internal void OnPlayerGotJoinRequest()
        {
            _playerGotJoinRequest.Invoke();
        }
    }
}