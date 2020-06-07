using System;
using System.Reflection;

namespace AllegianceOverhaul
{
  internal class DebugHelper
  {
    public static void HandleException(Exception ex, MethodInfo methodInfo, string section)
    {
      MessageHelper.ErrorMessage(string.Format("Allegiance Overhaul - error occured in [{1}]{0} - {2} See details in the mod log.", (methodInfo != null ? $" in {methodInfo.Name}" : ""), section, ex.Message));
      LoggingHelper.Log(string.Format("Error occured{0} - {1}", (methodInfo != null ? $" in {methodInfo}" : ""), ex.ToString()), section);
    }
  }
}
