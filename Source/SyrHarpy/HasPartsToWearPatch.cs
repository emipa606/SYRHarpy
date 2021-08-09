using HarmonyLib;
using RimWorld;
using Verse;

namespace SyrHarpy
{
    // Token: 0x02000010 RID: 16
    [HarmonyPatch(typeof(ApparelUtility), "HasPartsToWear")]
    public static class HasPartsToWearPatch
    {
        // Token: 0x06000042 RID: 66 RVA: 0x00003514 File Offset: 0x00001714
        [HarmonyPostfix]
        public static void HasPartsToWear_Postfix(ref bool __result, Pawn p, ThingDef apparel)
        {
            bool hasApparel;
            if (apparel == null)
            {
                hasApparel = false;
            }
            else
            {
                var apparel2 = apparel.apparel;
                hasApparel = apparel2?.bodyPartGroups != null;
            }

            if (!hasApparel || p?.def == null)
            {
                return;
            }

            if (apparel.apparel.bodyPartGroups.Contains(HarpyDefOf.Feet) &&
                !apparel.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Legs) && p.def == HarpyDefOf.Harpy)
            {
                __result = false;
            }

            if (apparel.apparel.bodyPartGroups.Contains(HarpyDefOf.Hands) &&
                !apparel.apparel.bodyPartGroups.Contains(HarpyDefOf.Arms) && p.def == HarpyDefOf.Harpy)
            {
                __result = false;
            }
        }
    }
}