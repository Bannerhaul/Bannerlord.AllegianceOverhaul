using HarmonyLib;

namespace AllegianceOverhaul.Extensions.Harmony
{
  public static class PatchExtension
  {
    public static string GetDebugString(this Patch patch)
    {
      return $"\t\tIndex: {patch.index}\n\t\tOwner: {patch.owner}\n\t\tPatch method: {patch.PatchMethod}\n\t\tPriority: {patch.priority}\n\t\tBefore: {patch.before}\n\t\tAfter: {patch.after}";
    }
  }
}
