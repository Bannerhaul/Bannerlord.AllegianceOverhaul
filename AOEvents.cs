using TaleWorlds.CampaignSystem;
using AllegianceOverhaul.Helpers;
using AllegianceOverhaul.SavableClasses;

namespace AllegianceOverhaul
{
  public class AOEvents
  {
    internal static class MbEvent2InvokeHandler<T1, T2>
    {
      internal delegate void InvokeDelegate(MbEvent<T1, T2> instance, T1 t1, T2 t2);
      internal static readonly InvokeDelegate deInvoke = AccessHelper.GetDelegate<InvokeDelegate>(typeof(MbEvent<T1, T2>), "Invoke");
    }
    internal static class MbEvent3InvokeHandler<T1, T2, T3>
    {
      internal delegate void InvokeDelegate(MbEvent<T1, T2, T3> instance, T1 t1, T2 t2, T3 t3);
      internal static readonly InvokeDelegate deInvoke = AccessHelper.GetDelegate<InvokeDelegate>(typeof(MbEvent<T1, T2, T3>), "Invoke");
    }
    internal static class MbEvent4InvokeHandler<T1, T2, T3, T4>
    {
      internal delegate void InvokeDelegate(MbEvent<T1, T2, T3, T4> instance, T1 t1, T2 t2, T3 t3, T4 t4);
      internal static readonly InvokeDelegate deInvoke = AccessHelper.GetDelegate<InvokeDelegate>(typeof(MbEvent<T1, T2, T3, T4>), "Invoke");
    }

    private readonly MbEvent<Hero, Hero, SegmentalFractionalScore> _relationShift = new MbEvent<Hero, Hero, SegmentalFractionalScore>();
    private readonly MbEvent<Hero, Hero, SegmentalFractionalScore, string> _relationShifted = new MbEvent<Hero, Hero, SegmentalFractionalScore, string>();

    public static AOEvents Instance { get; internal set; }

    public static IMbEvent<Hero, Hero, SegmentalFractionalScore> OnRelationShiftEvent => Instance._relationShift;
    internal void OnRelationShift(Hero baseHero, Hero otherHero, SegmentalFractionalScore fractionalScore)
    {
      MbEvent3InvokeHandler<Hero, Hero, SegmentalFractionalScore>.deInvoke(Instance._relationShift, baseHero, otherHero, fractionalScore);
    }

    public static IMbEvent<Hero, Hero, SegmentalFractionalScore, string> OnRelationShiftedEvent => Instance._relationShifted;
    internal void OnRelationShifted(Hero baseHero, Hero otherHero, SegmentalFractionalScore fractionalScore, string explanation)
    {
      MbEvent4InvokeHandler<Hero, Hero, SegmentalFractionalScore, string>.deInvoke(Instance._relationShifted, baseHero, otherHero, fractionalScore, explanation);
    }
  }
}