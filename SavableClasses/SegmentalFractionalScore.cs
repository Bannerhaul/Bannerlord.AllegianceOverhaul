using System;

using TaleWorlds.SaveSystem;

namespace AllegianceOverhaul.SavableClasses
{
  //[SaveableStruct(901)]
  public struct SegmentalFractionalScore
  {
    [SaveableProperty(1)]
    public double PositiveScore { get; private set; }
    [SaveableProperty(2)]
    public double NegativeScore { get; private set; }

    public SegmentalFractionalScore(double positiveScore, double negativeScore)
    {
      PositiveScore = positiveScore;
      NegativeScore = negativeScore;
    }

    public bool ReadyToExtract() => (PositiveScore >= 1 || NegativeScore >= 1) && (Math.Floor(PositiveScore) != Math.Floor(NegativeScore));

    public SegmentalFractionalScore ExtractWholeParts(out SegmentalFractionalScore integerScrore)
    {
      integerScrore = new SegmentalFractionalScore(Math.Floor(Math.Max(0, PositiveScore)), Math.Floor(Math.Max(0, NegativeScore)));
      return this - integerScrore;
    }

    public override string ToString() => string.Format("({0}, {1})", PositiveScore.ToString("N0"), NegativeScore.ToString("N0"));
    public override bool Equals(object obj) => obj is SegmentalFractionalScore segmentalFractionalScore
                                               && PositiveScore == segmentalFractionalScore.PositiveScore
                                               && NegativeScore == segmentalFractionalScore.NegativeScore;
    public bool Equals(SegmentalFractionalScore obj) => PositiveScore == obj.PositiveScore
                                                        && NegativeScore == obj.NegativeScore;
    public override int GetHashCode()
    {
      return PositiveScore.GetHashCode() ^ NegativeScore.GetHashCode();
    }
    public static SegmentalFractionalScore operator +(SegmentalFractionalScore a) => a;
    public static SegmentalFractionalScore operator -(SegmentalFractionalScore a) => new SegmentalFractionalScore(-a.PositiveScore, -a.NegativeScore);
    public static SegmentalFractionalScore operator +(SegmentalFractionalScore a, SegmentalFractionalScore b) => new SegmentalFractionalScore(a.PositiveScore + b.PositiveScore, a.NegativeScore + b.NegativeScore);
    public static SegmentalFractionalScore operator -(SegmentalFractionalScore a, SegmentalFractionalScore b) => a + (-b);
    public static SegmentalFractionalScore operator *(SegmentalFractionalScore a, SegmentalFractionalScore b) => new SegmentalFractionalScore(a.PositiveScore * b.PositiveScore, a.NegativeScore * b.NegativeScore);
    public static SegmentalFractionalScore operator /(SegmentalFractionalScore a, SegmentalFractionalScore b)
    {
      if (b.PositiveScore == 0 || b.NegativeScore == 0)
      {
        throw new DivideByZeroException();
      }
      return new SegmentalFractionalScore(a.PositiveScore / b.PositiveScore, a.NegativeScore / b.NegativeScore);
    }
  }
}
