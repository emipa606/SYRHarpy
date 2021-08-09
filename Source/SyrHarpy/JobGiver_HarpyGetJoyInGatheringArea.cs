using RimWorld;
using Verse;
using Verse.AI;

namespace SyrHarpy
{
    // Token: 0x0200002F RID: 47
    public class JobGiver_HarpyGetJoyInGatheringArea : JobGiver_HarpyGetJoy
    {
        // Token: 0x06000086 RID: 134 RVA: 0x00005460 File Offset: 0x00003660
        protected override Job TryGiveJobFromJoyGiverDefDirect(JoyGiverDef def, Pawn pawn)
        {
            if (pawn.mindState.duty == null)
            {
                return null;
            }

            var cell = pawn.mindState.duty.focus.Cell;
            return def.Worker.TryGiveJobInGatheringArea(pawn, cell);
        }
    }
}