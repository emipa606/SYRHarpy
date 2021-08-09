using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace SyrHarpy
{
    // Token: 0x02000011 RID: 17
    [HarmonyPatch(typeof(PawnCapacityUtility), "CalculatePartEfficiency")]
    public class CalculatePartEfficiencyPatch
    {
        // Token: 0x06000043 RID: 67 RVA: 0x000035C8 File Offset: 0x000017C8
        [HarmonyPostfix]
        public static void CalculatePartEfficiency_Postfix(ref float __result, HediffSet diffSet, BodyPartRecord part,
            bool ignoreAddedParts = false, List<PawnCapacityUtility.CapacityImpactor> impactors = null)
        {
            if (part == null || part.def != HarpyDefOf.WingHarpy && part.def != HarpyDefOf.HandClawsHarpy &&
                part.def != HarpyDefOf.FootClawsHarpy)
            {
                return;
            }

            if (part.parent != null && diffSet.PartIsMissing(part.parent))
            {
                __result = 0f;
            }

            var num = 1f;
            if (!ignoreAddedParts)
            {
                foreach (var hediff in diffSet.hediffs)
                {
                    if (hediff is not Hediff_AddedPart hediff_AddedPart || hediff_AddedPart.Part != part)
                    {
                        continue;
                    }

                    num *= hediff_AddedPart.def.addedPartProps.partEfficiency;
                    if (hediff_AddedPart.def.addedPartProps.partEfficiency != 1f && impactors != null)
                    {
                        impactors.Add(new PawnCapacityUtility.CapacityImpactorHediff
                        {
                            hediff = hediff_AddedPart
                        });
                    }
                }
            }

            var num2 = -1f;
            var num3 = 0f;
            var ignoreMissingHp = false;
            foreach (var hediff in diffSet.hediffs)
            {
                if (hediff.Part != part || hediff.CurStage == null)
                {
                    continue;
                }

                var curStage = hediff.CurStage;
                num3 += curStage.partEfficiencyOffset;
                ignoreMissingHp |= curStage.partIgnoreMissingHP;
                if (curStage.partEfficiencyOffset != 0f && curStage.becomeVisible && impactors != null)
                {
                    impactors.Add(new PawnCapacityUtility.CapacityImpactorHediff
                    {
                        hediff = hediff
                    });
                }
            }

            if (!ignoreMissingHp)
            {
                var num4 = diffSet.GetPartHealth(part) / part.def.GetMaxHealth(diffSet.pawn);
                if (num4 != 1f)
                {
                    if (DamageWorker_AddInjury.ShouldReduceDamageToPreservePart(part))
                    {
                        num4 = Mathf.InverseLerp(0.1f, 1f, num4);
                    }

                    if (impactors != null)
                    {
                        impactors.Add(new PawnCapacityUtility.CapacityImpactorBodyPartHealth
                        {
                            bodyPart = part
                        });
                    }

                    num *= num4;
                }
            }

            num += num3;
            if (num > 0.0001f)
            {
                num = Mathf.Max(num, num2);
            }

            __result = Mathf.Max(num, 0f);
        }
    }
}