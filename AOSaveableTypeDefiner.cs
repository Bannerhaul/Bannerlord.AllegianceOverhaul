using System.Collections.Generic;

using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.SaveSystem;

using AllegianceOverhaul.CampaignBehaviors.BehaviorManagers;
using AllegianceOverhaul.SavableClasses;

namespace AllegianceOverhaul
{
  internal class AOSaveableTypeDefiner : SaveableTypeDefiner
  {
    public AOSaveableTypeDefiner() : base(2001750000) { }
    protected override void DefineClassTypes()
    {
      //BasicSavableClasses (1 through 50)
      base.AddClassDefinition(typeof(KingdomDecisionConclusion), 1);
      //ComplexSavableClasses (51 through 99)
      //BehaviorManagers (100 through 150)
      base.AddClassDefinition(typeof(AOCooldownManager), 100);
      base.AddClassDefinition(typeof(AORelationManager), 101);
    }
    protected override void DefineStructTypes()
    {
      this.AddStructDefinition(typeof(SegmentalFractionalScore), 901);
    }
    protected override void DefineContainerDefinitions()
    {
      base.ConstructContainerDefinition(typeof(Dictionary<KingdomDecision, KingdomDecisionConclusion>));
      base.ConstructContainerDefinition(typeof(Dictionary<ulong, SegmentalFractionalScore>));
      base.ConstructContainerDefinition(typeof(Dictionary<ulong, int>));
    }
  }
}