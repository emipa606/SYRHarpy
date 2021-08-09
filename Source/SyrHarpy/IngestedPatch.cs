using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SyrHarpy
{
    // Token: 0x02000009 RID: 9
    [HarmonyPatch(typeof(Thing), "Ingested")]
    public class IngestedPatch
    {
        // Token: 0x06000032 RID: 50 RVA: 0x00002FAF File Offset: 0x000011AF
        [HarmonyPrefix]
        public static void Ingested_Prefix(Thing __instance, Pawn ingester, float nutritionWanted, ref int __state)
        {
            if (ingester != null && __instance.def == HarpyDefOf.RawHarpyChilis && ingester.def != HarpyDefOf.Harpy)
            {
                __state = IngestedCalculateStackCount(__instance, ingester, nutritionWanted);
            }
        }

        // Token: 0x06000033 RID: 51 RVA: 0x00002FD8 File Offset: 0x000011D8
        [HarmonyPostfix]
        public static void Ingested_Postfix(Thing __instance, Pawn ingester, float nutritionWanted, int __state)
        {
            if (ingester == null)
            {
                return;
            }

            var compIngredients = __instance.TryGetComp<CompIngredients>();
            var need_Bloodlust = ingester.needs.TryGetNeed(HarpyDefOf.Bloodlust) as Need_Bloodlust;
            if (compIngredients?.ingredients != null)
            {
                if (compIngredients.ingredients.Contains(HarpyDefOf.RawHarpyChilis) && ingester.def != HarpyDefOf.Harpy)
                {
                    var hediff = HediffMaker.MakeHediff(HarpyDefOf.HarpyChiliBurn, ingester);
                    hediff.Severity = 0.4f;
                    ingester.health.AddHediff(hediff);
                }
                else if (compIngredients.ingredients.Contains(HarpyDefOf.RawHarpyChilis) &&
                         compIngredients.ingredients.Contains(ThingDefOf.Meat_Human) &&
                         ingester.def == HarpyDefOf.Harpy && need_Bloodlust != null)
                {
                    need_Bloodlust.GainBloodlust(0.1f);
                    ingester.needs.mood.thoughts.memories.TryGainMemory(HarpyDefOf.AteHarpySpecial);
                }
                else if (compIngredients.ingredients.Contains(HarpyDefOf.RawHarpyChilis) &&
                         ingester.def == HarpyDefOf.Harpy && need_Bloodlust != null)
                {
                    need_Bloodlust.GainBloodlust(0.05f);
                    ingester.needs.mood.thoughts.memories.TryGainMemory(HarpyDefOf.AteHarpyChili);
                }
                else if (compIngredients.ingredients.Contains(ThingDefOf.Meat_Human) &&
                         ingester.def == HarpyDefOf.Harpy && need_Bloodlust != null)
                {
                    need_Bloodlust.GainBloodlust(0.05f);
                    ingester.needs.mood.thoughts.memories.TryGainMemory(HarpyDefOf.AteHumanMeatAsHarpy);
                }
            }

            if (__instance.def == HarpyDefOf.RawHarpyChilis && ingester.def != HarpyDefOf.Harpy &&
                ingester.HealthScale > 0.4f)
            {
                var hediff2 = HediffMaker.MakeHediff(HarpyDefOf.HarpyChiliBurn, ingester);
                hediff2.Severity = __state * 0.05f / ingester.BodySize;
                ingester.health.AddHediff(hediff2);
                return;
            }

            bool isPawnMeat;
            var def = __instance.def;
            if (def == null)
            {
                isPawnMeat = false;
            }
            else
            {
                var ingestible = def.ingestible;
                if (ingestible == null)
                {
                    isPawnMeat = false;
                }
                else
                {
                    var sourceDef = ingestible.sourceDef;
                    isPawnMeat = sourceDef?.race != null;
                }
            }

            if (isPawnMeat && __instance.def.ingestible.sourceDef == ThingDefOf.Human &&
                ingester.def == HarpyDefOf.Harpy && need_Bloodlust != null)
            {
                need_Bloodlust.GainBloodlust(0.05f);
                ingester.needs.mood.thoughts.memories.TryGainMemory(HarpyDefOf.AteHumanMeatAsHarpy);
                return;
            }

            if (__instance.def != HarpyDefOf.RawHarpyChilis || ingester.def != HarpyDefOf.Harpy ||
                need_Bloodlust == null)
            {
                return;
            }

            need_Bloodlust.GainBloodlust(0.05f);
            ingester.needs.mood.thoughts.memories.TryGainMemory(HarpyDefOf.AteHarpyChili);
        }

        // Token: 0x06000034 RID: 52 RVA: 0x00003278 File Offset: 0x00001478
        public static int IngestedCalculateStackCount(Thing thing, Pawn ingester, float nutritionWanted)
        {
            var num = Mathf.CeilToInt(nutritionWanted / thing.GetStatValue(StatDefOf.Nutrition));
            num = Mathf.Min(num, thing.def.ingestible.maxNumToIngestAtOnce, thing.stackCount);
            return Mathf.Max(num, 1);
        }
    }
}