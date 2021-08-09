using HarmonyLib;
using RimWorld;
using Verse;

namespace SyrHarpy
{
    // Token: 0x02000012 RID: 18
    [HarmonyPatch(typeof(ITab_Pawn_Gear), "DrawThingRow")]
    public static class DrawThingRowPatch
    {
        // Token: 0x06000045 RID: 69 RVA: 0x000037F0 File Offset: 0x000019F0
        [HarmonyPrefix]
        public static bool DrawThingRow_Prefix(ITab_Pawn_Gear __instance, ref float y, float width, Thing thing,
            bool inventory = false)
        {
            var pawn = (Pawn) AccessTools.Property(typeof(ITab_Pawn_Gear), "SelPawnForGear").GetGetMethod(true)
                .Invoke(__instance, null);
            return pawn == null || pawn.def != HarpyDefOf.Harpy || thing == null ||
                   !HarpyUtility.IsHarpyLightningWeapon(thing.def);
        }
    }
}