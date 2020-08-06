using TaleWorlds.ObjectSystem;

namespace AllegianceOverhaul.Helpers
{
  public static class ElegantPairHelper
  {
    public static long Pair(int a, int b)
    {
      ulong A = (ulong)(a >= 0 ? 2 * (long)a : -2 * (long)a - 1);
      ulong B = (ulong)(b >= 0 ? 2 * (long)b : -2 * (long)b - 1);
      long C = (long)((A >= B ? A * A + A + B : A + B * B) / 2);
      return a < 0 && b < 0 || a >= 0 && b >= 0 ? C : -C - 1;
    }

    public static ulong Pair(uint a, uint b)
    {
      ulong A = a;
      ulong B = b;
      return A >= B ? A * A + A + B : A + B * B;
    }

    public static int Pair(short a, short b)
    {
      var A = (uint)(a >= 0 ? 2 * a : -2 * a - 1);
      var B = (uint)(b >= 0 ? 2 * b : -2 * b - 1);
      var C = (int)((A >= B ? A * A + A + B : A + B * B) / 2);
      return a < 0 && b < 0 || a >= 0 && b >= 0 ? C : -C - 1;
    }

    public static uint Pair(ushort a, ushort b)
    {
      uint A = a;
      uint B = b;
      return A >= B ? A * A + A + B : A + B * B;
    }

    public static ulong Pair(MBGUID a, MBGUID b)
    {
      return Pair(a.InternalValue, b.InternalValue);
    }

    public static (uint a, uint b) UnPair(ulong pairValue)
    {
      ulong sqrt = IntegerSqrt(pairValue);
      ulong remainder = pairValue - sqrt * sqrt;
      return remainder < sqrt ? ((uint)remainder, (uint)sqrt) : ((uint)sqrt, (uint)(remainder - sqrt));
    }

    public static (int a, int b) UnPair(long pairValue)
    {
      long z = pairValue >= 0 ? pairValue : -pairValue - 1;
      ulong Z = 2 * (ulong)z;

      (uint A, uint B) uRes = UnPair(Z);
      (int a, int b) firstPossibleResult = GetSignedTuple(uRes);

      return Pair(firstPossibleResult.a, firstPossibleResult.b) == pairValue ? firstPossibleResult : GetSignedTuple(UnPair(Z + 1));
    }

    public static (ushort a, ushort b) UnPair(uint pairValue)
    {
      uint sqrt = IntegerSqrt(pairValue);
      uint remainder = pairValue - sqrt * sqrt;
      return remainder < sqrt ? ((ushort)remainder, (ushort)sqrt) : ((ushort)sqrt, (ushort)(remainder - sqrt));
    }

    public static (short a, short b) UnPair(int pairValue)
    {
      int z = pairValue >= 0 ? pairValue : -pairValue - 1;
      uint Z = 2 * (uint)z;

      (ushort A, ushort B) uRes = UnPair(Z);
      (short a, short b) firstPossibleResult = ((short a, short b))GetSignedTuple(uRes);

      return Pair(firstPossibleResult.a, firstPossibleResult.b) == pairValue ? firstPossibleResult : ((short a, short b))GetSignedTuple(UnPair(Z + 1));
    }

    private static ulong IntegerSqrt(ulong a)
    {
      ulong min = 0;
      ulong max = ((ulong)1) << 32;
      while (true)
      {
        if (max <= 1 + min)
        {
          return min;
        }

        ulong sqt = min + (max - min) / 2;
        ulong sq = sqt * sqt;

        if (sq == a)
        {
          return sqt;
        }

        if (sq > a)
        {
          max = sqt;
        }
        else
        {
          min = sqt;
        }
      }
    }

    private static uint IntegerSqrt(uint a)
    {
      uint min = 0;
      uint max = ((uint)1) << 16;
      while (true)
      {
        if (max <= 1 + min)
        {
          return min;
        }

        uint sqt = min + (max - min) / 2;
        uint sq = sqt * sqt;

        if (sq == a)
        {
          return sqt;
        }

        if (sq > a)
        {
          max = sqt;
        }
        else
        {
          min = sqt;
        }
      }
    }

    private static (int a, int b) GetSignedTuple((uint A, uint B) unsignedTuple)
    {
      return (a: unsignedTuple.A % 2 == 0 ? (int)unsignedTuple.A / 2 : (int)((unsignedTuple.A + 1) / -2), b: unsignedTuple.B % 2 == 0 ? (int)unsignedTuple.B / 2 : (int)((unsignedTuple.B + 1) / -2));
    }
  }
}
