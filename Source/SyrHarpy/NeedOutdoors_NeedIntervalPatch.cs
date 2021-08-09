using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SyrHarpy
{
    // Token: 0x02000017 RID: 23
    [HarmonyPatch(typeof(Need_Outdoors), "NeedInterval")]
    public static class NeedOutdoors_NeedIntervalPatch
    {
        // Token: 0x0600004A RID: 74 RVA: 0x00003A80 File Offset: 0x00001C80
        [HarmonyPostfix]
        public static void NeedOutdoors_NeedInterval_Postfix(Need_Outdoors __instance, Pawn ___pawn)
        {
            if (___pawn == null)
            {
                return;
            }

            if (___pawn.def != HarpyDefOf.Harpy)
            {
                return;
            }

            var num = 0.2f;
            var roofDef = ___pawn.Spawned ? ___pawn.Position.GetRoof(___pawn.Map) : null;
            float num2;
            if (roofDef != null && !___pawn.Position.UsesOutdoorTemperature(___pawn.Map))
            {
                if (!roofDef.isThickRoof)
                {
                    num2 = -0.32f;
                }
                else
                {
                    num2 = -0.45f;
                    num = 0f;
                }
            }
            else if (roofDef is {isThickRoof: true})
            {
                num2 = -0.4f;
            }
            else
            {
                num2 = 0f;
            }

            if (___pawn.InBed() && num2 < 0f)
            {
                num2 *= 0.2f;
            }

            num2 *= 0.0025f;
            if (num2 < 0f)
            {
                __instance.CurLevel =
                    Mathf.Min(__instance.CurLevel, Mathf.Max(__instance.CurLevel + num2, num));
                return;
            }

            __instance.CurLevel = Mathf.Min(__instance.CurLevel + num2, 1f);
        }
    }
}