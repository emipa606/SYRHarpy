using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace SyrHarpy
{
    // Token: 0x0200001A RID: 26
    [HarmonyPatch(typeof(Alert_Boredom), "BoredPawns", MethodType.Getter)]
    public static class BoredPawnsPatch
    {
        // Token: 0x0600004D RID: 77 RVA: 0x00003D5C File Offset: 0x00001F5C
        [HarmonyPrefix]
        public static bool BoredPawns_Prefix(ref List<Pawn> __result,
            List<Pawn> ___boredPawnsResult)
        {
            ___boredPawnsResult.Clear();
            foreach (var pawn in PawnsFinder.AllMaps_FreeColonistsSpawned)
            {
                if (pawn.def != HarpyDefOf.Harpy &&
                    (pawn.needs.joy.CurLevelPercentage < 0.24000001f ||
                     pawn.GetTimeAssignment() == TimeAssignmentDefOf.Joy) &&
                    pawn.needs.joy.tolerances.BoredOfAllAvailableJoyKinds(pawn))
                {
                    ___boredPawnsResult.Add(pawn);
                }
            }

            __result = ___boredPawnsResult;
            return false;
        }
    }
}