using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SyrHarpy
{
    // Token: 0x02000020 RID: 32
    [HarmonyPatch(typeof(PawnTechHediffsGenerator), "GenerateTechHediffsFor")]
    public static class GenerateTechHediffsForPatch
    {
        // Token: 0x06000055 RID: 85 RVA: 0x00003F98 File Offset: 0x00002198
        [HarmonyPostfix]
        public static void GenerateTechHediffsFor_Postfix(Pawn pawn)
        {
            if (pawn?.kindDef == null)
            {
                return;
            }

            var modExtension = pawn.kindDef.GetModExtension<SyrHarpyExtension>();
            if (modExtension?.requiredProsthetics == null)
            {
                return;
            }

            using var enumerator = modExtension.requiredProsthetics.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var thc = enumerator.Current;
                var enumerable = from x in DefDatabase<RecipeDef>.AllDefs
                    where thc != null && x.addsHediff == thc.hediffDef && pawn.def.AllRecipes.Contains(x)
                    select x;
                if (enumerable.Any())
                {
                    var recipeDef = enumerable.RandomElement();
                    if (thc != null)
                    {
                        for (var i = 0; i < thc.count; i++)
                        {
                            if (recipeDef.Worker.GetPartsToApplyOn(pawn, recipeDef).Any())
                            {
                                recipeDef.Worker.ApplyOnPawn(pawn,
                                    recipeDef.Worker.GetPartsToApplyOn(pawn, recipeDef).RandomElement(), null,
                                    new List<Thing>(), null);
                            }
                        }
                    }
                }

                if (thc != null && thc.hediffDef.hediffClass != typeof(Hediff_Level))
                {
                    continue;
                }

                var enumerable2 = from t in DefDatabase<ThingDef>.AllDefs
                    where thc != null && (t.HasComp(typeof(CompUseEffect_InstallImplant)) ||
                                          t.HasComp(typeof(CompUseEffect_LightningImplant))) &&
                          t.GetCompProperties<CompProperties_UseEffectInstallImplant>().hediffDef ==
                          thc.hediffDef
                    select t;
                if (!enumerable2.Any())
                {
                    continue;
                }

                var thingDef = enumerable2.RandomElement();
                var bodyPartRecord = pawn.health.hediffSet.GetNotMissingParts().First(x =>
                    x.def == thingDef.GetCompProperties<CompProperties_UseEffectInstallImplant>()
                        .bodyPart);
                if (thc == null)
                {
                    continue;
                }

                var num = (int) Mathf.Clamp(thc.count, thc.hediffDef.minSeverity,
                    thc.hediffDef.maxSeverity);
                if (num > 0)
                {
                    if (HediffMaker.MakeHediff(thc.hediffDef, pawn, bodyPartRecord) is Hediff_Level Hediff_Level)
                    {
                        Hediff_Level.level = num;
                        pawn.health.AddHediff(Hediff_Level);
                    }
                }

                if (thingDef.HasComp(typeof(CompUseEffect_LightningImplant)))
                {
                    HarpyUtility.SwapLightningWeapon(pawn, num);
                }
            }
        }
    }
}