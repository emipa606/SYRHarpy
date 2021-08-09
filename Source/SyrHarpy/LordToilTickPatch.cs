using HarmonyLib;
using RimWorld;
using Verse;

namespace SyrHarpy
{
    // Token: 0x0200001B RID: 27
    [HarmonyPatch(typeof(LordToil_Party), "LordToilTick")]
    public static class LordToilTickPatch
    {
        // Token: 0x0600004E RID: 78 RVA: 0x00003E04 File Offset: 0x00002004
        [HarmonyPrefix]
        public static bool LordToilTick_Prefix(LordToil_Party __instance, IntVec3 ___spot, float ___joyPerTick)
        {
            var ownedPawns = __instance.lord.ownedPawns;
            foreach (var pawn in ownedPawns)
            {
                if (!GatheringsUtility.InGatheringArea(pawn.Position, ___spot, __instance.Map))
                {
                    continue;
                }

                if (pawn.needs.joy != null)
                {
                    pawn.needs.joy.GainJoy(___joyPerTick, JoyKindDefOf.Social);
                }

                var lordToilData_Party = __instance.data as LordToilData_Gathering;
                if (lordToilData_Party != null && !lordToilData_Party.presentForTicks.ContainsKey(pawn))
                {
                    lordToilData_Party.presentForTicks.Add(pawn, 0);
                }

                if (lordToilData_Party == null)
                {
                    continue;
                }

                var presentForTicks = lordToilData_Party.presentForTicks;
                var num = presentForTicks[pawn];
                presentForTicks[pawn] = num + 1;
            }

            return false;
        }
    }
}