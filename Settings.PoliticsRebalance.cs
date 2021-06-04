using MCM.Abstractions.Settings.Base.Global;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Dropdown;

namespace AllegianceOverhaul
{
  public partial class Settings : AttributeGlobalSettings<Settings>
  {
    private const string HeadingPoliticsRebalance = "{=1bsT0jB0}Politics rebalance";
    private const string HeadingElectionRebalance = HeadingPoliticsRebalance + "/{=If18n1li}Election rebalance";
    private const string HeadingDecisionSupportRebalance = HeadingElectionRebalance + "/{=}Decision support rebalance";
    private const string HeadingFactorsFineTuning = HeadingDecisionSupportRebalance + "/{=}Factors fine-tuning";
    private const string HeadingFactorsStrength = HeadingDecisionSupportRebalance + "/{=}Factors strength";
    private const string HeadingElectionCooldowns = HeadingElectionRebalance + "/{=}Election cooldowns";

    //Politics rebalance
    [SettingPropertyBool("{=1bsT0jB0}Politics rebalance", RequireRestart = true, IsToggle = true, HintText = "{=u9YiryQR}Enables specifying various adjustments regarding kingdom political life and relations between lords.")]
    [SettingPropertyGroup(HeadingPoliticsRebalance, GroupOrder = 1)]
    public bool UsePoliticsRebalance { get; set; } = false;

    [SettingPropertyDropdown("{=yul4vp54}Applies to", RequireRestart = false, HintText = "{=y3ClkEcy}Specify if below rules should affect all kingdoms, or just the player's one. Default is [All kingdoms].")]
    [SettingPropertyGroup(HeadingPoliticsRebalance)]
    public DropdownDefault<string> PoliticsRebalanceScope { get; set; } = new DropdownDefault<string>(new string[]
    {
      DropdownValueAllFactions,
      DropdownValuePlayers,
      DropdownValueRuledBy
    }, 0);

    //Election rebalance
    [SettingPropertyBool("{=If18n1li}Election rebalance", RequireRestart = true, IsToggle = true, HintText = "{=GmyQQ5Cm}Enables specifying various adjustments regarding kingdom elections.")]
    [SettingPropertyGroup(HeadingElectionRebalance, GroupOrder = 0)]
    public bool UseElectionRebalance { get; set; } = false;

    [SettingPropertyDropdown("{=eoQCaS0r}Influence required to override decision", Order = 0, RequireRestart = false, HintText = "{=Gp9KcTuz}Specify desired way of calculating the influence, required to override popular decision with unpopular one. Ruler can pay 'Slight Favor', 'Strong Favor' or 'Full Push' decision costs for each lacking support point of unpopular decision, or just pay the exact amount of influence, that supporters of the popular decision spent in total. Native is [Override using 'Full Push' cost]. Suggested is [Flat influence override].")]
    [SettingPropertyGroup(HeadingElectionRebalance)]
    public DropdownDefault<DropdownObject<ODCostCalculationMethod>> OverrideDecisionCostCalculationMethod { get; set; } = new DropdownDefault<DropdownObject<ODCostCalculationMethod>>(DropdownObject<ODCostCalculationMethod>.SetDropdownListFromEnum(), (int)ODCostCalculationMethod.FlatInfluenceOverride);

    [SettingPropertyFloatingInteger("{=}Score threshold for AI to override decision", 0f, 200f, Order = 1, RequireRestart = true, HintText = "{=}Minimum difference between AI ruler desired decision score and popular decision score for the ruler clan to consider overriding popular decision. Native is 10. Default = 50.0.")]
    [SettingPropertyGroup(HeadingElectionRebalance)]
    public float OverrideDecisionScoreThreshold { get; set; } = 50f;

    //Decision support calculation
    [SettingPropertyBool("{=}Decision support rebalance", RequireRestart = true, IsToggle = true, HintText = "{=}Enables adjustments to decision support calculation logics, used by AI clans.")]
    [SettingPropertyGroup(HeadingDecisionSupportRebalance, GroupOrder = 0)]
    public bool UseDecisionSupportRebalance { get; set; } = false;

    [SettingPropertyDropdown("{=}Make peace", Order = 0, RequireRestart = false, HintText = "{=}Specify if and how clan support calculation logics for the make peace decision should be enhanced. Situational factor takes into account wars currently being fought and success rate against the faction to make peace with. Relationship factor is based on the relation with lords of the faction to make peace with. Tribute factor accounts for the gains or losses from the tributes to be imposed, including reputational ones.")]
    [SettingPropertyGroup(HeadingDecisionSupportRebalance)]
    public DropdownDefault<DropdownObject<PeaceAndWarConsideration>> PeaceSupportCalculationMethod { get; set; } = new DropdownDefault<DropdownObject<PeaceAndWarConsideration>>(DropdownObject<PeaceAndWarConsideration>.SetDropdownListFromEnum(), DropdownObject<PeaceAndWarConsideration>.GetEnumIndex(PeaceAndWarConsideration.All));

    [SettingPropertyDropdown("{=}Declare war", Order = 1, RequireRestart = false, HintText = "{=}Specify if and how clan support calculation logics for the declare war decision should be enhanced. Situational factor takes into account the power ratings of the currently warring kingdoms and of the kingdom to declare war on. Relationship factor is based on the relation with lords of the kingdom to declare war on. Tribute factor accounts for the amount of dinars the kingdom is currently receiving or being forced to pay.")]
    [SettingPropertyGroup(HeadingDecisionSupportRebalance)]
    public DropdownDefault<DropdownObject<PeaceAndWarConsideration>> WarSupportCalculationMethod { get; set; } = new DropdownDefault<DropdownObject<PeaceAndWarConsideration>>(DropdownObject<PeaceAndWarConsideration>.SetDropdownListFromEnum(), DropdownObject<PeaceAndWarConsideration>.GetEnumIndex(PeaceAndWarConsideration.All));

    [SettingPropertyDropdown("{=}Fief ownership", Order = 2, RequireRestart = false, HintText = "{=}Specify if and how clan support calculation logics should be enhanced when deciding who will own a fief.")]
    [SettingPropertyGroup(HeadingDecisionSupportRebalance)]
    public DropdownDefault<DropdownObject<FiefOwnershipConsideration>> FiefOwnershipSupportCalculationMethod { get; set; } = new DropdownDefault<DropdownObject<FiefOwnershipConsideration>>(DropdownObject<FiefOwnershipConsideration>.SetDropdownListFromEnum(), DropdownObject<FiefOwnershipConsideration>.GetEnumIndex(FiefOwnershipConsideration.All));

    [SettingPropertyDropdown("{=}Fief annexation", Order = 3, RequireRestart = false, HintText = "{=}Specify if and how clan support calculation logics for the fief annexation decision should be enhanced.")]
    [SettingPropertyGroup(HeadingDecisionSupportRebalance)]
    public DropdownDefault<DropdownObject<FiefOwnershipConsideration>> AnnexSupportCalculationMethod { get; set; } = new DropdownDefault<DropdownObject<FiefOwnershipConsideration>>(DropdownObject<FiefOwnershipConsideration>.SetDropdownListFromEnum(), DropdownObject<FiefOwnershipConsideration>.GetEnumIndex(FiefOwnershipConsideration.All));

    //Factors fine-tuning
    [SettingPropertyDropdown("{=}Fiefs restriction baseline", Order = 0, RequireRestart = false, HintText = "{=}Specify a method for determining the maximum number of fiefs that could belong to a clan, without imposing penalties.")]
    [SettingPropertyGroup(HeadingFactorsFineTuning, GroupOrder = 0)]
    public DropdownDefault<DropdownObject<NumberOfFiefsCalculationMethod>> FiefsDeemedFairBaseline { get; set; } = new DropdownDefault<DropdownObject<NumberOfFiefsCalculationMethod>>(DropdownObject<NumberOfFiefsCalculationMethod>.SetDropdownListFromEnum(), DropdownObject<NumberOfFiefsCalculationMethod>.GetEnumIndex(NumberOfFiefsCalculationMethod.ByClanTier));

    [SettingPropertyInteger("{=}Fiefs restriction modifier", -5, 5, Order = 1, RequireRestart = false, HintText = "{=}Flat value that will be added to the allowed number of fiefs, still not imposing any penalties. Default = 1.")]
    [SettingPropertyGroup(HeadingFactorsFineTuning, GroupOrder = 0)]
    public int FiefsDeemedFairModifier { get; set; } = 1;

    [SettingPropertyDropdown("{=}Desired fiefs baseline", Order = 10, RequireRestart = false, HintText = "{=}Specify a method for determining the desired number of fiefs for the clan.")]
    [SettingPropertyGroup(HeadingFactorsFineTuning, GroupOrder = 0)]
    public DropdownDefault<DropdownObject<NumberOfFiefsCalculationMethod>> DesiredFiefsBaseline { get; set; } = new DropdownDefault<DropdownObject<NumberOfFiefsCalculationMethod>>(DropdownObject<NumberOfFiefsCalculationMethod>.SetDropdownListFromEnum(), DropdownObject<NumberOfFiefsCalculationMethod>.GetEnumIndex(NumberOfFiefsCalculationMethod.ByClanMembers));

    [SettingPropertyInteger("{=}Desired fiefs modifier", -5, 5, Order = 11, RequireRestart = false, HintText = "{=}Flat value that will be added to the desired number of fiefs. Default = 1.")]
    [SettingPropertyGroup(HeadingFactorsFineTuning, GroupOrder = 0)]
    public int DesiredFiefsModifier { get; set; } = 1;

    [SettingPropertyBool("{=}Account for personal traits", Order = 12, RequireRestart = false, HintText = "{=}Specify if personal traits of the clan leader should affect the desired number of fiefs.")]
    [SettingPropertyGroup(HeadingFactorsFineTuning, GroupOrder = 0)]
    public bool DesiredFiefPersonalTraitsModifier { get; set; } = true;

    //Factors strength
    [SettingPropertyFloatingInteger("{=}Make peace situational factor strength", 0f, 5f, Order = 0, RequireRestart = false, HintText = "{=}A multiplier for the baseline result of calculating the situational factor for the 'make peace' decision. Situational factor takes into account the total number of wars currently being fought and the power ratings of each participating kingdom, as well as the success rate in the war against the faction to make peace with. Default = 1.")]
    [SettingPropertyGroup(HeadingFactorsStrength, GroupOrder = 1)]
    public float MakePeaceSituationalFactorStrength { get; set; } = 1f;

    [SettingPropertyFloatingInteger("{=}Make peace relationship factor strength", 0f, 5f, Order = 1, RequireRestart = false, HintText = "{=}A multiplier for the baseline result of calculating the relationship factor for the 'make peace' decision. Relationship factor is based on the mean relation with lords of the faction to make peace with. Default = 1.")]
    [SettingPropertyGroup(HeadingFactorsStrength, GroupOrder = 1)]
    public float MakePeaceRelationshipFactorStrength { get; set; } = 1f;

    [SettingPropertyFloatingInteger("{=}Make peace tribute factor strength", 0f, 5f, Order = 2, RequireRestart = false, HintText = "{=}A multiplier for the baseline result of calculating the tribute factor for the 'make peace' decision. Tribute factor takes into account the amount of denars that the kingdom will receive or will be forced to pay if peace is concluded, as well as the associated reputational gains or losses. Default = 1.")]
    [SettingPropertyGroup(HeadingFactorsStrength, GroupOrder = 1)]
    public float MakePeaceTributeFactorStrength { get; set; } = 1f;

    [SettingPropertyFloatingInteger("{=}Declare war situational factor strength", 0f, 5f, Order = 10, RequireRestart = false, HintText = "{=}A multiplier for the baseline result of calculating the situational factor for the 'declare war' decision. Situational factor takes into account the total number of wars currently being fought and the power ratings of each participating kingdom as well as of the kingdom to declare war on. Default = 1.")]
    [SettingPropertyGroup(HeadingFactorsStrength, GroupOrder = 1)]
    public float DeclareWarSituationalFactorStrength { get; set; } = 1f;

    [SettingPropertyFloatingInteger("{=}Declare war relationship factor strength", 0f, 5f, Order = 11, RequireRestart = false, HintText = "{=}A multiplier for the baseline result of calculating the relationship factor for the 'declare war' decision.. Relationship factor takes into account the median and extremes of relation with the clans of the kingdom to declare war on. Default = 1.")]
    [SettingPropertyGroup(HeadingFactorsStrength, GroupOrder = 1)]
    public float DeclareWarRelationshipFactorStrength { get; set; } = 1f;

    [SettingPropertyFloatingInteger("{=}Declare war tribute factor strength", 0f, 5f, Order = 12, RequireRestart = false, HintText = "{=}A multiplier for the baseline result of calculating the tribute factor for the 'declare war' decision. Tribute factor takes into account the amount of dinars the kingdom is currently receiving or being forced to pay. Default = 1.")]
    [SettingPropertyGroup(HeadingFactorsStrength, GroupOrder = 1)]
    public float DeclareWarTributeFactorStrength { get; set; } = 1f;

    [SettingPropertyFloatingInteger("{=}Fief ownership possessions factor strength", 0f, 5f, Order = 20, RequireRestart = false, HintText = "{=}A multiplier for the baseline result of calculating the possessions factor for the 'fief ownership' decision. Possessions factor takes into account total number of fiefs already belonging to the pretending clan. Default = 1.")]
    [SettingPropertyGroup(HeadingFactorsStrength, GroupOrder = 1)]
    public float FiefOwnershipPossessionsFactorStrength { get; set; } = 1f;

    //Election cooldowns
    [SettingPropertyBool("{=}Election cooldowns", RequireRestart = true, IsToggle = true, HintText = "{=}Enables cooldowns for kingdom elections on the identical topics.")]
    [SettingPropertyGroup(HeadingElectionCooldowns, GroupOrder = 1)]
    public bool UseElectionCooldowns { get; set; } = false;

    [SettingPropertyInteger("{=}Make peace election cooldown", 0, 168, Order = 0, RequireRestart = false, HintText = "{=}Minimum period in days following concluding of kingdom election on making peace with a faction, after which new election for making peace with that faction could be started. Default = 10.")]
    [SettingPropertyGroup(HeadingElectionCooldowns)]
    public int MakePeaceDecisionCooldown { get; set; } = 10;

    [SettingPropertyInteger("{=}Declare war election cooldown", 0, 168, Order = 1, RequireRestart = false, HintText = "{=}Minimum period in days following concluding of kingdom election on declaring war on a faction, after which new election for declaring war on that faction could be started. Default = 10.")]
    [SettingPropertyGroup(HeadingElectionCooldowns)]
    public int DeclareWarDecisionCooldown { get; set; } = 10;

    [SettingPropertyInteger("{=}Expel clan election cooldown", 0, 168, Order = 2, RequireRestart = false, HintText = "{=}Minimum period in days following concluding of kingdom election on expelling a clan from the kingdom, after which new election for expelling that clan could be started. Default = 84 (a game year).")]
    [SettingPropertyGroup(HeadingElectionCooldowns)]
    public int ExpelClanDecisionCooldown { get; set; } = 84;

    [SettingPropertyInteger("{=}Kingdom policy election cooldown", 0, 168, Order = 3, RequireRestart = false, HintText = "{=}Minimum period in days following concluding of kingdom election regarding certain kingdom policy, after which new election regarding same kingdom policy could be started. Default = 21 (one quarter of a game year).")]
    [SettingPropertyGroup(HeadingElectionCooldowns)]
    public int KingdomPolicyDecisionCooldown { get; set; } = 21;

    [SettingPropertyInteger("{=}Fief annexation election cooldown", 0, 168, Order = 4, RequireRestart = false, HintText = "{=}Minimum period in days following concluding of kingdom election on annexation of certain fief, after which new election regarding annexation of the same fief could be started. Half of that period applies to every other settlement of the original owner clan. Default = 42 (half a game year).")]
    [SettingPropertyGroup(HeadingElectionCooldowns)]
    public int AnnexationDecisionCooldown { get; set; } = 42;

    [SettingPropertyBool("{=}Affect player kingdom management", Order = 5, RequireRestart = true, HintText = "{=}Specify if election cooldowns should affect player's kingdom management, making it impossible for player to propose decisions that were recently under consideration by the kingdom's royal court.")]
    [SettingPropertyGroup(HeadingElectionCooldowns)]
    public bool UseElectionCooldownsForPlayer { get; set; } = true;
  }
}
