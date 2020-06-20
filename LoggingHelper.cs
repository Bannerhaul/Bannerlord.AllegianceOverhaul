using System;
using System.IO;
using TaleWorlds.Library;

namespace AllegianceOverhaul
{
  internal static class LoggingHelper
  {
    public static readonly string AOLogFile = Path.Combine(BasePath.Name, "Modules", "AllegianceOverhaul", "AllegianceOverhaul.log");
    public static void Log(string message)
    {
      lock (AOLogFile)
      {
        using (StreamWriter streamWriter = File.AppendText(AOLogFile))
        {
          streamWriter.WriteLine(message);
        }
      }
    }
    public static void Log(string message, string sectionName)
    {
      lock (AOLogFile)
      {
        using (StreamWriter streamWriter = File.AppendText(AOLogFile))
        {
          streamWriter.WriteLine($"[{DateTime.Now:dd.MM.yyyy HH:mm:ss}] - {sectionName}.\n{message}");
        }
      }
    }
  }
}
