using HarmonyLib;
using RimWorld;
using Verse;

namespace SyrHarpy
{
    // Token: 0x0200000A RID: 10
    [HarmonyPatch(typeof(Designator_ZoneAdd_Growing), "CanDesignateCell")]
    public class GrowingZone_CanDesignateCellPatch
    {
        // Token: 0x06000036 RID: 54 RVA: 0x000032D4 File Offset: 0x000014D4
        [HarmonyPostfix]
        public static void GrowingZone_CanDesignateCell_Postfix(ref AcceptanceReport __result,
            Designator_ZoneAdd_Growing __instance, IntVec3 c)
        {
            if (__instance.Map.fertilityGrid.FertilityAt(c) >= HarpyDefOf.Plant_HarpyChili.plant.fertilityMin)
            {
                __result = true;
            }
        }
    }
}