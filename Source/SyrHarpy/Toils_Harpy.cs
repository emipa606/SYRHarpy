using RimWorld;
using Verse;
using Verse.AI;

namespace SyrHarpy
{
    // Token: 0x0200002D RID: 45
    public static class Toils_Harpy
    {
        // Token: 0x0600007F RID: 127 RVA: 0x00005238 File Offset: 0x00003438
        public static Toil GoToNearUnroofedCell(TargetIndex ind, float range)
        {
            var toil = new Toil();
            toil.initAction = delegate
            {
                var pawn = toil.actor;
                var map = pawn.Map;
                var moveCell = IntVec3.Invalid;

                bool PassCheck(IntVec3 c)
                {
                    return c.Walkable(map);
                }

                bool Processor(IntVec3 v)
                {
                    if (v.Roofed(map) || v.DistanceTo(pawn.jobs.curJob.GetTarget(ind).Cell) > range)
                    {
                        return false;
                    }

                    moveCell = v;
                    return true;
                }

                map.floodFiller.FloodFill(pawn.Position, PassCheck, Processor, 2000);
                if (moveCell != IntVec3.Invalid)
                {
                    pawn.pather.StartPath(moveCell, PathEndMode.OnCell);
                    return;
                }

                pawn.jobs.EndCurrentJob(JobCondition.Incompletable);
                Messages.Message("HarpyFly_RoofCaster".Translate(), pawn, MessageTypeDefOf.RejectInput, false);
            };
            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            return toil;
        }

        // Token: 0x06000080 RID: 128 RVA: 0x00005290 File Offset: 0x00003490
        public static Toil CastFlyAbility(TargetIndex ind)
        {
            var toil = new Toil();
            toil.initAction = delegate
            {
                var actor = toil.actor;
                var harpyComp = actor.TryGetComp<HarpyComp>();
                if (harpyComp != null)
                {
                    harpyComp.FlyAbility(actor, actor.jobs.curJob.GetTarget(ind).Cell);
                    return;
                }

                actor.jobs.EndCurrentJob(JobCondition.Incompletable);
            };
            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            return toil;
        }
    }
}