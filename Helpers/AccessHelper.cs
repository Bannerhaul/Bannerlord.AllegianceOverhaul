using System;
using System.Reflection;
using static HarmonyLib.AccessTools;

namespace AllegianceOverhaul.Helpers
{
  public static class AccessHelper
  {
    public static TDelegate GetDelegate<TDelegate, TInstance>(TInstance instance, string originalMethod) where TDelegate : Delegate where TInstance : class
    {
      return GetDelegate<TDelegate>(instance.GetType(), originalMethod);
    }

    public static TDelegate GetDelegate<TDelegate>(Type type, string originalMethod) where TDelegate : Delegate
    {
      MethodInfo miOriginal = Method(type, originalMethod);
      return miOriginal != null ? (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), miOriginal) : null;
    }

    public static TFieldType GetFieldValue<TFieldType, TInstance>(TInstance instance, string fieldName)
    {
      return (TFieldType)Field(instance.GetType(), fieldName).GetValue(instance);
    }
  }
}
