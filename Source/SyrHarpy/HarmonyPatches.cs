using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace SyrHarpy
{
    // Token: 0x02000008 RID: 8
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        // Token: 0x0600002E RID: 46 RVA: 0x00002D70 File Offset: 0x00000F70
        static HarmonyPatches()
        {
            var harmony = new Harmony("Syrchalis.Rimworld.Harpy");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            if (!ThrumkinActive && !NagaActive)
            {
                harmony.Patch(AccessTools.Method(typeof(GrammarUtility), "RulesForPawn", new[]
                {
                    typeof(string),
                    typeof(Name),
                    typeof(string),
                    typeof(PawnKindDef),
                    typeof(Gender),
                    typeof(Faction),
                    typeof(int),
                    typeof(int),
                    typeof(string),
                    typeof(bool),
                    typeof(bool),
                    typeof(bool),
                    typeof(List<RoyalTitle>),
                    typeof(Dictionary<string, string>),
                    typeof(bool)
                }), null, new HarmonyMethod(AccessTools.Method(typeof(RulesForPawnPatch), "RulesForPawn_Postfix")));
            }

            if (IndividualityActive)
            {
                return;
            }

            harmony.Patch(AccessTools.Method(typeof(InteractionWorker_RomanceAttempt), "RandomSelectionWeight"),
                new HarmonyMethod(AccessTools.Method(typeof(RandomSelectionWeightPatch),
                    "RandomSelectionWeight_Prefix")));
            harmony.Patch(AccessTools.Method(typeof(Pawn_RelationsTracker), "SecondaryLovinChanceFactor"),
                new HarmonyMethod(AccessTools.Method(typeof(SecondaryLovinChanceFactorPatch),
                    "SecondaryLovinChanceFactor_Prefix")));
        }

        // Token: 0x1700000F RID: 15
        // (get) Token: 0x0600002F RID: 47 RVA: 0x00002F2E File Offset: 0x0000112E
        public static bool ThrumkinActive
        {
            get { return ModsConfig.ActiveModsInLoadOrder.Any(m => m.PackageId == "syrchalis.thrumkin"); }
        }

        // Token: 0x17000010 RID: 16
        // (get) Token: 0x06000030 RID: 48 RVA: 0x00002F59 File Offset: 0x00001159
        public static bool NagaActive
        {
            get { return ModsConfig.ActiveModsInLoadOrder.Any(m => m.PackageId == "syrchalis.naga"); }
        }

        // Token: 0x17000011 RID: 17
        // (get) Token: 0x06000031 RID: 49 RVA: 0x00002F84 File Offset: 0x00001184
        public static bool IndividualityActive
        {
            get { return ModsConfig.ActiveModsInLoadOrder.Any(m => m.PackageId == "syrchalis.individuality"); }
        }
    }
}