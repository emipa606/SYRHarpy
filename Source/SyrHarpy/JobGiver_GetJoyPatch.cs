using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace SyrHarpy
{
    // Token: 0x0200001C RID: 28
    [HarmonyPatch(typeof(JobGiver_GetJoy), "TryGiveJob")]
    public static class JobGiver_GetJoyPatch
    {
        // Token: 0x0600004F RID: 79 RVA: 0x00003ED5 File Offset: 0x000020D5
        [HarmonyPrefix]
        public static bool JobGiver_GetJoy_Prefix(Job __result, Pawn pawn)
        {
            return pawn.def != HarpyDefOf.Harpy;
        }
    }
}