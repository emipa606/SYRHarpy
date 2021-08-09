using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace SyrHarpy
{
    // Token: 0x02000018 RID: 24
    [HarmonyPatch(typeof(JoyUtility), "JoyTickCheckEnd")]
    public static class JoyTickCheckEndPatch
    {
        // Token: 0x0600004B RID: 75 RVA: 0x00003B84 File Offset: 0x00001D84
        [HarmonyPrefix]
        public static bool JoyTickCheckEnd_Prefix(Pawn pawn, JoyTickFullJoyAction fullJoyAction)
        {
            if (pawn == null || pawn.def != HarpyDefOf.Harpy)
            {
                return true;
            }

            var curJob = pawn.CurJob;
            if (curJob.def.joyKind == null)
            {
                Log.Warning("This method can only be called for jobs with joyKind.");
                return false;
            }

            if (curJob.def.joySkill != null)
            {
                pawn.skills.GetSkill(curJob.def.joySkill).Learn(curJob.def.joyXpPerTick);
            }

            if (!curJob.ignoreJoyTimeAssignment && !pawn.GetTimeAssignment().allowJoy)
            {
                pawn.jobs.curDriver.EndJobWith(JobCondition.InterruptForced);
            }

            if (curJob.startTick + 1200 >= Find.TickManager.TicksGame)
            {
                return false;
            }

            if (fullJoyAction == JoyTickFullJoyAction.EndJob)
            {
                pawn.jobs.curDriver.EndJobWith(JobCondition.Succeeded);
            }

            if (fullJoyAction == JoyTickFullJoyAction.GoToNextToil)
            {
                pawn.jobs.curDriver.ReadyForNextToil();
            }

            return false;
        }
    }
}