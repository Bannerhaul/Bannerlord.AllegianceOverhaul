using AllegianceOverhaul.CampaignBehaviors.BehaviorManagers;
using AllegianceOverhaul.Extensions.Harmony;
using AllegianceOverhaul.Helpers;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Barterables;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.BarterBehaviors;
using TaleWorlds.Core;

namespace AllegianceOverhaul.Patches.Migration
{
  [HarmonyPatch(typeof(DiplomaticBartersBehavior), "DailyTickClan")]
  class DiplomaticBartersBehaviorDailyTickClanPatch
  {
    private delegate void ConsiderDefectionDelegate(DiplomaticBartersBehavior instance, Clan clan1, Kingdom kingdom);
    private delegate void ConsiderClanJoinDelegate(DiplomaticBartersBehavior instance, Clan clan, Kingdom kingdom);
    private delegate void ConsiderClanJoinAsMercenaryDelegate(DiplomaticBartersBehavior instance, Clan clan, Kingdom kingdom);

    private static readonly ConsiderDefectionDelegate? deConsiderDefection = AccessHelper.GetDelegate<ConsiderDefectionDelegate>(typeof(DiplomaticBartersBehavior), "ConsiderDefection");
    private static readonly ConsiderClanJoinDelegate? deConsiderClanJoin = AccessHelper.GetDelegate<ConsiderClanJoinDelegate>(typeof(DiplomaticBartersBehavior), "ConsiderClanJoin");
    private static readonly ConsiderClanJoinAsMercenaryDelegate? deConsiderClanJoinAsMercenary = AccessHelper.GetDelegate<ConsiderClanJoinAsMercenaryDelegate>(typeof(DiplomaticBartersBehavior), "ConsiderClanJoinAsMercenary");

    private static readonly MethodInfo miGetRandomFloat = AccessTools.DeclaredPropertyGetter(typeof(MBRandom), "RandomFloat");
    private static readonly MethodInfo miGetAllKingdoms = AccessTools.DeclaredPropertyGetter(typeof(Kingdom), "All");
    private static readonly MethodInfo miConsiderPeace = AccessTools.Method(typeof(DiplomaticBartersBehavior), "ConsiderPeace");
    private static readonly MethodInfo miConsiderDefection = AccessTools.Method(typeof(DiplomaticBartersBehavior), "ConsiderDefection");
    private static readonly MethodInfo miConsiderClanJoin = AccessTools.Method(typeof(DiplomaticBartersBehavior), "ConsiderClanJoin");
    private static readonly MethodInfo miGetDefectionDecision = AccessTools.Method(typeof(DiplomaticBartersBehaviorDailyTickClanPatch), "GetDefectionDecision");
    private static readonly MethodInfo miGetJoinDecision = AccessTools.Method(typeof(DiplomaticBartersBehaviorDailyTickClanPatch), "GetJoinDecision");

    public static void GetDefectionDecision(Clan clan, bool clanHasMapEvent, List<Clan> list, DiplomaticBartersBehavior instance)
    {
      try
      {
        if (clanHasMapEvent)
        {
          return;
        }
        Clan? clanToDefectTo;
        if (SettingsHelper.SubSystemEnabled(SubSystemType.UseDeterminedKingdomPick))
        {
          bool alwaysPickPlayerKingdom = SettingsHelper.SubSystemEnabled(SubSystemType.AlwaysPickPlayerKingdom) && Clan.PlayerClan.Kingdom is not null;
          clanToDefectTo = alwaysPickPlayerKingdom ? Clan.PlayerClan : GetTopValuedKingdom(clan)?.Clans.FirstOrDefault(clanSelector => !clanSelector.IsEliminated);
        }
        else
        {
          clanToDefectTo = list.GetRandomElement();
          int num = 0;
          while (NotApplicable(clan, clanToDefectTo))
          {
            clanToDefectTo = list.GetRandomElement();
            ++num;
            if (num >= 20)
            {
              break;
            }
          }
        }
        if (!NotApplicable(clan, clanToDefectTo))
        {
          deConsiderDefection!(instance, clan, (Kingdom)clanToDefectTo!.MapFaction);
        }
      }
      catch (Exception ex)
      {
        MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
        DebugHelper.HandleException(ex, methodInfo, "Harmony patch for DiplomaticBartersBehavior. DailyTickClan");
      }

      //local methods
      static bool NotApplicable(Clan clan, Clan? clanToDefectTo) =>
        clanToDefectTo is null
        || clan.Kingdom is null
        || clanToDefectTo.Kingdom == null
        || clan.Kingdom == clanToDefectTo.Kingdom
        || !clanToDefectTo.MapFaction.IsKingdomFaction
        || clanToDefectTo.IsEliminated
        || ((clanToDefectTo == Clan.PlayerClan || clanToDefectTo.MapFaction.Leader == Hero.MainHero) && (!SettingsHelper.SubSystemEnabled(SubSystemType.AllowJoinRequests) || AOCooldownManager.HasPlayerRequestCooldown()));
    }

    public static void GetJoinDecision(Clan clan, bool clanHasMapEvent, DiplomaticBartersBehavior instance)
    {
      try
      {
        if (clanHasMapEvent)
        {
          return;
        }

        Kingdom? kingdomToJoin;
        if (SettingsHelper.SubSystemEnabled(SubSystemType.UseDeterminedKingdomPick))
        {
          bool alwaysPickPlayerKingdom = SettingsHelper.SubSystemEnabled(SubSystemType.AlwaysPickPlayerKingdom) && Clan.PlayerClan.Kingdom is not null;
          kingdomToJoin = alwaysPickPlayerKingdom ? Clan.PlayerClan.Kingdom : GetTopValuedKingdom(clan);
        }
        else
        {
          kingdomToJoin = Kingdom.All[MBRandom.RandomInt(Kingdom.All.Count)];
          kingdomToJoin = CulturalRndRevise(clan, kingdomToJoin);
        }

        if (NotApplicable(clan, kingdomToJoin))
        {
          return;
        }
        if (clan.IsMinorFaction)
        {
          deConsiderClanJoinAsMercenary!(instance, clan, kingdomToJoin!);
        }
        else
        {
          deConsiderClanJoin!(instance, clan, kingdomToJoin!);
        }
      }
      catch (Exception ex)
      {
        MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
        DebugHelper.HandleException(ex, methodInfo, "Harmony patch for DiplomaticBartersBehavior. DailyTickClan");
      }

      //local methods
      static Kingdom CulturalRndRevise(Clan clan, Kingdom kingdom1)
      {
        int num1 = 0;
        foreach (Kingdom kingdom2 in Kingdom.All)
        {
          if (kingdom2.Culture == clan.Culture)
          {
            num1 += 10;
          }
          else
          {
            ++num1;
          }
        }
        int num2 = (int)(MBRandom.RandomFloat * num1);
        foreach (Kingdom kingdom2 in Kingdom.All)
        {
          if (kingdom2.Culture == clan.Culture)
          {
            num2 -= 10;
          }
          else
          {
            --num2;
          }

          if (num2 < 0)
          {
            kingdom1 = kingdom2;
            break;
          }
        }

        return kingdom1;
      }
      static bool CanJoinOutOfWars(Clan clan, Kingdom kingdom1)
      {
        bool canJoin = true;
        if (!clan.IsMinorFaction)
        {
          foreach (Kingdom kingdom2 in Kingdom.All)
          {
            if (kingdom2 != kingdom1 && clan.IsAtWarWith(kingdom2) && (!kingdom2.IsAtWarWith(kingdom1) && kingdom1.TotalStrength <= 10.0 * kingdom2.TotalStrength))
            {
              canJoin = false;
              break;
            }
          }
        }
        return canJoin;
      }
      static bool NotApplicable(Clan clan, Kingdom? kingdomToJion) =>
        kingdomToJion is null
        || kingdomToJion.IsEliminated
        || (clan.Kingdom != null && !clan.IsUnderMercenaryService)
        || clan.MapFaction == kingdomToJion
        || clan.MapFaction.IsAtWarWith(kingdomToJion)
        || (kingdomToJion.Leader == Hero.MainHero && (!SettingsHelper.SubSystemEnabled(clan.IsUnderMercenaryService ? SubSystemType.AllowHireRequests : SubSystemType.AllowJoinRequests) || AOCooldownManager.HasPlayerRequestCooldown()))
        || !CanJoinOutOfWars(clan, kingdomToJion);
    }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
      try
      {
        int defectConditionIndex = -1, defectStartIndex = -1, defectEndIndex = -1;
        int joinConditionIndex = -1, joinStartIndex = -1, joinEndIndex = -1;
        List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
        for (int i = 0; i < codes.Count; ++i)
        {
          if (FindDefectionCondition(defectConditionIndex, codes, i))
          {
            defectConditionIndex = i;
            continue;
          }
          if (FindDefectionStart(defectConditionIndex, defectStartIndex, codes, i))
          {
            defectStartIndex = i;
            continue;
          }
          if (defectStartIndex > 0 && defectEndIndex < 0 && codes[i].opcode == OpCodes.Ret && codes[i - 1].Calls(miConsiderDefection))
          {
            defectEndIndex = i;
            continue;
          }
          if (defectEndIndex > 0 && joinConditionIndex < 0 && codes[i].Calls(miGetRandomFloat) && codes[i + 1].opcode == OpCodes.Ldarg_1)
          {
            joinConditionIndex = i;
            continue;
          }
          if (joinConditionIndex > 0 && joinStartIndex < 0 && codes[i].Calls(miGetAllKingdoms) && codes[i + 1].Calls(miGetAllKingdoms))
          {
            joinStartIndex = i;
            continue;
          }
          if (joinStartIndex > 0 && joinEndIndex < 0 && codes[i].opcode == OpCodes.Ret && codes[i - 1].Calls(miConsiderClanJoin))
          {
            joinEndIndex = i;
            break;
          }
        }
        //Logging
        if (defectStartIndex < 0 || defectEndIndex < 0 || joinStartIndex < 0 || joinEndIndex < 0)
        {
          LogNoHooksIssue(defectConditionIndex, defectStartIndex, defectEndIndex, joinConditionIndex, joinStartIndex, joinEndIndex, codes);
        }
        //do it in reverse pattern to mantain indexes in tact
        ReplaceCodeInstructions(joinStartIndex, joinEndIndex, codes, miGetJoinDecision, "join");
        ReplaceCodeInstructions(defectStartIndex, defectEndIndex, codes, miGetDefectionDecision, "defection");
        return codes.AsEnumerable();
      }
      catch (Exception ex)
      {
        MethodInfo? methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
        DebugHelper.HandleException(ex, methodInfo, "Harmony transpiler for DiplomaticBartersBehavior. DailyTickClan");
        return instructions;
      }

      //local methods
      static bool FindDefectionCondition(int defectConditionIndex, List<CodeInstruction> codes, int i)
      {
        return defectConditionIndex < 0 && i > 2 && i < codes.Count - 2
               && codes[i].Calls(miGetRandomFloat)
               && codes[i - 1].opcode == OpCodes.Ret && codes[i - 2].Calls(miConsiderPeace)
               && codes[i + 1].opcode == OpCodes.Ldc_R4 && codes[i + 2].opcode == OpCodes.Bge_Un;
      }
      static bool FindDefectionStart(int defectConditionIndex, int defectStartIndex, List<CodeInstruction> codes, int i)
      {
        return defectConditionIndex > 0 && defectStartIndex < 0 && i < codes.Count - 2
               && codes[i].opcode == OpCodes.Ldloc_1
               && codes[i + 1].opcode == OpCodes.Call
               && codes[i + 1].operand.ToString().Contains("GetRandomElement")
               && codes[i + 2].opcode == OpCodes.Stloc_S;
      }
      static void ReplaceCodeInstructions(int startIndex, int endIndex, List<CodeInstruction> codes, MethodInfo miToCall, string decisionDesc)
      {
        if (startIndex > -1 && endIndex > -1)
        {
          codes.RemoveRange(startIndex, endIndex - startIndex);
          codes.InsertRange(startIndex, GetCodeInstructions(miToCall));
        }
        else
        {
          MessageHelper.ErrorMessage("Harmony transpiler for DiplomaticBartersBehavior. DailyTickClan could not find code hooks for " + decisionDesc + " decision!");
        }

        static CodeInstruction[] GetCodeInstructions(MethodInfo miToCall) =>
          miToCall == miGetJoinDecision
            ? new CodeInstruction[]
              {
                  new CodeInstruction(opcode: OpCodes.Ldarg_1),
                  new CodeInstruction(opcode: OpCodes.Ldloc_0),
                  new CodeInstruction(opcode: OpCodes.Ldarg_0),
                  new CodeInstruction(opcode: OpCodes.Call, operand: miToCall)
              }
            : new CodeInstruction[]
              {
                  new CodeInstruction(opcode: OpCodes.Ldarg_1),
                  new CodeInstruction(opcode: OpCodes.Ldloc_0),
                  new CodeInstruction(opcode: OpCodes.Ldloc_1),
                  new CodeInstruction(opcode: OpCodes.Ldarg_0),
                  new CodeInstruction(opcode: OpCodes.Call, operand: miToCall)
              };
      }

      static void LogNoHooksIssue(int defectConditionIndex, int defectStartIndex, int defectEndIndex, int joinConditionIndex, int joinStartIndex, int joinEndIndex, List<CodeInstruction> codes)
      {
        LoggingHelper.Log("Indexes:", "Transpiler for DailyTickClan");
        StringBuilder issueInfo = new StringBuilder("");
        issueInfo.Append($"\tdefectConditionIndex = {defectConditionIndex}.\n\tdefectStartIndex={defectStartIndex}.\n\tdefectEndIndex={defectEndIndex}.\n\tjoinConditionIndex={joinConditionIndex}.\n\tjoinStartIndex={joinStartIndex}.\n\tjoinEndIndex={joinEndIndex}.");
        issueInfo.Append($"\nMethodInfos:");
        issueInfo.Append($"\n\tmiGetRandomFloat={(miGetRandomFloat != null ? miGetRandomFloat.ToString() : "not found")}");
        issueInfo.Append($"\n\tmiGetAllKingdoms={(miGetAllKingdoms != null ? miGetAllKingdoms.ToString() : "not found")}");
        issueInfo.Append($"\n\tmiConsiderPeace={(miConsiderPeace != null ? miConsiderPeace.ToString() : "not found")}");
        issueInfo.Append($"\n\tmiConsiderDefection={(miConsiderDefection != null ? miConsiderDefection.ToString() : "not found")}");
        issueInfo.Append($"\n\tmiConsiderClanJoin={(miConsiderClanJoin != null ? miConsiderClanJoin.ToString() : "not found")}");
        issueInfo.Append($"\nIL:");
        for (int i = 0; i < codes.Count; ++i)
        {
          issueInfo.Append($"\n\t{i:D4}:\t{codes[i]}");
        }
        // get info about other transpilers on OriginalMethod        
        HarmonyLib.Patches patches;
        patches = Harmony.GetPatchInfo(MethodBase.GetCurrentMethod());
        if (patches != null)
        {
          issueInfo.Append($"\nOther transpilers:");
          foreach (Patch patch in patches.Transpilers)
          {
            issueInfo.Append(patch.GetDebugString());
          }
        }
        LoggingHelper.Log(issueInfo.ToString());
      }
    }

    private static Kingdom? GetTopValuedKingdom(Clan clan)
    {
      Kingdom? topValuedKingdom = null;
      bool predicate(Kingdom k) =>
        !k.IsEliminated && k != clan.Kingdom
        && (SettingsHelper.SubSystemEnabled(clan.IsUnderMercenaryService ? SubSystemType.AllowHireRequests
                                                                         : SubSystemType.AllowJoinRequests) || k.Leader != Hero.MainHero);

      Dictionary<Kingdom, float> kingdoms = Kingdom.All.Where(predicate)
                                                       .ToDictionary(keySelector: kingdom => kingdom, elementSelector: kingdom => GetJoinValue(clan, kingdom));

      topValuedKingdom = kingdoms.OrderByDescending(kvp => kvp.Value).First().Key;
      return topValuedKingdom;

      static float GetJoinValue(Clan clan, Kingdom kingdom) =>
        clan.IsUnderMercenaryService ? new MercenaryJoinKingdomBarterable(clan.Leader, null, kingdom).GetValueForFaction(clan)
                                     : new JoinKingdomAsClanBarterable(clan.Leader, kingdom).GetValueForFaction(clan);
    }

    public static bool Prepare()
    {
      return SettingsHelper.SubSystemEnabled(SubSystemType.MigrationTweaks);
    }
  }
}
