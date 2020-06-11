using TaleWorlds.Localization;

namespace AllegianceOverhaul.Extensions
{
  internal static class StringExtension
  {
    public static string ToLocalizedString(this string String)
    {
      return new TextObject(String).ToString();
    }
  }
}
