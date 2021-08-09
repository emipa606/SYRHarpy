using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace SyrHarpy
{
    // Token: 0x0200000F RID: 15
    [HarmonyPatch(typeof(Pawn_MindState), "MindStateTick")]
    public static class MindStateTickPatch
    {
        // Token: 0x06000040 RID: 64 RVA: 0x000034E1 File Offset: 0x000016E1
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> MindStateTick_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var FlightCapabable = AccessTools.Method(typeof(MindStateTickPatch), "FlightCapabableOver");
            var pawn = AccessTools.Field(typeof(Pawn_MindState), "pawn");
            var found = false;
            var done = false;
            foreach (var i in instructions)
            {
                if (i.opcode == OpCodes.Ldloc_1 && !done)
                {
                    found = true;
                }
                else if (i.opcode == OpCodes.Brfalse_S && found)
                {
                    yield return i;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, pawn);
                    yield return new CodeInstruction(OpCodes.Call, FlightCapabable);
                    yield return new CodeInstruction(OpCodes.Brtrue_S, i.operand);
                    done = true;
                    found = false;
                    continue;
                }

                yield return i;
            }
        }

        // Token: 0x06000041 RID: 65 RVA: 0x000034F1 File Offset: 0x000016F1
        public static bool FlightCapabableOver(Pawn pawn)
        {
            return pawn.Position.GetTerrain(pawn.Map).traversedThought != null && HarpyUtility.FlightCapabable(pawn);
        }
    }
}