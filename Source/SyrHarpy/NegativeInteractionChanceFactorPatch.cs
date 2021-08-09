using HarmonyLib;
using RimWorld;
using Verse;

namespace SyrHarpy
{
    // Token: 0x0200000B RID: 11
    [HarmonyPatch(typeof(NegativeInteractionUtility), "NegativeInteractionChanceFactor")]
    public class NegativeInteractionChanceFactorPatch
    {
        // Token: 0x06000038 RID: 56 RVA: 0x0000330C File Offset: 0x0000150C
        [HarmonyPostfix]
        public static void NegativeInteractionChanceFactor_Postfix(ref float __result, Pawn initiator, Pawn recipient)
        {
            if (initiator != null && recipient != null && initiator.def == HarpyDefOf.Harpy &&
                recipient.def == HarpyDefOf.Harpy)
            {
                __result *= 2f;
            }
            else if (initiator != null && initiator.def == HarpyDefOf.Harpy)
            {
                __result *= 1.25f;
            }

            if (initiator == null)
            {
                return;
            }

            if (initiator.needs.TryGetNeed(HarpyDefOf.Bloodlust) is Need_Bloodlust {CurLevel: < 0.5f} need_Bloodlust)
            {
                __result *= 1f / (need_Bloodlust.CurLevel * 2f);
            }
        }
    }
}