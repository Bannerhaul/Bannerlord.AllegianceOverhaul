using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Dropdown;
using MCM.Abstractions.Settings.Base.Global;

namespace AllegianceOverhaul
{
  public partial class Settings : AttributeGlobalSettings<Settings>
  {
    private const string HeadingPoliticsRebalance = "{=36iqZfxor}Politics rebalance";
    private const string HeadingElectionRebalance = HeadingPoliticsRebalance + "/{=KjkJ0swLq}Election rebalance";
    private const string HeadingElectionCooldowns = HeadingElectionRebalance + "/{=YyNEnyXao}Election cooldowns";

    //Politics rebalance
    [SettingPropertyBool("{=36iqZfxor}Politics rebalance", RequireRestart = true, IsToggle = true, HintText = "{=SSc734kI8}Enables specifying various adjustments regarding kingdom political life and relations between lords.")]
    [SettingPropertyGroup(HeadingPoliticsRebalance, GroupOrder = 2)]
    public bool UsePoliticsRebalance { get; set; } = false;

    [SettingPropertyDropdown("{=mNeDsYqbr}Applies to", RequireRestart = false, HintText = "{=EgueYDtnH}Specify if below rules should affect all kingdoms, or just the player's one. Default is [All kingdoms].")]
    [SettingPropertyGroup(HeadingPoliticsRebalance)]
    public DropdownDefault<string> PoliticsRebalanceScope { get; set; } = new DropdownDefault<string>(new string[]
    {
      DropdownValueAllFactions,
      DropdownValuePlayers,
      DropdownValueRuledBy
    }, 0);

    //Election rebalance
    [SettingPropertyBool("{=KjkJ0swLq}Election rebalance", RequireRestart = true, IsToggle = true, HintText = "{=sFWVFQSX7}Enables specifying various adjustments regarding kingdom elections.")]
    [SettingPropertyGroup(HeadingElectionRebalance, GroupOrder = 0)]
    public bool UseElectionRebalance { get; set; } = false;

    [SettingPropertyDropdown("{=BZnXaAO4m}Influence required to override decision", Order = 0, RequireRestart = false, HintText = "{=mhjZ7999E}Specify desired way of calculating the influence, required to override popular decision with unpopular one. Ruler can pay 'Slight Favor', 'Strong Favor' or 'Full Push' decision costs for each lacking support point of unpopular decision, or just pay the exact amount of influence, that supporters of the popular decision spent in total. Native is [Override using 'Full Push' cost]. Suggested is [Flat influence override].")]
    [SettingPropertyGroup(HeadingElectionRebalance)]
    public DropdownDefault<DropdownObject<ODCostCalculationMethod>> OverrideDecisionCostCalculationMethod { get; set; } = new DropdownDefault<DropdownObject<ODCostCalculationMethod>>(DropdownObject<ODCostCalculationMethod>.SetDropdownListFromEnum(), (int)ODCostCalculationMethod.FlatInfluenceOverride);

    [SettingPropertyFloatingInteger("{=PB4s4f10m}Score threshold for AI to override decision", 0f, 200f, Order = 1, RequireRestart = true, HintText = "{=GWUSWJ41x}Minimum difference between AI ruler desired decision score and popular decision score for the ruler clan to consider overriding popular decision. Native is 10. Default = 50.0.")]
    [SettingPropertyGroup(HeadingElectionRebalance)]
    public float OverrideDecisionScoreThreshold { get; set; } = 50f;

    //Election cooldowns
    [SettingPropertyBool("{=YyNEnyXao}Election cooldowns", RequireRestart = true, IsToggle = true, HintText = "{=cBT3GuZ7A}Enables cooldowns for kingdom elections on the identical topics.")]
    [SettingPropertyGroup(HeadingElectionCooldowns, GroupOrder = 1)]
    public bool UseElectionCooldowns { get; set; } = false;

    [SettingPropertyInteger("{=GhFRRa81m}Make peace election cooldown", 0, 168, Order = 0, RequireRestart = false, HintText = "{=gFtIHi76c}Minimum period in days following concluding of kingdom election on making peace with a faction, after which new election for making peace with that faction could be started. Default = 10.")]
    [SettingPropertyGroup(HeadingElectionCooldowns)]
    public int MakePeaceDecisionCooldown { get; set; } = 10;

    [SettingPropertyInteger("{=ChnOQKRaL}Declare war election cooldown", 0, 168, Order = 1, RequireRestart = false, HintText = "{=A6YgnDiES}Minimum period in days following concluding of kingdom election on declaring war on a faction, after which new election for declaring war on that faction could be started. Default = 10.")]
    [SettingPropertyGroup(HeadingElectionCooldowns)]
    public int DeclareWarDecisionCooldown { get; set; } = 10;

    [SettingPropertyInteger("{=A6I64VlbW}Expel clan election cooldown", 0, 168, Order = 2, RequireRestart = false, HintText = "{=ueSGbEov9}Minimum period in days following concluding of kingdom election on expelling a clan from the kingdom, after which new election for expelling that clan could be started. Default = 84 (a game year).")]
    [SettingPropertyGroup(HeadingElectionCooldowns)]
    public int ExpelClanDecisionCooldown { get; set; } = 84;

    [SettingPropertyInteger("{=XEHTEUkOg}Kingdom policy election cooldown", 0, 168, Order = 3, RequireRestart = false, HintText = "{=DnfTXY2fI}Minimum period in days following concluding of kingdom election regarding certain kingdom policy, after which new election regarding same kingdom policy could be started. Default = 21 (one quarter of a game year).")]
    [SettingPropertyGroup(HeadingElectionCooldowns)]
    public int KingdomPolicyDecisionCooldown { get; set; } = 21;

    [SettingPropertyInteger("{=GaFi8TUsT}Fief annexation election cooldown", 0, 168, Order = 4, RequireRestart = false, HintText = "{=heYA15q43}Minimum period in days following concluding of kingdom election on annexation of certain fief, after which new election regarding annexation of the same fief could be started. Half of that period applies to every other settlement of the original owner clan. Default = 42 (half a game year).")]
    [SettingPropertyGroup(HeadingElectionCooldowns)]
    public int AnnexationDecisionCooldown { get; set; } = 42;

    [SettingPropertyBool("{=Wx4GbepKH}Affect player kingdom management", Order = 5, RequireRestart = true, HintText = "{=3AgxiAy1v}Specify if election cooldowns should affect player's kingdom management, making it impossible for player to propose decisions that were recently under consideration by the kingdom's royal court.")]
    [SettingPropertyGroup(HeadingElectionCooldowns)]
    public bool UseElectionCooldownsForPlayer { get; set; } = true;
  }
}
