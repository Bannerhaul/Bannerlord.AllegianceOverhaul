using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.Library;

namespace AllegianceOverhaul
{
  internal class MessageHelper
  {
    public static void SimpleMessage(string message)
    {
      InformationManager.DisplayMessage(new InformationMessage(message, Colors.Yellow));
    }
  }
}
