using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace AllegianceOverhaul
{
  class SubModule : MBSubModuleBase
  {
    protected override void OnSubModuleLoad()
    {
      base.OnSubModuleLoad();
      new Harmony("Bannerlord.AllegianceOverhaul").PatchAll();
    }
    protected override void OnBeforeInitialModuleScreenSetAsRoot()
    {
      base.OnBeforeInitialModuleScreenSetAsRoot();
      InformationManager.DisplayMessage(new InformationMessage("Loaded Allegiance Overhaul!", Color.FromUint(4282569842U)));
    }
  }
}

/*
 * Идеи:
 * * Внятные сообщения, отключаемые через настройку 
 * * Кулдаун на смену факции
 * * Гарантированная верность с настройками (база, учет типа предательства, учет чести - отдельно для каждого типа предательства, ?учет generocity?)
 * * Настройка ценности факции (отношения, уровень чести, длительность отношений)
 * * Отношения на очнове довольства политиками
 * * Отношения при голосованиях
 * * Управление армиями королевства
*/
