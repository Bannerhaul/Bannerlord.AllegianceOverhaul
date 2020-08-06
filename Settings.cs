using System;
using System.Collections.Generic;
using MCM.Abstractions.Data;
using MCM.Abstractions.Settings.Base;
using MCM.Abstractions.Settings.Base.Global;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using TaleWorlds.Localization;
using AllegianceOverhaul.Extensions;

namespace AllegianceOverhaul
{
  public class Settings : AttributeGlobalSettings<Settings>
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

    private const string HeadingEnsuredLoyalty = "{=Ps6RgRH1}Ensured loyalty";
    private const string HeadingEnsuredLoyaltyByRelation = HeadingEnsuredLoyalty + "/{=mmUgw8hL}Achieve via relation";
    private const string HeadingEnsuredLoyaltyByContext = HeadingEnsuredLoyaltyByRelation + "/{=m5JmQwFt}Modify by situational context";
    private const string HeadingEnsuredLoyaltyByHonor = HeadingEnsuredLoyaltyByRelation + "/{=eExUFCCQ}Modify by honor level";
    private const string HeadingEnsuredLoyaltyPrice = HeadingEnsuredLoyaltyByRelation + "/{=uXwgU2WE}Withhold price";
    private const string HeadingEnsuredLoyaltyBribe = HeadingEnsuredLoyaltyPrice + "/{=rhvq8Yhb}Bribing";

    private const string HeadingPoliticsRebalance = "{=1bsT0jB0}Politics rebalance";
    private const string HeadingElectionRebalance = HeadingPoliticsRebalance + "/{=If18n1li}Election rebalance";
    private const string HeadingDecisionSupportRebalance = HeadingElectionRebalance + "/{=}Decision support rebalance";
    private const string HeadingFactorsFineTuning = HeadingDecisionSupportRebalance + "/{=}Factors fine-tuning";
    private const string HeadingFactorsStrength = HeadingDecisionSupportRebalance + "/{=}Factors strength";

    private const string HeadingElectionCooldowns = HeadingElectionRebalance + "/{=}Election cooldowns";

    private const string HeadingRelationOverhaul = "{=}Relation overhaul";

    private const string HeadingHarmonyCheckup = HeadingDebug + "/{=ZnT9o5HI}Harmony checkup on initialize";

    private const string HeadingDestabilizeJoining = HeadingTesting + "/{=}Destabilize join kingdom evaluation";
    private const string HeadingDestabilizeLeaving = HeadingTesting + "/{=}Destabilize leave kingdom evaluation";

    //Reused settings, hints and values
    private const string PresetSuggested = "{=s1ojXK7t}Suggested";
    private const string PresetSLogging = "{=ViCdJulG}Suggested with logging";
    private const string PresetTechnical = "{=3WYNEaOI}Technical";

    private const string DropdownValueAllFactions = "{=COahS2f6}All kingdoms";
    private const string DropdownValuePlayers = "{=JeehGy9z}Player's kingdom";
    private const string DropdownValueRuledBy = "{=v2TacaMr}Kingdom ruled by player";

    //Ensured loyalty
    [SettingPropertyBool("{=Ps6RgRH1}Ensured loyalty", RequireRestart = true, HintText = "{=QLii7mIG}Enables specifying conditions for clans to become unreservedly loyal to kingdoms.")]
    [SettingPropertyGroup(HeadingEnsuredLoyalty, GroupOrder = 0, IsMainToggle = true)]
    public bool UseEnsuredLoyalty { get; set; } = false;

    [SettingPropertyDropdown("{=yul4vp54}Applies to", RequireRestart = false, HintText = "{=y3ClkEcy}Specify if below rules should affect all kingdoms, or just the player's one. Default is [All kingdoms].")]
    [SettingPropertyGroup(HeadingEnsuredLoyalty)]
    public DefaultDropdown<string> EnsuredLoyaltyScope { get; set; } = new DefaultDropdown<string>(new string[]
    {
      DropdownValueAllFactions,
      DropdownValuePlayers,
      DropdownValueRuledBy
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
    [SettingPropertyBool("{=Fxyb9iPM}Affect player conversations", Order = 3, RequireRestart = true, HintText = "{=ZaPJhkuk}Specify if ensured loyalty system should affect player conversations, making it impossible to reqruit lords that are loyal to ther lieges.")]
    [SettingPropertyGroup(HeadingEnsuredLoyalty)]
    public bool UseLoyaltyInConversations { get; set; } = true;

    //Ensured loyalty via relation
    [SettingPropertyBool("{=mmUgw8hL}Achieve via relation", RequireRestart = false, HintText = "{=gj3cKWJo}Specify if reaching certain relation level with kingdom leader should make clan unreservedly loyal to that kingdom.")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyByRelation, GroupOrder = 0, IsMainToggle = true)]
    public bool UseRelationForEnsuredLoyalty { get; set; } = false;

    [SettingPropertyInteger("{=JvWw6eSp}Ensured loyalty baseline", -100, 100, RequireRestart = false, HintText = "{=XP7qtIkt}The minimum required relationship a clan leader must have with a kingdom leader in order for clan to never leave that kingdom. Being below that threshold does NOT mean clan will aitomatically leave. Serves as a baseline for other togglable modifiers. Default = 50.")]
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
    [SettingPropertyInteger("{=5bJx7aO7}Landless clan modifier", 0, 100, Order = 3, RequireRestart = false, HintText = "{=QRmeDMie}Flat value that will be added to baseline if clan owns no land. Default = 20 (landless clans tend to be less loyal).")]
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
    [SettingPropertyFloatingInteger("{=siPJey6u}Influence cost multiplier", 0.1f, 5f, Order = 1, RequireRestart = false, HintText = "{=Tei4s42W}Multiplier for calculated influence cost to withhold the clan. Default = 1.5.")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyPrice)]
    public float WithholdInfluenceMultiplier { get; set; } = 1.5f;

    [SettingPropertyBool("{=rhvq8Yhb}Bribing", RequireRestart = false, HintText = "{=rJw4D3kJ}Specify if withholding the clan should cost money in addition to influence.")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyBribe, GroupOrder = 3, IsMainToggle = true)]
    public bool UseWithholdBribing { get; set; } = false;

    [SettingPropertyFloatingInteger("{=cM9saCDB}Tolerance limit for bribing (millions)", 0.1f, 10f, Order = 3, RequireRestart = false, HintText = "{=tCqdcDuR}Maximum amount, measured in millions, by which score of clan to leave kingdom may exceed threshold, not arising the need for ruler to spend gold. Default = 1.0.")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyBribe)]
    public float WithholdToleranceLimitForBribes { get; set; } = 1f;
    [SettingPropertyInteger("{=2JzGpjwk}Gold cost multiplier", 100, 5000, Order = 4, RequireRestart = false, HintText = "{=6TbpFzJk}Multiplier for calculated gold cost to withhold the clan. Suggested value would be somewhere between 100 and 1000. Default = 1000.")]
    [SettingPropertyGroup(HeadingEnsuredLoyaltyBribe)]
    public int WithholdGoldMultiplier { get; set; } = 1000;

    //Politics rebalance
    [SettingPropertyBool("{=1bsT0jB0}Politics rebalance", RequireRestart = true, HintText = "{=u9YiryQR}Enables specifying various adjustments regarding kingdom political life and relations between lords.")]
    [SettingPropertyGroup(HeadingPoliticsRebalance, GroupOrder = 1, IsMainToggle = true)]
    public bool UsePoliticsRebalance { get; set; } = false;

    [SettingPropertyDropdown("{=yul4vp54}Applies to", RequireRestart = false, HintText = "{=y3ClkEcy}Specify if below rules should affect all kingdoms, or just the player's one. Default is [All kingdoms].")]
    [SettingPropertyGroup(HeadingPoliticsRebalance)]
    public DefaultDropdown<string> PoliticsRebalanceScope { get; set; } = new DefaultDropdown<string>(new string[]
    {
      DropdownValueAllFactions,
      DropdownValuePlayers,
      DropdownValueRuledBy
    }, 0);

    //Election rebalance
    [SettingPropertyBool("{=If18n1li}Election rebalance", RequireRestart = true, HintText = "{=GmyQQ5Cm}Enables specifying various adjustments regarding kingdom elections.")]
    [SettingPropertyGroup(HeadingElectionRebalance, GroupOrder = 0, IsMainToggle = true)]
    public bool UseElectionRebalance { get; set; } = false;

    [SettingPropertyDropdown("{=eoQCaS0r}Influence required to override decision", Order = 0, RequireRestart = false, HintText = "{=Gp9KcTuz}Specify desired way of calculating the influence, required to override popular decision with unpopular one. Ruler can pay 'Slight Favor', 'Strong Favor' or 'Full Push' decision costs for each lacking support point of unpopular decision, or just pay the exact amount of influence, that supporters of the popular decision spent in total. Native is [Override using 'Full Push' cost]. Suggested is [Flat influence override].")]
    [SettingPropertyGroup(HeadingElectionRebalance)]
    public DefaultDropdown<DropdownObject<ODCostCalculationMethod>> OverrideDecisionCostCalculationMethod { get; set; } = new DefaultDropdown<DropdownObject<ODCostCalculationMethod>>(DropdownObject<ODCostCalculationMethod>.SetDropdownListFromEnum(), (int)ODCostCalculationMethod.FlatInfluenceOverride);

    [SettingPropertyFloatingInteger("{=}Score threshold for AI to override decision", 0f, 200f, Order = 1, RequireRestart = true, HintText = "{=}Minimum difference between AI ruler desired decision score and popular decision score for the ruler clan to consider overriding popular decision. Native is 10. Default = 50.0.")]
    [SettingPropertyGroup(HeadingElectionRebalance)]
    public float OverrideDecisionScoreThreshold { get; set; } = 50f;

    //Decision support calculation
    [SettingPropertyBool("{=}Decision support rebalance", RequireRestart = true, HintText = "{=}Enables adjustments to decision support calculation logics, used by AI clans.")]
    [SettingPropertyGroup(HeadingDecisionSupportRebalance, GroupOrder = 0, IsMainToggle = true)]
    public bool UseDecisionSupportRebalance { get; set; } = false;

    [SettingPropertyDropdown("{=}Make peace", Order = 0, RequireRestart = false, HintText = "{=}Specify if and how clan support calculation logics for the make peace decision should be enhanced. Situational factor takes into account wars currently being fought and success rate against the faction to make peace with. Relationship factor is based on the relation with lords of the faction to make peace with. Tribute factor accounts for the gains or losses from the tributes to be imposed, including reputational ones.")]
    [SettingPropertyGroup(HeadingDecisionSupportRebalance)]
    public DefaultDropdown<DropdownObject<PeaceAndWarConsideration>> PeaceSupportCalculationMethod { get; set; } = new DefaultDropdown<DropdownObject<PeaceAndWarConsideration>>(DropdownObject<PeaceAndWarConsideration>.SetDropdownListFromEnum(), DropdownObject<PeaceAndWarConsideration>.GetEnumIndex(PeaceAndWarConsideration.All));

    [SettingPropertyDropdown("{=}Declare war", Order = 1, RequireRestart = false, HintText = "{=}Specify if and how clan support calculation logics for the declare war decision should be enhanced. Situational factor takes into account the power ratings of the currently warring kingdoms and of the kingdom to declare war on. Relationship factor is based on the relation with lords of the kingdom to declare war on. Tribute factor accounts for the amount of dinars the kingdom is currently receiving or being forced to pay.")]
    [SettingPropertyGroup(HeadingDecisionSupportRebalance)]
    public DefaultDropdown<DropdownObject<PeaceAndWarConsideration>> WarSupportCalculationMethod { get; set; } = new DefaultDropdown<DropdownObject<PeaceAndWarConsideration>>(DropdownObject<PeaceAndWarConsideration>.SetDropdownListFromEnum(), DropdownObject<PeaceAndWarConsideration>.GetEnumIndex(PeaceAndWarConsideration.All));

    [SettingPropertyDropdown("{=}Fief ownership", Order = 2, RequireRestart = false, HintText = "{=}Specify if and how clan support calculation logics should be enhanced when deciding who will own a fief.")]
    [SettingPropertyGroup(HeadingDecisionSupportRebalance)]
    public DefaultDropdown<DropdownObject<FiefOwnershipConsideration>> FiefOwnershipSupportCalculationMethod { get; set; } = new DefaultDropdown<DropdownObject<FiefOwnershipConsideration>>(DropdownObject<FiefOwnershipConsideration>.SetDropdownListFromEnum(), DropdownObject<FiefOwnershipConsideration>.GetEnumIndex(FiefOwnershipConsideration.All));

    [SettingPropertyDropdown("{=}Fief annexation", Order = 3, RequireRestart = false, HintText = "{=}Specify if and how clan support calculation logics for the fief annexation decision should be enhanced.")]
    [SettingPropertyGroup(HeadingDecisionSupportRebalance)]
    public DefaultDropdown<DropdownObject<FiefOwnershipConsideration>> AnnexSupportCalculationMethod { get; set; } = new DefaultDropdown<DropdownObject<FiefOwnershipConsideration>>(DropdownObject<FiefOwnershipConsideration>.SetDropdownListFromEnum(), DropdownObject<FiefOwnershipConsideration>.GetEnumIndex(FiefOwnershipConsideration.All));

    //Factors fine-tuning
    [SettingPropertyDropdown("{=}Fiefs restriction baseline", Order = 0, RequireRestart = false, HintText = "{=}Specify a method for determining the maximum number of fiefs that could belong to a clan, without imposing penalties.")]
    [SettingPropertyGroup(HeadingFactorsFineTuning, GroupOrder = 0)]
    public DefaultDropdown<DropdownObject<NumberOfFiefsCalculationMethod>> FiefsDeemedFairBaseline { get; set; } = new DefaultDropdown<DropdownObject<NumberOfFiefsCalculationMethod>>(DropdownObject<NumberOfFiefsCalculationMethod>.SetDropdownListFromEnum(), DropdownObject<NumberOfFiefsCalculationMethod>.GetEnumIndex(NumberOfFiefsCalculationMethod.ByClanTier));

    [SettingPropertyInteger("{=}Fiefs restriction modifier", -5, 5, Order = 1, RequireRestart = false, HintText = "{=}Flat value that will be added to the allowed number of fiefs, still not imposing any penalties. Default = 1.")]
    [SettingPropertyGroup(HeadingFactorsFineTuning, GroupOrder = 0)]
    public int FiefsDeemedFairModifier { get; set; } = 1;

    [SettingPropertyDropdown("{=}Desired fiefs baseline", Order = 10, RequireRestart = false, HintText = "{=}Specify a method for determining the desired number of fiefs for the clan.")]
    [SettingPropertyGroup(HeadingFactorsFineTuning, GroupOrder = 0)]
    public DefaultDropdown<DropdownObject<NumberOfFiefsCalculationMethod>> DesiredFiefsBaseline { get; set; } = new DefaultDropdown<DropdownObject<NumberOfFiefsCalculationMethod>>(DropdownObject<NumberOfFiefsCalculationMethod>.SetDropdownListFromEnum(), DropdownObject<NumberOfFiefsCalculationMethod>.GetEnumIndex(NumberOfFiefsCalculationMethod.ByClanMembers));

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

    [SettingPropertyFloatingInteger("{=}Fief ownership possessions factor strength", 0f, 5f, Order = 0, RequireRestart = false, HintText = "{=}A multiplier for the baseline result of calculating the possessions factor for the 'fief ownership' decision. Possessions factor takes into account total number of fiefs already belonging to the pretending clan. Default = 1.")]
    [SettingPropertyGroup(HeadingFactorsStrength, GroupOrder = 1)]
    public float FiefOwnershipPossessionsFactorStrength { get; set; } = 1f;

    //Election cooldowns
    [SettingPropertyBool("{=}Election cooldowns", RequireRestart = true, HintText = "{=}Enables cooldowns for kingdom elections on the identical topics.")]
    [SettingPropertyGroup(HeadingElectionCooldowns, GroupOrder = 1, IsMainToggle = true)]
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
    /*
    //Relation overhaul
    [SettingPropertyBool("{=}Relation overhaul", RequireRestart = true, HintText = "{=}Enables personalized asymmetric hero relation system and specifying various other adjustments regarding hero relations, including new sources of relation shifts.")]
    [SettingPropertyGroup(HeadingRelationOverhaul, GroupOrder = 2, IsMainToggle = true)]
    public bool UseRelationOverhaul { get; set; } = false;

    [SettingPropertyDropdown("{=yul4vp54}Applies to", RequireRestart = false, HintText = "{=y3ClkEcy}Specify if below rules should affect all kingdoms, or just the player's one. Default is [All kingdoms].")]
    [SettingPropertyGroup(HeadingRelationOverhaul)]
    public DefaultDropdown<string> RelationOverhaulScope { get; set; } = new DefaultDropdown<string>(new string[]
    {
      DropdownValueAllFactions.ToLocalizedString(),
      DropdownValuePlayers.ToLocalizedString(),
      DropdownValueRuledBy.ToLocalizedString()
    }, 0);
    */
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
      DropdownValueAllFactions.ToLocalizedString(),
      DropdownValuePlayers.ToLocalizedString(),
      DropdownValueRuledBy.ToLocalizedString()
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

    //Presets
    public override IDictionary<string, Func<BaseSettings>> GetAvailablePresets()
    {
      IDictionary<string, Func<BaseSettings>> basePresets = base.GetAvailablePresets(); // include the 'Default' preset that MCM provides
      basePresets.Add(PresetSuggested, () => new Settings()
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
      basePresets.Add(PresetSLogging, () => new Settings()
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
      basePresets.Add(PresetTechnical, () => new Settings()
      {
        UseEnsuredLoyalty = true,
        UseRelationForEnsuredLoyalty = true,
        UseContextForEnsuredLoyalty = true,
        UseHonorForEnsuredLoyalty = true,
        UseWithholdPrice = true,
        UseWithholdBribing = true,
        FixMinorFactionVassals = true,
        UseAdvancedHeroTooltips = true,
        UsePoliticsRebalance = true,
        UseElectionRebalance = true,
        EnableGeneralDebugging = true,
        EnableTechnicalDebugging = true,
        DebugFactionScope = new DefaultDropdown<string>(new string[]
          {
            DropdownValueAllFactions,
            DropdownValuePlayers,
            DropdownValueRuledBy
          }, 0)
      });
      return basePresets;
    }
  }

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
