using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace SyrHarpy
{
    // Token: 0x0200002C RID: 44
    public class JobDriver_FlyAbility : JobDriver
    {
        // Token: 0x0600007C RID: 124 RVA: 0x0000521C File Offset: 0x0000341C
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        // Token: 0x0600007D RID: 125 RVA: 0x0000521F File Offset: 0x0000341F
        protected override IEnumerable<Toil> MakeNewToils()
        {
            var harpyComp = pawn.TryGetComp<HarpyComp>();
            if (harpyComp == null)
            {
                yield break;
            }

            if (pawn.Position.Roofed(pawn.Map))
            {
                yield return Toils_Harpy.GoToNearUnroofedCell(TargetIndex.A, harpyComp.AdjustedRange);
            }

            yield return Toils_Harpy.CastFlyAbility(TargetIndex.A);
        }
    }
}