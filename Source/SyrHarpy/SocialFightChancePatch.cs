using HarmonyLib;
using RimWorld;
using Verse;

namespace SyrHarpy
{
    // Token: 0x0200000C RID: 12
    [HarmonyPatch(typeof(Pawn_InteractionsTracker), "SocialFightChance")]
    public class SocialFightChancePatch
    {
        // Token: 0x0600003A RID: 58 RVA: 0x000033A4 File Offset: 0x000015A4
        [HarmonyPostfix]
        public static void SocialFightChance_Postfix(ref float __result, InteractionDef interaction, Pawn initiator,
            Pawn ___pawn)
        {
            if (___pawn != null && initiator != null && ___pawn.def == HarpyDefOf.Harpy &&
                initiator.def == HarpyDefOf.Harpy)
            {
                __result *= 4f;
                return;
            }

            if (___pawn != null && ___pawn.def == HarpyDefOf.Harpy)
            {
                __result *= 2f;
            }
        }
    }
}