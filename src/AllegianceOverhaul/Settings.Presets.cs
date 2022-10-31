using AllegianceOverhaul.Extensions;

using MCM.Abstractions;
using MCM.Abstractions.Base;
using MCM.Abstractions.Base.Global;

using MCM.Common;

using System;
using System.Collections.Generic;

namespace AllegianceOverhaul
{
    public partial class Settings : AttributeGlobalSettings<Settings>
    {
        private const string PresetSuggested = "{=R1fvUUG3F}Suggested";
        private const string PresetSLogging = "{=TwYyQD6LF}Suggested with logging";
        private const string PresetTechnical = "{=gYrl8ux8o}Technical";

        //Presets
        public override IEnumerable<ISettingsPreset> GetBuiltInPresets()
        {
            // include all the presets that MCM provides
            foreach (var preset in base.GetBuiltInPresets())
            {
                yield return preset;
            }

            yield return new MemorySettingsPreset(Id, "suggested", PresetSuggested.ToLocalizedString(), () => new Settings()
            {
                //Loyalty
                UseEnsuredLoyalty = true,
                UseRelationForEnsuredLoyalty = true,
                UseContextForEnsuredLoyalty = true,
                UseHonorForEnsuredLoyalty = true,
                UseWithholdPrice = true,
                UseWithholdBribing = true,
                //Migration
                UseMigrationTweaks = true,
                //Politics
                UsePoliticsRebalance = true,
                UseElectionRebalance = true,
                UseElectionCooldowns = true,
                //General
                FixMinorFactionVassals = true,
                UseAdvancedHeroTooltips = true
            });

            yield return new MemorySettingsPreset(Id, "logging", PresetSLogging.ToLocalizedString(), () => new Settings()
            {
                UseEnsuredLoyalty = true,
                UseRelationForEnsuredLoyalty = true,
                UseContextForEnsuredLoyalty = true,
                UseHonorForEnsuredLoyalty = true,
                UseWithholdPrice = true,
                UseWithholdBribing = true,
                //Migration
                UseMigrationTweaks = true,
                //Politics
                UsePoliticsRebalance = true,
                UseElectionRebalance = true,
                UseElectionCooldowns = true,
                //General
                FixMinorFactionVassals = true,
                UseAdvancedHeroTooltips = true,
                EnableGeneralDebugging = true,
                DebugFactionScope = new Dropdown<string>(new string[]
                {
            DropdownValueAllFactions,
            DropdownValuePlayers,
            DropdownValueRuledBy
                }, 1),
                DebugSystemScope = new Dropdown<DropdownObject<AOSystems>>(DropdownObject<AOSystems>.SetDropdownListFromEnum(), 7)
            });

            yield return new MemorySettingsPreset(Id, "technical", PresetTechnical.ToLocalizedString(), () => new Settings()
            {
                UseEnsuredLoyalty = true,
                UseRelationForEnsuredLoyalty = true,
                UseContextForEnsuredLoyalty = true,
                UseHonorForEnsuredLoyalty = true,
                UseWithholdPrice = true,
                UseWithholdBribing = true,
                //Migration
                UseMigrationTweaks = true,
                //Politics
                UsePoliticsRebalance = true,
                UseElectionRebalance = true,
                UseElectionCooldowns = true,
                //General
                FixMinorFactionVassals = true,
                UseAdvancedHeroTooltips = true,
                EnableGeneralDebugging = true,
                EnableTechnicalDebugging = true,
                DebugFactionScope = new Dropdown<string>(new string[]
                {
            DropdownValueAllFactions,
            DropdownValuePlayers,
            DropdownValueRuledBy
                }, 0),
                DebugSystemScope = new Dropdown<DropdownObject<AOSystems>>(DropdownObject<AOSystems>.SetDropdownListFromEnum(), 7)
            });
        }
    }
}