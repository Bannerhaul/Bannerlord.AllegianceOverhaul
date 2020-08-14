using System;
using MCM.Abstractions.Data;
using MCM.Abstractions.Settings.Base.Global;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using TaleWorlds.Localization;
using AllegianceOverhaul.Extensions;

namespace AllegianceOverhaul
{
  public partial class Settings : AttributeGlobalSettings<Settings>
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
    private const string HeadingTesting = "{=}Testing settings";

    private const string HeadingHarmonyCheckup = HeadingDebug + "/{=ZnT9o5HI}Harmony checkup on initialize";

    private const string HeadingDestabilizeJoining = HeadingTesting + "/{=}Destabilize join kingdom evaluation";
    private const string HeadingDestabilizeLeaving = HeadingTesting + "/{=}Destabilize leave kingdom evaluation";

    //Reused settings, hints and values
    private const string DropdownValueAllFactions = "{=COahS2f6}All kingdoms";
    private const string DropdownValuePlayers = "{=JeehGy9z}Player's kingdom";
    private const string DropdownValueRuledBy = "{=v2TacaMr}Kingdom ruled by player";

    //General settings
    [SettingPropertyInteger("{=}Influence to denars ratio", 100, 2000, Order = 0, RequireRestart = false, HintText = "{=}The amount of denars that is interconvertible to one influence point. Native is 500. Default = 1000.")]
    [SettingPropertyGroup(HeadingGeneral, GroupOrder = 99)]
    public int InfluenceToDenars { get; set; } = 1000;

    [SettingPropertyBool("{=Fok4pGDs}Vassal minor factions follow general rules", Order = 1, RequireRestart = true, HintText = "{=1J0XkdxB}Specify if vassal minor factions should use general logic when considering leaving their kingdoms. If disabled, minor factions will use mercenary logic even being vassals. Enabling is suggested, consider this a bug fix.")]
    [SettingPropertyGroup(HeadingGeneral, GroupOrder = 99)]
    public bool FixMinorFactionVassals { get; set; } = false;

    [SettingPropertyBool("{=UgeDMBZE}Advanced hero tooltips", Order = 2, RequireRestart = true, HintText = "{=13W8p9hd}Enable adding additional info to hero tooltips in game Encyclopedia. That adds info about relations, loyalty etc - depending on enabled systems.")]
    [SettingPropertyGroup(HeadingGeneral, GroupOrder = 99)]
    public bool UseAdvancedHeroTooltips { get; set; } = false;

    //Debugging and loging
    [SettingPropertyDropdown("{=yul4vp54}Applies to", Order = 0, RequireRestart = false, HintText = "{=z3oSKZFE}Specify if you interested in debugging all kingdoms, or just the player's one. Default is [Player's kingdom].")]
    [SettingPropertyGroup(HeadingDebug, GroupOrder = 100)]
    public DefaultDropdown<string> DebugFactionScope { get; set; } = new DefaultDropdown<string>(new string[]
    {
      DropdownValueAllFactions,
      DropdownValuePlayers,
      DropdownValueRuledBy
    }, 1);

    [SettingPropertyDropdown("{=}Systems of interest", Order = 1, RequireRestart = false, HintText = "{=}Specify if you interested in debugging all of the mod functionality, or just some particular systems. Default is [All systems].")]
    [SettingPropertyGroup(HeadingDebug, GroupOrder = 100)]
    public DefaultDropdown<DropdownObject<AOSystems>> DebugSystemScope { get; set; } = new DefaultDropdown<DropdownObject<AOSystems>>(DropdownObject<AOSystems>.SetDropdownListFromEnum(), 0);

    [SettingPropertyBool("{=xwz5YwZ8}Debug messages", Order = 2, RequireRestart = true, HintText = "{=uPYkZfKs}Enables general debug messages. These are informative and reasonably lore-friendly, but spammy. Default is false.")]
    [SettingPropertyGroup(HeadingDebug, GroupOrder = 100)]
    public bool EnableGeneralDebugging { get; set; } = false;

    [SettingPropertyBool("{=acsZWxWF}Technical debug messages", Order = 2, RequireRestart = true, HintText = "{=eBVb3S6S}Enables technical debug messages. These are not localized, poorly readable and extremely spammy. Default is false.")]
    [SettingPropertyGroup(HeadingDebug, GroupOrder = 100)]
    public bool EnableTechnicalDebugging { get; set; } = false;

    [SettingPropertyBool("{=ZnT9o5HI}Harmony checkup on initialize", RequireRestart = true, HintText = "{=ELPI6N1Q}Specify if there should be a checkup for possible conflicts with other mods, that are using Harmony patches on same methods as Allegiance Overhaul.")]
    [SettingPropertyGroup(HeadingHarmonyCheckup, GroupOrder = 0, IsMainToggle = true)]
    public bool EnableHarmonyCheckup { get; set; } = true;

    [SettingPropertyText("{=531Vobla}Ignore list", RequireRestart = true, HintText = "{=59KvCjXV}List of IDs of the mods that should be ignored when checking for possible conflicts. Those IDs should be separated by semicolon.")]
    [SettingPropertyGroup(HeadingHarmonyCheckup)]
    public string HarmonyCheckupIgnoreList { get; set; } = "";

    //Testing settings
    [SettingPropertyBool("{=}Testing settings", RequireRestart = true, HintText = "{=}These settings are intended for mod testing purposes, do not use them in actual gameplay.")]
    [SettingPropertyGroup(HeadingTesting, GroupOrder = 101, IsMainToggle = true)]
    public bool UseTestingSettings { get; set; } = false;

    [SettingPropertyBool("{=}Free decision overriding", Order = 0, RequireRestart = true, HintText = "{=}Override kingdom decisions for free. Cheat!")]
    [SettingPropertyGroup(HeadingTesting)]
    public bool FreeDecisionOverriding { get; set; } = false;

    [SettingPropertyBool("{=}Destabilize join kingdom evaluation", Order = 0, RequireRestart = true, HintText = "{=}Destabilize the evaluation of the ScoreOfClanToJoinKingdom.")]
    [SettingPropertyGroup(HeadingDestabilizeJoining, GroupOrder = 0, IsMainToggle = true)]
    public bool DestabilizeJoinEvaluation { get; set; } = false;

    [SettingPropertyFloatingInteger("{=}Join kingdom score flat modifier", -10f, 10f, Order = 0, RequireRestart = false, HintText = "{=}Negative score modifier makes harder for clans to defect, positive score modifier increases probability of defection. Measured in millions.  Default = 10.0.")]
    [SettingPropertyGroup(HeadingDestabilizeJoining)]
    public float JoinScoreFlatModifier { get; set; } = 10f;

    [SettingPropertyBool("{=}Destabilize leave kingdom evaluation", Order = 1, RequireRestart = true, HintText = "{=}Destabilize the evaluation of the ScoreOfClanToLeaveKingdom.")]
    [SettingPropertyGroup(HeadingDestabilizeLeaving, GroupOrder = 1, IsMainToggle = true)]
    public bool DestabilizeLeaveEvaluation { get; set; } = false;

    [SettingPropertyFloatingInteger("{=}Leave kingdom score flat modifier", -10f, 10f, Order = 0, RequireRestart = false, HintText = "{=}Negative score modifier makes harder for clans to leave, positive score modifier increases probability of clans leaving kingdoms. Measured in millions. Default = 10.0.")]
    [SettingPropertyGroup(HeadingDestabilizeLeaving)]
    public float LeaveScoreFlatModifier { get; set; } = 10f;
  }

  //Enums
  [Flags]
  public enum AOSystems : byte
  {
    [System.ComponentModel.Description("{=}None")]
    None = 0,
    [System.ComponentModel.Description("{=Ps6RgRH1}Ensured loyalty")]
    EnsuredLoyalty = 1,
    [System.ComponentModel.Description("{=1bsT0jB0}Politics rebalance")]
    PoliticsRebalance = 2,
    [System.ComponentModel.Description("{=}Relation overhaul")]
    RelationOverhaul = 4,
    //Groups
    [System.ComponentModel.Description("{=}All systems")]
    All = EnsuredLoyalty | PoliticsRebalance | RelationOverhaul
  }
  public enum ODCostCalculationMethod : byte
  {
    [System.ComponentModel.Description("{=}Flat influence override")]
    FlatInfluenceOverride = 0,
    [System.ComponentModel.Description("{=}Override using 'Slight Favor' cost")]
    SlightlyFavor = 1,
    [System.ComponentModel.Description("{=}Override using 'Strong Favor' cost")]
    StronglyFavor = 2,
    [System.ComponentModel.Description("{=}Override using 'Full Push' cost")]
    FullyPush = 3
  }

  [Flags]
  public enum PeaceAndWarConsideration : byte
  {
    [System.ComponentModel.Description("{=}Use native logic")]
    Native = 0,
    [System.ComponentModel.Description("{=}Apply situational factor")]
    SituationalFactor = 1,
    [System.ComponentModel.Description("{=}Apply relationship factor")]
    RelationshipFactor = 2,
    [System.ComponentModel.Description("{=}Apply tribute factor")]
    TributeFactor = 4,
    //Groups
    [System.ComponentModel.Description("{=}Apply situational and relationship factors")]
    FirstPair = SituationalFactor | RelationshipFactor,
    [System.ComponentModel.Description("{=}Apply situational and tribute factors")]
    SecondPair = SituationalFactor | TributeFactor,
    [System.ComponentModel.Description("{=}Apply relationship and tribute factors")]
    ThirdPair = RelationshipFactor | TributeFactor,
    [System.ComponentModel.Description("{=}Apply all the factors")]
    All = SituationalFactor | RelationshipFactor | TributeFactor
  }

  [Flags]
  public enum FiefOwnershipConsideration : byte
  {
    [System.ComponentModel.Description("{=}Use native logic")]
    Native = 0,
    [System.ComponentModel.Description("{=}Apply possessions factor")]
    PossessionsFactor = 1,
    [System.ComponentModel.Description("{=}Apply personality factor")]
    PersonalityFactor = 2,
    [System.ComponentModel.Description("{=}Apply benefit factor")]
    BenefitFactor = 4,
    //Groups
    [System.ComponentModel.Description("{=}Apply possessions and personality factors")]
    FirstPair = PossessionsFactor | PersonalityFactor,
    [System.ComponentModel.Description("{=}Apply possessions and benefit factors")]
    SecondPair = PossessionsFactor | BenefitFactor,
    [System.ComponentModel.Description("{=}Apply personality and benefit factors")]
    ThirdPair = PersonalityFactor | BenefitFactor,
    [System.ComponentModel.Description("{=}Apply all the factors")]
    All = PossessionsFactor | PersonalityFactor | BenefitFactor
  }

  public enum NumberOfFiefsCalculationMethod : byte
  {
    [System.ComponentModel.Description("{=}Without restrictions")]
    WithoutRestrictions = 0,
    [System.ComponentModel.Description("{=}Based on the clan tier")]
    ByClanTier = 1,
    [System.ComponentModel.Description("{=}Based on the number of clan members")]
    ByClanMembers = 2
  }
}
