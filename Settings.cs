using System;
using System.Collections.Generic;
using MCM.Abstractions.Data;
using MCM.Abstractions.Settings.Base;
using MCM.Abstractions.Settings.Base.Global;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using TaleWorlds.Localization;

namespace AllegianceOverhaul
{
  class Settings : AttributeGlobalSettings<Settings>
  {
    public override string Id => "AllegianceOverhaul_v1";
    public override string DisplayName => $"{new TextObject("{=qfpqfAdz}Allegiance Overhaul")} {typeof(Settings).Assembly.GetName().Version.ToString(3)}";
    public override string FolderName => "Allegiance Overhaul";
    public override string Format => "json";

    //Headings
    private const string HeadingGeneral = "{=yRuHl3O6}General settings";
    //private const string HeadingGeneralSub = "/{=yRuHl3O6}General settings";
    private const string HeadingDebug = "{=fmUHcBME}Debug settings";
    //private const string HeadingDebugSub = "/{=fmUHcBME}Debug settings";

    private const string HeadingEnsuredLoyalty = "{=Ps6RgRH1}Ensured loyalty";
    private const string HeadingEnsuredLoyaltyByRelation = HeadingEnsuredLoyalty + "/{=mmUgw8hL}Achieve via relation";
    private const string HeadingEnsuredLoyaltyByContext = HeadingEnsuredLoyaltyByRelation + "/{=m5JmQwFt}Modify by situational context";
    private const string HeadingEnsuredLoyaltyByHonor = HeadingEnsuredLoyaltyByRelation + "/{=eExUFCCQ}Modify by honor level";
    private const string HeadingEnsuredLoyaltyPrice = HeadingEnsuredLoyaltyByRelation + "/{=uXwgU2WE}Withhold price";
    private const string HeadingEnsuredLoyaltyBribe = HeadingEnsuredLoyaltyPrice + "/{=rhvq8Yhb}Bribing";

    //Reused settings, hints and values
    private const string DropdownValueAll = "{=COahS2f6}All kingdoms";
    private const string DropdownValuePlayers = "{=JeehGy9z}Player's kingdom";
    private const string DropdownValueRuled = "{=v2TacaMr}Kingdom ruled by player";

    //Ensured loyalty
    [SettingPropertyBool("{=Ps6RgRH1}Ensured loyalty", RequireRestart = true, HintText = "{=QLii7mIG}Enables specifying conditions for clans to become unreservedly loyal to kingdoms.")]
    [SettingPropertyGroup(HeadingEnsuredLoyalty, IsMainToggle = true)]
    public bool UseEnsuredLoyalty { get; set; } = false;

    [SettingPropertyDropdown("{=yul4vp54}Applies to", RequireRestart = false, HintText = "{=y3ClkEcy}Specify if below rules should affect all kingdoms, or just the player's one. Default is [All kingdoms].")]
    [SettingPropertyGroup(HeadingEnsuredLoyalty)]
    public DefaultDropdown<string> EnsuredLoyaltyScope { get; set; } = new DefaultDropdown<string>(new string[]
    {
      DropdownValueAll,
      DropdownValuePlayers,
      DropdownValueRuled
    }, 0);

    [SettingPropertyInteger("{=MIDoz9om}Faction oath of fealty limitation period", 0, 420, Order = 0, RequireRestart = false, HintText = "{=l5LWFlHv}Period in days after joining a kingdom, during which clan would not even consider leaving that kingdom. Default = 84 (a game year).")]
    [SettingPropertyGroup(HeadingEnsuredLoyalty)]
    public int FactionOathPeriod { get; set; } = 84;    
    [SettingPropertyInteger("{=Az4K6FiF}Minor faction oath of fealty limitation period", 0, 420, Order = 1, RequireRestart = false, HintText = "{=qRnwZFts}Period in days after joining a kingdom, during which minor faction would not even consider leaving that kingdom. Default = 63 (three quarters of a game year).")]
    [SettingPropertyGroup(HeadingEnsuredLoyalty)]
    public int MinorFactionOathPeriod { get; set; } = 63;
    [SettingPropertyInteger("{=eMMKVjRU}Minimum mercenary service period", 0, 420, Order = 2, RequireRestart = false, HintText = "{=00arXPdi}Period in days after initiating mercenary service of kingdom, during which minor faction would not even consider leaving that kingdom. Default = 42 (half a game year).")]
    [SettingPropertyGroup(HeadingEnsuredLoyalty)]
    public int MinorFactionServicePeriod { get; set; } = 42;
    [SettingPropertyBool("{=Fxyb9iPM}Affect player conversations", Order = 3, RequireRestart = true, HintText = "{=ZaPJhkuk}Specify if ensured loyalty system should affect player conversations, making impossible to reqruit lords that are loyal to ther lieges.")]
    [SettingPropertyGroup(HeadingEnsuredLoyalty)]
    public bool UseLoyaltyInConversations { get; set; } = true;

    //Ensured loyalty via relation
    [SettingPropertyBool("{=mmUgw8hL}Achieve via relation",  RequireRestart = false, HintText = "{=gj3cKWJo}Specify if reaching certain relation level with kingdom leader should make clan unreservedly loyal to that kingdom.")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyByRelation, GroupOrder = 0, IsMainToggle = true)]
    public bool UseRelationForEnsuredLoyalty { get; set; } = false;

    [SettingPropertyInteger("{=JvWw6eSp}Ensured loyalty baseline", -100, 100, RequireRestart = false, HintText = "{=XP7qtIkt}The minimum required relationship a clan leader must have with a kingdom leader in order for them to never leave that kingdom. Being below that threshold does NOT mean clan will aitomatically leave. Can serve as a baseline for other togglable modifiers. Default = 50.")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyByRelation)]
    public int EnsuredLoyaltyBaseline { get; set; } = 50;

    //Ensured loyalty via relation - situational context
    [SettingPropertyBool("{=m5JmQwFt}Modify by situational context", RequireRestart = false, HintText = "{=0oeDqVTM}Specify if situational context should affect minimum required relationship, at which loyalty is ensured.")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyByContext, GroupOrder = 0, IsMainToggle = true)]
    public bool UseContextForEnsuredLoyalty { get; set; } = false;

    [SettingPropertyInteger("{=AOnhwhiK}Blood relation modifier", 0, 50, Order = 0, RequireRestart = false, HintText = "{=gI3AplJv}Flat value that will be deducted from baseline if there is a kinship with the governing clan. The same value will be added to the baseline if clan considering defecting to a kingdom, ruled by kinsman. Default = 30 (blood relatives are loyal to and tend to join their kinsfolk).")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyByContext)]
    public int BloodRelativesEnsuredLoyaltyModifier { get; set; } = 30;
    [SettingPropertyInteger("{=Rbo96zKF}Minor faction modifier", 0, 50, Order = 1, RequireRestart = false, HintText = "{=BFnZrVGK}Flat value that will be added to baseline if minor faction considering leaving its kingdom. Default = 10 (minor factions tend to be less loyal).")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyByContext)]
    public int MinorFactionEnsuredLoyaltyModifier { get; set; } = 10;
    [SettingPropertyInteger("{=Nff6KlEn}Defection modifier", -50, 50, Order = 2, RequireRestart = false, HintText = "{=0B87ROVp}Flat value that will be added to baseline if clan considering not just leaving, but defecting to another kingdom. Default = 0.")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyByContext)]
    public int DefectionEnsuredLoyaltyModifier { get; set; } = 0;
    [SettingPropertyInteger("{=5bJx7aO7}Landless clan modifier", 0, 100, Order = 3, RequireRestart = false, HintText = "{=QRmeDMie}Flat value that will be added to baseline if clan owns no land. Default = 20 (Landless clans tend to be less loyal).")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyByContext)]
    public int LandlessClanEnsuredLoyaltyModifier { get; set; } = 20;
    [SettingPropertyInteger("{=rHn5IY0P}Landless kingdom modifier", 0, 100, Order = 4, RequireRestart = false, HintText = "{=vSD05nAh}Flat value that will be added to baseline of all clans of the kingdom, except the ruling one, if kingdom owns no land. Default = 50 (none but most honorable vassals would tolerate this).")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyByContext)]
    public int LandlessKingdomEnsuredLoyaltyModifier { get; set; } = 50;

    //Ensured loyalty via relation - honor
    [SettingPropertyBool("{=eExUFCCQ}Modify by honor level", RequireRestart = false, HintText = "{=gIDkdZP8}Specify if clan leader's honor should affect minimum required relationship, at which loyalty is ensured.")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyByHonor, GroupOrder = 1, IsMainToggle = true)]
    public bool UseHonorForEnsuredLoyalty { get; set; } = false;

    [SettingPropertyInteger("{=1QhZZBVA}High honor step when leaving", 0, 30, Order = 0, RequireRestart = false, HintText = "{=SJ2OA4HM}Flat value that will be deducted from baseline for each positive honor level of a clan leader when considering leaving kingdom (positive honor makes leaders loyal at lower relation). Default = 5.")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyByHonor)]
    public int PositiveHonorEnsuredLoyaltyModifier_Leaving { get; set; } = 5;
    [SettingPropertyInteger("{=QJFQ5eSy}Low honor step when leaving", 0, 30, Order = 1, RequireRestart = false, HintText = "{=bp2jDgN4}Flat value that will be added to baseline for each negative honor level of a clan leader when considering leaving kingdom (negative honor requires higher relation for leaders to become loyal). Default = 5.")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyByHonor)]
    public int NegativeHonorEnsuredLoyaltyModifier_Leaving { get; set; } = 5;
    [SettingPropertyInteger("{=Flo6n1rf}High honor step when defecting", 0, 30, Order = 2, RequireRestart = false, HintText = "{=6TOUjrrQ}Flat value that will be deducted from baseline for each positive honor level of a clan leader when considering defection from kingdom (positive honor makes leaders loyal at lower relation). Default = 15.")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyByHonor)]
    public int PositiveHonorEnsuredLoyaltyModifier_Defecting { get; set; } = 15;
    [SettingPropertyInteger("{=ZZxrdAfn}Low honor step when defecting", 0, 30, Order = 3, RequireRestart = false, HintText = "{=rNiMWcE4}Flat value that will be added to baseline for each negative honor level of a clan leader when considering defection from kingdom (negative honor requires higher relation for leaders to become loyal). Default = 20.")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyByHonor)]
    public int NegativeHonorEnsuredLoyaltyModifier_Defecting { get; set; } = 20;

    //Ensured loyalty via relation - withhold price
    [SettingPropertyBool("{=uXwgU2WE}Withhold price", RequireRestart = false, HintText = "{=BEiLWxeU}Specify if reaching certain relation level between clan leader and kingdom leader does not guarantee loyalty per se, but instead gives kingdom leader an option to withhold the clan that wishes to leave, using influence and money.")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyPrice, GroupOrder = 2, IsMainToggle = true)]
    public bool UseWithholdPrice { get; set; } = false;

    [SettingPropertyFloatingInteger("{=bZH00rVn}Tolerance limit (millions)", 0.1f, 5f, Order = 0, RequireRestart = false, HintText = "{=moc7wRwB}Maximum amount, measured in millions, by which score of clan to leave kingdom may safely exceed threshold, not arising the need for ruler to intervene. Default = 0.5.")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyPrice)]
    public float WithholdToleranceLimit { get; set; } = 0.5f;
    [SettingPropertyFloatingInteger("{=siPJey6u}Influence cost multiplier", 1f, 5f, Order = 1, RequireRestart = false, HintText = "{=Tei4s42W}Multiplier for calculated influence cost to withhold the clan. Default = 2.5.")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyPrice)]
    public float WithholdInfluenceMultiplier { get; set; } = 2.5f;

    [SettingPropertyBool("{=rhvq8Yhb}Bribing", RequireRestart = false, HintText = "{=rJw4D3kJ}Specify if withholding the clan should cost money in addition to influence.")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyBribe, GroupOrder = 3, IsMainToggle = true)]
    public bool UseWithholdBribing { get; set; } = false;

    [SettingPropertyFloatingInteger("{=cM9saCDB}Tolerance limit for bribing (millions)", 0.1f, 10f, Order = 3, RequireRestart = false, HintText = "{=tCqdcDuR}Maximum amount, measured in millions, by which score of clan to leave kingdom may exceed threshold, not arising the need for ruler to spend gold. Default = 1.0.")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyBribe)]
    public float WithholdToleranceLimitForBribes { get; set; } = 1f;
    [SettingPropertyInteger("{=2JzGpjwk}Gold cost multiplier", 100, 5000, Order = 4, RequireRestart = false, HintText = "{=6TbpFzJk}Multiplier for calculated gold cost to withhold the clan. Suggested value would be somewhere between 100 and 1000. Default = 1000.")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyBribe)]
    public int WithholdGoldMultiplier { get; set; } = 1000;

    //General settings
    [SettingPropertyBool("{=Fok4pGDs}Vassal minor factions follow general rules", Order = 0, RequireRestart = true, HintText = "{=1J0XkdxB}Specify if vassal minor factions should use general logic when considering leaving their kingdoms. If disabled, minor factions will use mercenary logic even being vassals. Enabling is suggested, consider this a bug fix.")]
    [SettingPropertyGroup(HeadingGeneral, GroupOrder = 99)]
    public bool FixMinorFactionVassals { get; set; } = false;
    [SettingPropertyBool("{=UgeDMBZE}Advanced hero tooltips", Order = 1, RequireRestart = true, HintText = "{=13W8p9hd}Enable adding additional info to hero tooltips in game Encyclopedia. That adds info about relations, loyalty etc - depending on enabled systems.")]
    [SettingPropertyGroup(HeadingGeneral, GroupOrder = 99)]
    public bool UseAdvancedHeroTooltips { get; set; } = false;

    //Debugging and loging
    [SettingPropertyDropdown("{=yul4vp54}Applies to", Order = 0, RequireRestart = false, HintText = "{=z3oSKZFE}Specify if you interested in debugging all kingdoms, or just the player's one. Default is [Player's kingdom].")]
    [SettingPropertyGroup(HeadingDebug, GroupOrder = 100)]
    public DefaultDropdown<string> EnsuredLoyaltyDebugScope { get; set; } = new DefaultDropdown<string>(new string[]
    {
      DropdownValueAll,
      DropdownValuePlayers,
      DropdownValueRuled
    }, 1);

    [SettingPropertyBool("{=xwz5YwZ8}Debug messages", Order = 1, RequireRestart = true, HintText = "{=uPYkZfKs}Enables general debug messages. These are informative and reasonably lore-friendly, but spammy. Default is false.")]
    [SettingPropertyGroup(HeadingDebug, GroupOrder = 100)]
    public bool EnableGeneralDebugging { get; set; } = false;

    [SettingPropertyBool("{=acsZWxWF}Technical debug messages", Order = 2, RequireRestart = true, HintText = "{=eBVb3S6S}Enables technical debug messages. These are not localized, poorly readable and extremely spammy. Default is false.")]
    [SettingPropertyGroup(HeadingDebug, GroupOrder = 100)]
    public bool EnableTechnicalDebugging { get; set; } = false;

    //Presets
    public override IDictionary<string, Func<BaseSettings>> GetAvailablePresets()
    {
      var basePresets = base.GetAvailablePresets(); // include the 'Default' preset that MCM provides
      basePresets.Add("Suggested", () => new Settings()
      {
        UseEnsuredLoyalty = true,
        UseRelationForEnsuredLoyalty = true,
        UseContextForEnsuredLoyalty = true,
        UseHonorForEnsuredLoyalty = true,
        UseWithholdPrice = true,
        UseWithholdBribing = true,
        FixMinorFactionVassals = true,
        UseAdvancedHeroTooltips = true
      });
      basePresets.Add("Suggested with logging", () => new Settings()
      {
        UseEnsuredLoyalty = true,
        UseRelationForEnsuredLoyalty = true,
        UseContextForEnsuredLoyalty = true,
        UseHonorForEnsuredLoyalty = true,
        UseWithholdPrice = true,
        UseWithholdBribing = true,
        FixMinorFactionVassals = true,
        UseAdvancedHeroTooltips = true,
        EnableGeneralDebugging = true
      });
      basePresets.Add("Technical", () => new Settings()
      {
        UseEnsuredLoyalty = true,
        UseRelationForEnsuredLoyalty = true,
        UseContextForEnsuredLoyalty = true,
        UseHonorForEnsuredLoyalty = true,
        UseWithholdPrice = true,
        UseWithholdBribing = true,
        FixMinorFactionVassals = true,
        UseAdvancedHeroTooltips = true,
        EnableGeneralDebugging = true,
        EnableTechnicalDebugging = true,
        EnsuredLoyaltyDebugScope  = new DefaultDropdown<string>(new string[]
          {
            DropdownValueAll,
            DropdownValuePlayers,
            DropdownValueRuled
          }, 0)
      });
      return basePresets;
    }
  }
}
