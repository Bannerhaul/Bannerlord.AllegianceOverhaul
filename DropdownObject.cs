using AllegianceOverhaul.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;

namespace AllegianceOverhaul
{
  public class DropdownObject<T> where T : struct
  {
    public T EnumValue { get; }
    public int Index { get; }
    public DropdownObject(T enumValue, int index)
    {
      if (!typeof(T).IsEnum)
      {
        throw new InvalidOperationException("The struct must be of Enum type!");
      }
      EnumValue = enumValue;
      Index = index;
    }
    public override string ToString() => EnumValue.GetDescription(useLocalizedStrings: true);
    public static IEnumerable<DropdownObject<T>> SetDropdownListFromEnum(bool accountForZeros = true, bool GetAllVariations = true)
    {
      if (!typeof(T).IsEnum)
      {
        throw new InvalidOperationException("The struct must be of Enum type!");
      }
      T enumValue = Enum.GetValues(typeof(T)).Cast<T>().First();
      int idx = -1;
      foreach (T item in typeof(T).IsDefined(typeof(FlagsAttribute), inherit: false) ? (GetAllVariations ? enumValue.GetPossibleVariations(accountForZeros) : enumValue.GetDefinedFlags(accountForZeros)) : enumValue.GetAllItems(accountForZeros))
      {
        idx++;
        yield return new DropdownObject<T>(item, idx);
      }
    }
    public static int GetEnumIndex(T enumValue, bool accountingForZeros = true, bool UsingAllVariations = true)
    {
      if (!typeof(T).IsEnum)
      {
        throw new InvalidOperationException("The struct must be of Enum type!");
      }
      int idx = -1;
      foreach (T item in typeof(T).IsDefined(typeof(FlagsAttribute), inherit: false) ? (UsingAllVariations ? enumValue.GetPossibleVariations(accountingForZeros) : enumValue.GetDefinedFlags(accountingForZeros)) : enumValue.GetAllItems(accountingForZeros))
      {
        idx++;
        if (Convert.ToInt32(item) == Convert.ToInt32(enumValue))
        {
          return idx;
        }
      }
      throw new ArgumentOutOfRangeException(nameof(enumValue), enumValue, "Value not found in dropdown list!");
    }
  }
}
