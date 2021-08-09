using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace SyrHarpy
{
    // Token: 0x0200000E RID: 14
    [HarmonyPatch(typeof(PathFinder), "FindPath", typeof(IntVec3), typeof(LocalTargetInfo), typeof(TraverseParms),
        typeof(PathEndMode), typeof(PathFinderCostTuning))]
    public static class FindPathPatch
    {
        // Token: 0x0600003E RID: 62 RVA: 0x00003455 File Offset: 0x00001655
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> FindPath_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionList = instructions.ToList();
            var TerrainPathCost = AccessTools.Method(typeof(FindPathPatch), "TerrainPathCost");
            int num;
            for (var i = 0; i < instructionList.Count; i = num + 1)
            {
                var instruction = instructionList[i];
                if (instruction.opcode == OpCodes.Stloc_S)
                {
                    if (instruction.operand is LocalBuilder {LocalIndex: 41} &&
                        instructionList[i - 2].opcode == OpCodes.Ldelem_I4)
                    {
                        yield return instruction;
                        yield return new CodeInstruction(OpCodes.Ldloc_S, 41);
                        yield return new CodeInstruction(OpCodes.Ldloc_S, 38);
                        yield return new CodeInstruction(OpCodes.Ldloc_S, 12);
                        yield return new CodeInstruction(OpCodes.Ldarg_3);
                        yield return new CodeInstruction(OpCodes.Call, TerrainPathCost);
                        instruction = instruction.Clone();
                    }
                }

                yield return instruction;
                num = i;
            }
        }

        // Token: 0x0600003F RID: 63 RVA: 0x00003468 File Offset: 0x00001668
        public static int TerrainPathCost(int cost, int nextIndex, TerrainDef[] terrainDef, TraverseParms parms)
        {
            var pawn = parms.pawn;
            if (pawn?.def == null || parms.pawn.def != HarpyDefOf.Harpy ||
                !HarpyUtility.FlightCapabable(parms.pawn) || terrainDef[nextIndex] == null)
            {
                return cost;
            }

            cost -= terrainDef[nextIndex].pathCost;
            if (parms.pawn.Drafted)
            {
                cost -= terrainDef[nextIndex].extraDraftedPerceivedPathCost;
            }
            else
            {
                cost -= terrainDef[nextIndex].extraNonDraftedPerceivedPathCost;
            }

            return cost;
        }
    }
}