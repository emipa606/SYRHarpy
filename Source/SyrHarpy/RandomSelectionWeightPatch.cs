using RimWorld;
using UnityEngine;
using Verse;

namespace SyrHarpy
{
    // Token: 0x02000021 RID: 33
    public class RandomSelectionWeightPatch
    {
        // Token: 0x06000056 RID: 86 RVA: 0x00004230 File Offset: 0x00002430
        public static bool RandomSelectionWeight_Prefix(ref float __result, Pawn initiator, Pawn recipient)
        {
            if (initiator == null || recipient == null || initiator.def != HarpyDefOf.Harpy)
            {
                return true;
            }

            if (TutorSystem.TutorialMode)
            {
                __result = 0f;
            }

            if (LovePartnerRelationUtility.LovePartnerRelationExists(initiator, recipient))
            {
                __result = 0f;
            }

            var num = initiator.relations.SecondaryRomanceChanceFactor(recipient);
            if (num < 0.15f)
            {
                __result = 0f;
            }

            var num2 = initiator.relations.OpinionOf(recipient);
            if (num2 < 5)
            {
                __result = 0f;
            }

            if (recipient.relations.OpinionOf(initiator) < 5)
            {
                __result = 0f;
            }

            var num3 = 1f;
            var pawn = LovePartnerRelationUtility.ExistingMostLikedLovePartner(initiator, false);
            if (pawn != null)
            {
                float num4 = initiator.relations.OpinionOf(pawn);
                num3 = Mathf.InverseLerp(50f, -50f, num4);
            }

            var num5 = Mathf.InverseLerp(0.15f, 1f, num);
            var num6 = Mathf.InverseLerp(5f, 100f, num2);
            __result = 1.15f * num5 * num6 * num3;
            return false;
        }
    }
}