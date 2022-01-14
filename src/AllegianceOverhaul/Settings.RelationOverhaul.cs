using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Dropdown;
using MCM.Abstractions.Settings.Base.Global;

namespace AllegianceOverhaul
{
    /*
    public partial class Settings : AttributeGlobalSettings<Settings>
    {
      private const string HeadingRelationOverhaul = "{=}Relation overhaul";

      //Relation overhaul
      [SettingPropertyBool("{=}Relation overhaul", RequireRestart = true, IsToggle = true, HintText = "{=}Enables personalized asymmetric hero relation system and specifying various other adjustments regarding hero relations, including new sources of relation shifts.")]
      [SettingPropertyGroup(HeadingRelationOverhaul, GroupOrder = 3)]
      public bool UseRelationOverhaul { get; set; } = false;

      [SettingPropertyDropdown("{=mNeDsYqbr}Applies to", RequireRestart = false, HintText = "{=EgueYDtnH}Specify if below rules should affect all kingdoms, or just the player's one. Default is [All kingdoms].")]
      [SettingPropertyGroup(HeadingRelationOverhaul)]
      public DropdownDefault<string> RelationOverhaulScope { get; set; } = new DropdownDefault<string>(new string[]
      {
        DropdownValueAllFactions,
        DropdownValuePlayers,
        DropdownValueRuledBy
      }, 0);
    }
    */
}