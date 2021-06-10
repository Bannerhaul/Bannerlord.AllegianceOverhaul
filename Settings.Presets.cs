using System;
using System.Collections.Generic;

using MCM.Abstractions.Settings.Base;
using MCM.Abstractions.Settings.Base.Global;
using MCM.Abstractions.Dropdown;

using AllegianceOverhaul.Extensions;

namespace AllegianceOverhaul
{
  public partial class Settings : AttributeGlobalSettings<Settings>
  {
    private const string PresetSuggested = "{=R1fvUUG3F}Suggested";
    private const string PresetSLogging = "{=TwYyQD6LF}Suggested with logging";
    private const string PresetTechnical = "{=gYrl8ux8o}Technical";

    //Presets
    public override IDictionary<string, Func<BaseSettings>> GetAvailablePresets()
    {
      IDictionary<string, Func<BaseSettings>> basePresets = base.GetAvailablePresets(); // include the 'Default' preset that MCM provides
      basePresets.Add(PresetSuggested.ToLocalizedString(), () => new Settings()
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
      basePresets.Add(PresetSLogging.ToLocalizedString(), () => new Settings()
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
      basePresets.Add(PresetTechnical.ToLocalizedString(), () => new Settings()
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
        DebugFactionScope = new DropdownDefault<string>(new string[]
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
