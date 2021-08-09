using HarmonyLib;
using RimWorld;
using Verse;

namespace SyrHarpy
{
    // Token: 0x02000022 RID: 34
    public class SecondaryLovinChanceFactorPatch
    {
        // Token: 0x06000058 RID: 88 RVA: 0x00004334 File Offset: 0x00002534
        [HarmonyPrefix]
        public static bool SecondaryLovinChanceFactor_Prefix(ref float __result, Pawn otherPawn, Pawn ___pawn)
        {
            if (___pawn == null || otherPawn == null || ___pawn.def != HarpyDefOf.Harpy)
            {
                return true;
            }

            if (___pawn == otherPawn)
            {
                __result = 0f;
            }

            var ageBiologicalYearsFloat = ___pawn.ageTracker.AgeBiologicalYearsFloat;
            var ageBiologicalYearsFloat2 = otherPawn.ageTracker.AgeBiologicalYearsFloat;
            if (ageBiologicalYearsFloat < 16f || ageBiologicalYearsFloat2 < 16f)
            {
                __result = 0f;
            }

            var num = ageBiologicalYearsFloat - 20f;
            var num2 = ageBiologicalYearsFloat - 7f;
            var num3 = ageBiologicalYearsFloat + 7f;
            var num4 = ageBiologicalYearsFloat + 20f;
            var num5 = GenMath.FlatHill(0.2f, num, num2, num3, num4, 0.2f, ageBiologicalYearsFloat2);
            var num6 = 0f;
            if (otherPawn.RaceProps.Humanlike)
            {
                num6 = otherPawn.GetStatValue(StatDefOf.PawnBeauty);
            }

            var num7 = 1f;
            if (num6 < 0f)
            {
                num7 = 0.3f;
            }
            else if (num6 > 0f)
            {
                num7 = 2.3f;
            }

            __result = num5 * num7;
            return false;
        }
    }
}