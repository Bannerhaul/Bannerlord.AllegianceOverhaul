using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Dropdown;
using MCM.Abstractions.Settings.Base.Global;

using System;

using TaleWorlds.Localization;

namespace AllegianceOverhaul
{
    public partial class Settings : AttributeGlobalSettings<Settings>
    {
        public override string Id => "AllegianceOverhaul_v3";
        public override string DisplayName => $"{new TextObject("{=rQWGD5PzD}Allegiance Overhaul")} {typeof(Settings).Assembly.GetName().Version.ToString(3)}";
        public override string FolderName => "Allegiance Overhaul";
        public override string FormatType => "json2";

        //Headings
        private const string HeadingGeneral = "{=TfOZojdUe}General settings";
        //private const string HeadingGeneralSub = "/{=TfOZojdUe}General settings";
        private const string HeadingDebug = "{=EYZduHauC}Debug settings";
        //private const string HeadingDebugSub = "/{=EYZduHauC}Debug settings";
        private const string HeadingTesting = "{=xveQEwa3t}Testing settings";

        private const string HeadingHarmonyCheckup = HeadingDebug + "/{=7miQNTkyK}Harmony checkup on initialize";

        private const string HeadingDestabilizeJoining = HeadingTesting + "/{=2QJJZSvAj}Destabilize join kingdom evaluation";
        private const string HeadingDestabilizeLeaving = HeadingTesting + "/{=UGM3kBQaI}Destabilize leave kingdom evaluation";

        //Reused settings, hints and values
        internal const string DropdownValueAllFactions = "{=F1recvlEK}All kingdoms";
        internal const string DropdownValuePlayers = "{=d7Lm9By8Y}Player's kingdom";
        internal const string DropdownValueRuledBy = "{=rE0tAgMYX}Kingdom ruled by player";

        //General settings
        [SettingPropertyInteger("{=tRpMRCEhN}Influence to denars ratio", 100, 2000, Order = 0, RequireRestart = false, HintText = "{=NKOFiv68M}The amount of denars that is interconvertible to one influence point. Native is 500. Default = 1000.")]
        [SettingPropertyGroup(HeadingGeneral, GroupOrder = 99)]
        public int InfluenceToDenars { get; set; } = 1000;

        [SettingPropertyBool("{=0F8Q0KWEY}Vassal minor factions follow general rules", Order = 1, RequireRestart = true, HintText = "{=JL391Jf8S}Specify if vassal minor factions should use general logic when considering leaving their kingdoms. If disabled, minor factions will use mercenary logic even being vassals. Enabling is suggested, consider this a bug fix.")]
        [SettingPropertyGroup(HeadingGeneral, GroupOrder = 99)]
        public bool FixMinorFactionVassals { get; set; } = false;

        [SettingPropertyBool("{=KJJRsb9mD}Advanced hero tooltips", Order = 2, RequireRestart = true, HintText = "{=myaps1StY}Enable adding additional info to hero tooltips in game Encyclopedia. That adds info about relations, loyalty etc - depending on enabled systems.")]
        [SettingPropertyGroup(HeadingGeneral, GroupOrder = 99)]
        public bool UseAdvancedHeroTooltips { get; set; } = false;

        //Debugging and loging
        [SettingPropertyDropdown("{=mNeDsYqbr}Applies to", Order = 0, RequireRestart = false, HintText = "{=eJ4E8HeZd}Specify if you interested in debugging all kingdoms, or just the player's one. Default is [Player's kingdom].")]
        [SettingPropertyGroup(HeadingDebug, GroupOrder = 100)]
        public DropdownDefault<string> DebugFactionScope { get; set; } = new DropdownDefault<string>(new string[]
        {
      DropdownValueAllFactions,
      DropdownValuePlayers,
      DropdownValueRuledBy
        }, 1);

        [SettingPropertyDropdown("{=hDblsNGNu}Systems of interest", Order = 1, RequireRestart = false, HintText = "{=vHBc3PR0d}Specify if you interested in debugging all of the mod functionality, or just some particular systems. Default is [All systems].")]
        [SettingPropertyGroup(HeadingDebug, GroupOrder = 100)]
        public DropdownDefault<DropdownObject<AOSystems>> DebugSystemScope { get; set; } = new DropdownDefault<DropdownObject<AOSystems>>(DropdownObject<AOSystems>.SetDropdownListFromEnum(), 0);

        [SettingPropertyBool("{=IcSqRbFXO}Debug messages", Order = 2, RequireRestart = true, HintText = "{=FBXsJfTus}Enables general debug messages. These are informative and reasonably lore-friendly, but spammy. Default is false.")]
        [SettingPropertyGroup(HeadingDebug, GroupOrder = 100)]
        public bool EnableGeneralDebugging { get; set; } = false;

        [SettingPropertyBool("{=3OLm7uYVa}Technical debug messages", Order = 2, RequireRestart = true, HintText = "{=wFmHefFVf}Enables technical debug messages. These are not localized, poorly readable and extremely spammy. Default is false.")]
        [SettingPropertyGroup(HeadingDebug, GroupOrder = 100)]
        public bool EnableTechnicalDebugging { get; set; } = false;

        [SettingPropertyBool("{=7miQNTkyK}Harmony checkup on initialize", RequireRestart = true, IsToggle = true, HintText = "{=FHAoyeQ1l}Specify if there should be a checkup for possible conflicts with other mods, that are using Harmony patches on same methods as Allegiance Overhaul.")]
        [SettingPropertyGroup(HeadingHarmonyCheckup, GroupOrder = 0)]
        public bool EnableHarmonyCheckup { get; set; } = true;

        [SettingPropertyText("{=Am6g9MuBh}Ignore list", RequireRestart = true, HintText = "{=QjPIOBo4f}List of IDs of the mods that should be ignored when checking for possible conflicts. Those IDs should be separated by semicolon.")]
        [SettingPropertyGroup(HeadingHarmonyCheckup)]
        public string HarmonyCheckupIgnoreList { get; set; } = "";

        //Testing settings
        [SettingPropertyBool("{=xveQEwa3t}Testing settings", RequireRestart = true, IsToggle = true, HintText = "{=lfkh9U9h2}These settings are intended for mod testing purposes, do not use them in actual gameplay.")]
        [SettingPropertyGroup(HeadingTesting, GroupOrder = 101)]
        public bool UseTestingSettings { get; set; } = false;

        [SettingPropertyBool("{=mAHbl2rzh}Free decision overriding", Order = 0, RequireRestart = true, HintText = "{=CB1vpOzra}Override kingdom decisions for free. Cheat!")]
        [SettingPropertyGroup(HeadingTesting)]
        public bool FreeDecisionOverriding { get; set; } = false;

        [SettingPropertyBool("{=g3xbLRIwj}Always pick player kingdom", Order = 1, RequireRestart = false, HintText = "{=q9flKsRfV}If \"Determined kingdom pick logic\" option from \"Migration tweaks\" system is enabled, player kingdom will always be chosen as a kingdom to join or defect to. Enabling this option also forces any checks on scores to join to be skipped. Cheat!")]
        [SettingPropertyGroup(HeadingTesting)]
        public bool AlwaysPickPlayerKingdom { get; set; } = false;

        [SettingPropertyBool("{=2QJJZSvAj}Destabilize join kingdom evaluation", Order = 2, RequireRestart = true, IsToggle = true, HintText = "{=ylTfKfJtR}Destabilize the evaluation of the ScoreOfClanToJoinKingdom.")]
        [SettingPropertyGroup(HeadingDestabilizeJoining, GroupOrder = 0)]
        public bool DestabilizeJoinEvaluation { get; set; } = false;

        [SettingPropertyFloatingInteger("{=jT31LaKLD}Join kingdom score flat modifier", -10f, 10f, Order = 0, RequireRestart = false, HintText = "{=L2S3TSjyg}Negative score modifier makes harder for clans to defect, positive score modifier increases probability of defection. Measured in millions.  Default = 10.0.")]
        [SettingPropertyGroup(HeadingDestabilizeJoining)]
        public float JoinScoreFlatModifier { get; set; } = 10f;

        [SettingPropertyBool("{=UGM3kBQaI}Destabilize leave kingdom evaluation", Order = 3, RequireRestart = true, IsToggle = true, HintText = "{=lxP0giGYl}Destabilize the evaluation of the ScoreOfClanToLeaveKingdom.")]
        [SettingPropertyGroup(HeadingDestabilizeLeaving, GroupOrder = 1)]
        public bool DestabilizeLeaveEvaluation { get; set; } = false;

        [SettingPropertyFloatingInteger("{=xZVeNSyJ7}Leave kingdom score flat modifier", -10f, 10f, Order = 0, RequireRestart = false, HintText = "{=4k9YXqh3E}Negative score modifier makes harder for clans to leave, positive score modifier increases probability of clans leaving kingdoms. Measured in millions. Default = 10.0.")]
        [SettingPropertyGroup(HeadingDestabilizeLeaving)]
        public float LeaveScoreFlatModifier { get; set; } = 10f;
    }

    //Enums
    [Flags]
    public enum AOSystems : byte
    {
        [System.ComponentModel.Description("{=5F7vaCncU}None")]
        None = 0,
        [System.ComponentModel.Description("{=qvoCfbvzC}Ensured loyalty")]
        EnsuredLoyalty = 1,
        [System.ComponentModel.Description("{=cMaAPsPG7}Migration tweaks")]
        MigrationTweaks = 2,
        [System.ComponentModel.Description("{=36iqZfxor}Politics rebalance")]
        PoliticsRebalance = 4,
        /*
        [System.ComponentModel.Description("{=}Relation overhaul")]
        RelationOverhaul = 8,
        */
        //Groups
        [System.ComponentModel.Description("{=Tku0VzTa5}All systems")]
        All = EnsuredLoyalty | MigrationTweaks | PoliticsRebalance //| RelationOverhaul
    }
    public enum ODCostCalculationMethod : byte
    {
        [System.ComponentModel.Description("{=zWf4os3fg}Flat influence override")]
        FlatInfluenceOverride = 0,
        [System.ComponentModel.Description("{=3Z8OCjcgX}Override using 'Slight Favor' cost")]
        SlightlyFavor = 1,
        [System.ComponentModel.Description("{=S1FwBcuBB}Override using 'Strong Favor' cost")]
        StronglyFavor = 2,
        [System.ComponentModel.Description("{=vuZpjgfjv}Override using 'Full Push' cost")]
        FullyPush = 3
    }
}
