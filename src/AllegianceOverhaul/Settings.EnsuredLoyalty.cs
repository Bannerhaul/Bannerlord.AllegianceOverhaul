using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Dropdown;
using MCM.Abstractions.Settings.Base.Global;

namespace AllegianceOverhaul
{
    public partial class Settings : AttributeGlobalSettings<Settings>
    {
        private const string HeadingEnsuredLoyalty = "{=qvoCfbvzC}Ensured loyalty";
        private const string HeadingEnsuredLoyaltyByRelation = HeadingEnsuredLoyalty + "/{=NHSdwRezs}Achieve via relation";
        private const string HeadingEnsuredLoyaltyByContext = HeadingEnsuredLoyaltyByRelation + "/{=CyQfExNwt}Modify by situational context";
        private const string HeadingEnsuredLoyaltyByHonor = HeadingEnsuredLoyaltyByRelation + "/{=djnL4Zdsh}Modify by honor level";
        private const string HeadingEnsuredLoyaltyPrice = HeadingEnsuredLoyaltyByRelation + "/{=SqzwL7txS}Withhold price";
        private const string HeadingEnsuredLoyaltyBribe = HeadingEnsuredLoyaltyPrice + "/{=yLmNJ0Rne}Bribing";

        //Ensured loyalty
        [SettingPropertyBool("{=qvoCfbvzC}Ensured loyalty", RequireRestart = true, IsToggle = true, HintText = "{=7kFDB4UUU}Enables specifying conditions for clans to become unreservedly loyal to kingdoms.")]
        [SettingPropertyGroup(HeadingEnsuredLoyalty, GroupOrder = 0)]
        public bool UseEnsuredLoyalty { get; set; } = false;

        [SettingPropertyDropdown("{=mNeDsYqbr}Applies to", RequireRestart = false, HintText = "{=EgueYDtnH}Specify if below rules should affect all kingdoms, or just the player's one. Default is [All kingdoms].")]
        [SettingPropertyGroup(HeadingEnsuredLoyalty)]
        public DropdownDefault<string> EnsuredLoyaltyScope { get; set; } = new DropdownDefault<string>(new string[]
        {
      DropdownValueAllFactions,
      DropdownValuePlayers,
      DropdownValueRuledBy
        }, 0);

        [SettingPropertyInteger("{=7R4UBeQQ0}Faction oath of fealty limitation period", 0, 420, Order = 0, RequireRestart = false, HintText = "{=LHA5p7XIj}Period in days after joining a kingdom, during which clan would not even consider leaving that kingdom. Default = 84 (a game year).")]
        [SettingPropertyGroup(HeadingEnsuredLoyalty)]
        public int FactionOathPeriod { get; set; } = 84;
        [SettingPropertyInteger("{=pTWZuG5gT}Minor faction oath of fealty limitation period", 0, 420, Order = 1, RequireRestart = false, HintText = "{=XAmM2cPa4}Period in days after joining a kingdom, during which minor faction would not even consider leaving that kingdom. Default = 63 (three quarters of a game year).")]
        [SettingPropertyGroup(HeadingEnsuredLoyalty)]
        public int MinorFactionOathPeriod { get; set; } = 63;
        [SettingPropertyInteger("{=DBIbrK44a}Minimum mercenary service period", 0, 420, Order = 2, RequireRestart = false, HintText = "{=tWVhVRqab}Period in days after initiating mercenary service of kingdom, during which minor faction would not even consider leaving that kingdom. Default = 42 (half a game year).")]
        [SettingPropertyGroup(HeadingEnsuredLoyalty)]
        public int MinorFactionServicePeriod { get; set; } = 42;
        [SettingPropertyBool("{=xnelIRbsx}Affect player conversations", Order = 3, RequireRestart = true, HintText = "{=62XGqQ3T1}Specify if ensured loyalty system should affect player conversations, making it impossible to reqruit lords that are loyal to ther lieges.")]
        [SettingPropertyGroup(HeadingEnsuredLoyalty)]
        public bool UseLoyaltyInConversations { get; set; } = true;

        //Ensured loyalty via relation
        [SettingPropertyBool("{=NHSdwRezs}Achieve via relation", RequireRestart = false, IsToggle = true, HintText = "{=sDS9CVleW}Specify if reaching certain relation level with kingdom leader should make clan unreservedly loyal to that kingdom.")]
        [SettingPropertyGroup(HeadingEnsuredLoyaltyByRelation, GroupOrder = 0)]
        public bool UseRelationForEnsuredLoyalty { get; set; } = false;

        [SettingPropertyInteger("{=FWaSJv1ew}Ensured loyalty baseline", -100, 100, RequireRestart = false, HintText = "{=01oFFSZay}The minimum required relationship a clan leader must have with a kingdom leader in order for clan to never leave that kingdom. Being below that threshold does NOT mean clan will aitomatically leave. Serves as a baseline for other togglable modifiers. Default = 50.")]
        [SettingPropertyGroup(HeadingEnsuredLoyaltyByRelation)]
        public int EnsuredLoyaltyBaseline { get; set; } = 50;

        //Ensured loyalty via relation - situational context
        [SettingPropertyBool("{=CyQfExNwt}Modify by situational context", RequireRestart = false, IsToggle = true, HintText = "{=kyRgBrRB5}Specify if situational context should affect minimum required relationship, at which loyalty is ensured.")]
        [SettingPropertyGroup(HeadingEnsuredLoyaltyByContext, GroupOrder = 0)]
        public bool UseContextForEnsuredLoyalty { get; set; } = false;

        [SettingPropertyInteger("{=71i0lndOs}Blood relation modifier", 0, 50, Order = 0, RequireRestart = false, HintText = "{=W72OrEIX0}Flat value that will be deducted from baseline if there is a kinship with the governing clan. The same value will be added to the baseline if clan considering defecting to a kingdom, ruled by kinsman. Default = 30 (blood relatives are loyal to and tend to join their kinsfolk).")]
        [SettingPropertyGroup(HeadingEnsuredLoyaltyByContext)]
        public int BloodRelativesEnsuredLoyaltyModifier { get; set; } = 30;
        [SettingPropertyInteger("{=JtjeW4Rer}Minor faction modifier", 0, 50, Order = 1, RequireRestart = false, HintText = "{=DObmEU4ws}Flat value that will be added to baseline if minor faction considering leaving its kingdom. Default = 10 (minor factions tend to be less loyal).")]
        [SettingPropertyGroup(HeadingEnsuredLoyaltyByContext)]
        public int MinorFactionEnsuredLoyaltyModifier { get; set; } = 10;
        [SettingPropertyInteger("{=iExnuT7fJ}Defection modifier", -50, 50, Order = 2, RequireRestart = false, HintText = "{=TjH9KMmq1}Flat value that will be added to baseline if clan considering not just leaving, but defecting to another kingdom. Default = 0.")]
        [SettingPropertyGroup(HeadingEnsuredLoyaltyByContext)]
        public int DefectionEnsuredLoyaltyModifier { get; set; } = 0;
        [SettingPropertyInteger("{=5QH7Ho5lw}Landless clan modifier", 0, 100, Order = 3, RequireRestart = false, HintText = "{=qDISZ8y7X}Flat value that will be added to baseline if clan owns no land. Default = 20 (landless clans tend to be less loyal).")]
        [SettingPropertyGroup(HeadingEnsuredLoyaltyByContext)]
        public int LandlessClanEnsuredLoyaltyModifier { get; set; } = 20;
        [SettingPropertyInteger("{=Y1qKYOEYS}Landless kingdom modifier", 0, 100, Order = 4, RequireRestart = false, HintText = "{=4Zoc3nZxs}Flat value that will be added to baseline of all clans of the kingdom, except the ruling one, if kingdom owns no land. Default = 50 (none but most honorable vassals would tolerate this).")]
        [SettingPropertyGroup(HeadingEnsuredLoyaltyByContext)]
        public int LandlessKingdomEnsuredLoyaltyModifier { get; set; } = 50;

        //Ensured loyalty via relation - honor
        [SettingPropertyBool("{=djnL4Zdsh}Modify by honor level", RequireRestart = false, IsToggle = true, HintText = "{=ac9wgFwlB}Specify if clan leader's honor should affect minimum required relationship, at which loyalty is ensured.")]
        [SettingPropertyGroup(HeadingEnsuredLoyaltyByHonor, GroupOrder = 1)]
        public bool UseHonorForEnsuredLoyalty { get; set; } = false;

        [SettingPropertyInteger("{=Ni7IrF2Mr}High honor step when leaving", 0, 30, Order = 0, RequireRestart = false, HintText = "{=YdpGFRihV}Flat value that will be deducted from baseline for each positive honor level of a clan leader when considering leaving kingdom (positive honor makes leaders loyal at lower relation). Default = 5.")]
        [SettingPropertyGroup(HeadingEnsuredLoyaltyByHonor)]
        public int PositiveHonorEnsuredLoyaltyModifier_Leaving { get; set; } = 5;
        [SettingPropertyInteger("{=AO6j3y5Sc}Low honor step when leaving", 0, 30, Order = 1, RequireRestart = false, HintText = "{=VRKD1Js1C}Flat value that will be added to baseline for each negative honor level of a clan leader when considering leaving kingdom (negative honor requires higher relation for leaders to become loyal). Default = 5.")]
        [SettingPropertyGroup(HeadingEnsuredLoyaltyByHonor)]
        public int NegativeHonorEnsuredLoyaltyModifier_Leaving { get; set; } = 5;
        [SettingPropertyInteger("{=3eL8FuPGx}High honor step when defecting", 0, 30, Order = 2, RequireRestart = false, HintText = "{=Q2u15dsoI}Flat value that will be deducted from baseline for each positive honor level of a clan leader when considering defection from kingdom (positive honor makes leaders loyal at lower relation). Default = 15.")]
        [SettingPropertyGroup(HeadingEnsuredLoyaltyByHonor)]
        public int PositiveHonorEnsuredLoyaltyModifier_Defecting { get; set; } = 15;
        [SettingPropertyInteger("{=m02D3oQHX}Low honor step when defecting", 0, 30, Order = 3, RequireRestart = false, HintText = "{=E7opeS9Z1}Flat value that will be added to baseline for each negative honor level of a clan leader when considering defection from kingdom (negative honor requires higher relation for leaders to become loyal). Default = 20.")]
        [SettingPropertyGroup(HeadingEnsuredLoyaltyByHonor)]
        public int NegativeHonorEnsuredLoyaltyModifier_Defecting { get; set; } = 20;

        //Ensured loyalty via relation - withhold price
        [SettingPropertyBool("{=SqzwL7txS}Withhold price", RequireRestart = false, IsToggle = true, HintText = "{=uWflyATMk}Specify if reaching certain relation level between clan leader and kingdom leader does not guarantee loyalty per se, but instead gives kingdom leader an option to withhold the clan that wishes to leave, using influence and money.")]
        [SettingPropertyGroup(HeadingEnsuredLoyaltyPrice, GroupOrder = 2)]
        public bool UseWithholdPrice { get; set; } = false;

        [SettingPropertyFloatingInteger("{=L92TepzxY}Tolerance limit (millions)", 0.1f, 5f, Order = 0, RequireRestart = false, HintText = "{=Nz207kdl4}Maximum amount, measured in millions, by which score of clan to leave kingdom may safely exceed threshold, not arising the need for ruler to intervene. Default = 0.5.")]
        [SettingPropertyGroup(HeadingEnsuredLoyaltyPrice)]
        public float WithholdToleranceLimit { get; set; } = 0.5f;
        [SettingPropertyFloatingInteger("{=3348wg7Ke}Influence cost multiplier", 0.1f, 5f, Order = 1, RequireRestart = false, HintText = "{=rjy6sldJM}Multiplier for calculated influence cost to withhold the clan. Default = 1.5.")]
        [SettingPropertyGroup(HeadingEnsuredLoyaltyPrice)]
        public float WithholdInfluenceMultiplier { get; set; } = 1.5f;

        [SettingPropertyBool("{=yLmNJ0Rne}Bribing", RequireRestart = false, IsToggle = true, HintText = "{=gqQJol1Jv}Specify if withholding the clan should cost money in addition to influence.")]
        [SettingPropertyGroup(HeadingEnsuredLoyaltyBribe, GroupOrder = 3)]
        public bool UseWithholdBribing { get; set; } = false;

        [SettingPropertyFloatingInteger("{=99gdptWF8}Tolerance limit for bribing (millions)", 0.1f, 10f, Order = 3, RequireRestart = false, HintText = "{=auUSqmZYu}Maximum amount, measured in millions, by which score of clan to leave kingdom may exceed threshold, not arising the need for ruler to spend gold. Default = 1.0.")]
        [SettingPropertyGroup(HeadingEnsuredLoyaltyBribe)]
        public float WithholdToleranceLimitForBribes { get; set; } = 1f;
        [SettingPropertyInteger("{=axp519JSq}Gold cost multiplier", 100, 5000, Order = 4, RequireRestart = false, HintText = "{=d1fl6A6Ov}Multiplier for calculated gold cost to withhold the clan. Suggested value would be somewhere between 100 and 1000. Default = 1000.")]
        [SettingPropertyGroup(HeadingEnsuredLoyaltyBribe)]
        public int WithholdGoldMultiplier { get; set; } = 1000;
    }
}