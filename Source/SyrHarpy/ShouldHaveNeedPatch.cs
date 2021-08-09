using HarmonyLib;
using RimWorld;
using Verse;

namespace SyrHarpy
{
    // Token: 0x0200001D RID: 29
    [HarmonyPatch(typeof(Pawn_NeedsTracker), "ShouldHaveNeed")]
    public static class ShouldHaveNeedPatch
    {
        // Token: 0x06000050 RID: 80 RVA: 0x00003EE7 File Offset: 0x000020E7
        [HarmonyPostfix]
        public static void ShouldHaveNeed_Postfix(ref bool __result, Pawn_NeedsTracker __instance, Pawn ___pawn,
            NeedDef nd)
        {
            if (___pawn != null && ___pawn.def != HarpyDefOf.Harpy && nd == HarpyDefOf.Bloodlust)
            {
                __result = false;
            }

            if (___pawn != null && ___pawn.def == HarpyDefOf.Harpy && nd == NeedDefOf.Joy)
            {
                __result = false;
            }
        }
    }
}