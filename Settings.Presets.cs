using System;
using System.Collections.Generic;
using MCM.Abstractions.Data;
using MCM.Abstractions.Settings.Base;
using MCM.Abstractions.Settings.Base.Global;

namespace AllegianceOverhaul
{
  public partial class Settings : AttributeGlobalSettings<Settings>
  {
    private const string PresetSuggested = "{=s1ojXK7t}Suggested";
    private const string PresetSLogging = "{=ViCdJulG}Suggested with logging";
    private const string PresetTechnical = "{=3WYNEaOI}Technical";

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
}
