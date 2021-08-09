using HarmonyLib;
using Verse;
using Verse.AI;

namespace SyrHarpy
{
    // Token: 0x0200000D RID: 13
    [HarmonyPatch(typeof(Pawn_PathFollower), "CostToMoveIntoCell", typeof(Pawn), typeof(IntVec3))]
    public class CostToMoveIntoCellPatch
    {
        // Token: 0x0600003C RID: 60 RVA: 0x00003400 File Offset: 0x00001600
        [HarmonyPostfix]
        public static void CostToMoveIntoCell_Postfix(ref int __result, Pawn pawn, IntVec3 c)
        {
            if (pawn?.Map != null && pawn.def == HarpyDefOf.Harpy &&
                HarpyUtility.FlightCapabable(pawn))
            {
                __result -= pawn.Map.pathing.For(pawn).pathGrid.CalculatedCostAt(c, false, pawn.Position);
            }
        }
    }
}