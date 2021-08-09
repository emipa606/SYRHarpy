using RimWorld;
using Verse;
using Verse.AI;

namespace SyrHarpy
{
    // Token: 0x02000030 RID: 48
    public class JobGiver_HarpyIdleJoy : JobGiver_HarpyGetJoy
    {
        // Token: 0x06000088 RID: 136 RVA: 0x000054A7 File Offset: 0x000036A7
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (Find.TickManager.TicksGame < 60000)
            {
                return null;
            }

            if (JoyUtility.LordPreventsGettingJoy(pawn) || JoyUtility.TimetablePreventsGettingJoy(pawn))
            {
                return null;
            }

            return base.TryGiveJob(pawn);
        }
    }
}