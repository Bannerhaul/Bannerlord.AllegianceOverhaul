using System;
using System.Reflection;
using TaleWorlds.Localization;

namespace AllegianceOverhaul
{
  internal class DebugHelper
  {
    public static void HandleException(Exception ex, MethodInfo methodInfo, string sectionName)
    {
      MessageHelper.ErrorMessage(string.Format("Allegiance Overhaul - error occured in [{1}]{0} - {2} See details in the mod log.", methodInfo != null ? $" in {methodInfo.Name}" : "", sectionName, ex.Message));
      LoggingHelper.Log(string.Format("Error occured{0} - {1}", methodInfo != null ? $" in {methodInfo}" : "", ex.ToString()), sectionName);
    }

    public static void HandleException(Exception ex, string sectionName, string logMessage, string chatMessage)
    {
      if (chatMessage.Length > 0)
      {
        TextObject textObject = new TextObject(chatMessage);
        textObject.SetTextVariable("SECTION", sectionName);
        textObject.SetTextVariable("EXCEPTION_MESSAGE", ex.Message);
        MessageHelper.ErrorMessage(textObject);
      }
      LoggingHelper.Log(string.Format(logMessage, ex.ToString()), sectionName);
    }
  }
}
