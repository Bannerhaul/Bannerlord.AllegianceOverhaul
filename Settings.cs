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
    public override string DisplayName => $"{new TextObject("{=qfpqfAdz}Allegiance Overhaul")} {typeof(GlobalSettings).Assembly.GetName().Version.ToString(3)}";
    public override string FolderName => "Allegiance Overhaul";
    public override string Format => "json";

    /*
    [SettingPropertyInteger("2 - Option 1", 0, 100, Order = 0, RequireRestart = false, HintText = "Option 1")]
    [SettingPropertyGroup("General settings group")]
    public int Test_Opion1 { get; set; } = 0;

    [SettingPropertyInteger("1 - Option 2", 0, 100, Order = 1, RequireRestart = false, HintText = "Option 2")]
    [SettingPropertyGroup("General settings group")]
    public int Test_Opion2 { get; set; } = 0;
    
    //[SettingPropertyInteger("Option 3", 0, 100, RequireRestart = false, HintText = "Option 3")]
    //[SettingPropertyGroup("Additional settings group", GroupOrder = 1)]
    //public int Test_Opion3 { get; set; } = 0;
    
    [SettingPropertyBool("4 - Togglable SubGroup1", RequireRestart = false, HintText = "Togglable SubGroup1")]
    [SettingPropertyGroup("Additional settings group/4 - Togglable SubGroup1", GroupOrder = 10, IsMainToggle = true)]
    public bool Test_TogglableSubGroup1 { get; set; } = false;

    [SettingPropertyInteger("Option layer 2", 0, 100, Order = 0, RequireRestart = false, HintText = "Option layer 2")]
    [SettingPropertyGroup("Additional settings group/4 - Togglable SubGroup1")]
    public int Test_Opion2_1 { get; set; } = 0;

    [SettingPropertyBool("Togglable SubGroup1_1", RequireRestart = false, HintText = "Togglable SubGroup1_1")]
    [SettingPropertyGroup("Additional settings group/4 - Togglable SubGroup1/Togglable SubGroup1_1", GroupOrder = 30, IsMainToggle = true)]
    public bool Test_TogglableSubGroup1_1 { get; set; } = false;

    [SettingPropertyInteger("Option layer 3", 0, 100, Order = 0, RequireRestart = false, HintText = "Option layer 3")]
    [SettingPropertyGroup("Additional settings group/4 - Togglable SubGroup1/Togglable SubGroup1_1")]
    public int Test_Opion3_1 { get; set; } = 0;

    [SettingPropertyBool("Togglable SubGroup1_1_1", RequireRestart = false, HintText = "Togglable SubGroup1_1_1")]
    [SettingPropertyGroup("Additional settings group/4 - Togglable SubGroup1/Togglable SubGroup1_1/Togglable SubGroup1_1_1", GroupOrder = 40, IsMainToggle = true)]
    public bool Test_TogglableSubGroup1_1_1 { get; set; } = false;

    [SettingPropertyInteger("Option layer 4", 0, 100, Order = 0, RequireRestart = false, HintText = "Option layer 4")]
    [SettingPropertyGroup("Additional settings group/4 - Togglable SubGroup1/Togglable SubGroup1_1/Togglable SubGroup1_1_1")]
    public int Test_Opion4_1 { get; set; } = 0;

    [SettingPropertyBool("3 - Togglable SubGroup2", RequireRestart = false, HintText = "Togglable SubGroup2")]
    [SettingPropertyGroup("Additional settings group/3 - Togglable SubGroup2", GroupOrder = 20, IsMainToggle = true)]
    public bool Test_TogglableSubGroup2 { get; set; } = false;
    */
    
    //Headings
    private const string HeadingGeneral = "{=yRuHl3O6}General settings";
    private const string HeadingGeneralSub = "/{=yRuHl3O6}General settings";
    private const string HeadingDebug = "{=fmUHcBME}Debug settings";
    private const string HeadingDebugSub = "/{=fmUHcBME}Debug settings";

    private const string HeadingEnsuredLoyalty = "{=Ps6RgRH1}Ensured loyalty";
    private const string HeadingEnsuredLoyaltyByRelation = HeadingEnsuredLoyalty + "/{=mmUgw8hL}Achieve via relation";
    private const string HeadingEnsuredLoyaltyByContext = HeadingEnsuredLoyaltyByRelation + "/{=m5JmQwFt}Modify by situational context";
    private const string HeadingEnsuredLoyaltyByHonor = HeadingEnsuredLoyaltyByRelation + "/{=eExUFCCQ}Modify by honor level";

    //Reused settings, hints and values
    private const string DropdownValueAll = "{=COahS2f6}All kingdoms";
    private const string DropdownValuePlayers = "{=JeehGy9z}Player's kingdom";
    private const string DropdownValueRuled = "{=v2TacaMr}Kingdom ruled by player";

    //Ensured loyalty
    [SettingPropertyBool("{=Ps6RgRH1}Ensured loyalty", RequireRestart = false, HintText = "{=xwz5YwZ8}Enables specifying conditions for clans to become unreservedly loyal to kingdoms.")]
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

    [SettingPropertyInteger("{=MIDoz9om}Faction oath of fealty limitation period", 0, 420, RequireRestart = false, HintText = "{=l5LWFlHv}Period in days after joining a kingdom, during which clan would not even consider leaving that kingdom. Default = 84 (a game year).")]
    [SettingPropertyGroup(HeadingEnsuredLoyalty)]
    public int FactionOathPeriod { get; set; } = 84;    
    [SettingPropertyInteger("{=Az4K6FiF}Minor faction oath of fealty limitation period", 0, 420, RequireRestart = false, HintText = "{=qRnwZFts}Period in days after joining a kingdom, during which minor faction would not even consider leaving that kingdom. Default = 63 (three quarters of a game year).")]
    [SettingPropertyGroup(HeadingEnsuredLoyalty)]
    public int MinorFactionOathPeriod { get; set; } = 42;
    [SettingPropertyInteger("{=eMMKVjRU}Minimum mercenary service period", 0, 420, RequireRestart = false, HintText = "{=00arXPdi}Period in days after initiating mercenary service of kingdom, during which minor faction would not even consider leaving that kingdom. Default = 42 (half a game year).")]
    [SettingPropertyGroup(HeadingEnsuredLoyalty)]
    public int MinorFactionServicePeriod { get; set; } = 42;

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

    [SettingPropertyInteger("{=Rbo96zKF}Minor faction modifier", 0, 50, Order = 0, RequireRestart = false, HintText = "{=BFnZrVGK}Flat value that will be added to baseline if minor faction considering leaving its kingdom. Default = 10 (minor factions tend to be less loyal).")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyByContext)]
    public int MinorFactionEnsuredLoyaltyModifier { get; set; } = 10;
    [SettingPropertyInteger("{=Nff6KlEn}Defection modifier", -50, 50, Order = 1, RequireRestart = false, HintText = "{=0B87ROVp}Flat value that will be added to baseline if clan considering not just leaving, but defection to another kingdom. Default = 0.")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyByContext)]
    public int DefectionEnsuredLoyaltyModifier { get; set; } = 0;
    [SettingPropertyInteger("{=5bJx7aO7}Landless clan modifier", 0, 100, Order = 2, RequireRestart = false, HintText = "{=QRmeDMie}Flat value that will be added to baseline if clan owns no land. Default = 20 (Landless clans tend to be less loyal).")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyByContext)]
    public int LandlessClanEnsuredLoyaltyModifier { get; set; } = 20;
    [SettingPropertyInteger("{=rHn5IY0P}Landless kingdom modifier", 0, 100, Order = 3, RequireRestart = false, HintText = "{=vSD05nAh}Flat value that will be added to baseline of all clans of the kingdom, except the ruling one, if kingdom owns no land. Default = 50 (none but most honorable vassals would tolerate this).")]
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

    [SettingPropertyBool("{=xwz5YwZ8}Debug messages", RequireRestart = false, HintText = "{=uPYkZfKs}Enables debug messages for ensured loyalty system. Lore-friendly, but spammy. Default is false.")]
    [SettingPropertyGroup(HeadingEnsuredLoyalty + HeadingDebugSub, GroupOrder = 1)]
    public bool DebugEnsuredLoyalty { get; set; } = false;

    [SettingPropertyDropdown("{=yul4vp54}Applies to", RequireRestart = false, HintText = "{=z3oSKZFE}Specify if you interested in debugging all kingdoms, or just the player's one. Default is [Player's kingdom].")]
    [SettingPropertyGroup(HeadingEnsuredLoyalty + HeadingDebugSub, GroupOrder = 1)]
    public DefaultDropdown<string> EnsuredLoyaltyDebugScope { get; set; } = new DefaultDropdown<string>(new string[]
    {
      DropdownValueAll,
      DropdownValuePlayers,
      DropdownValueRuled
    }, 1);

    //Presets
    public override IDictionary<string, Func<BaseSettings>> GetAvailablePresets()
    {
      var basePresets = base.GetAvailablePresets(); // include the 'Default' preset that MCM provides
      basePresets.Add("Suggested", () => new Settings()
      {
        UseEnsuredLoyalty = true,
        UseRelationForEnsuredLoyalty = true,
        UseContextForEnsuredLoyalty = true,
        UseHonorForEnsuredLoyalty = true
      });
      return basePresets;
    }
  }
}
