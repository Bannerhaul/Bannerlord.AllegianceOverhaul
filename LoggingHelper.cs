using System;
using System.IO;
using TaleWorlds.Library;

namespace AllegianceOverhaul
{
  internal static class LoggingHelper
  {
    public static readonly string AOLogFile = Path.Combine(BasePath.Name, "Modules", "AllegianceOverhaul", "AllegianceOverhaul.log");
    public static void Log(string Message)
    {
      lock (AOLogFile)
      {
        using (StreamWriter streamWriter = File.AppendText(AOLogFile))
          streamWriter.WriteLine(Message);
      }
    }
    public static void Log(string Message, string Section)
    {
      lock (AOLogFile)
      {
        using (StreamWriter streamWriter = File.AppendText(AOLogFile))
          streamWriter.WriteLine($"[{DateTime.Now:dd.MM.yyyy HH:mm:ss}] - {Section}.\n{Message}");
      }
    }
  }
}
