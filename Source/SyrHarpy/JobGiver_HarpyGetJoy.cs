using RimWorld;
using Verse;
using Verse.AI;

namespace SyrHarpy
{
    // Token: 0x0200002E RID: 46
    public class JobGiver_HarpyGetJoy : ThinkNode_JobGiver
    {
        // Token: 0x04000044 RID: 68
        [Unsaved] private DefMap<JoyGiverDef, float> joyGiverChances;

        // Token: 0x06000081 RID: 129 RVA: 0x000052DE File Offset: 0x000034DE
        public override void ResolveReferences()
        {
            joyGiverChances = new DefMap<JoyGiverDef, float>();
        }

        // Token: 0x06000082 RID: 130 RVA: 0x000052EB File Offset: 0x000034EB
        protected virtual Job TryGiveJobFromJoyGiverDefDirect(JoyGiverDef def, Pawn pawn)
        {
            return def.Worker.TryGiveJob(pawn);
        }

        // Token: 0x06000083 RID: 131 RVA: 0x000052FC File Offset: 0x000034FC
        protected override Job TryGiveJob(Pawn pawn)
        {
            var allDefsListForReading = DefDatabase<JoyGiverDef>.AllDefsListForReading;
            foreach (var joyGiverDef in allDefsListForReading)
            {
                joyGiverChances[joyGiverDef] = 0f;
                if (joyGiverDef.Worker.CanBeGivenTo(pawn) && joyGiverDef.giverClass != typeof(JoyGiver_TakeDrug))
                {
                    if (joyGiverDef.pctPawnsEverDo < 1.0)
                    {
                        Rand.PushState(pawn.thingIDNumber ^ 63216713);
                        if (Rand.Value >= joyGiverDef.pctPawnsEverDo)
                        {
                            Rand.PopState();
                            goto IL_DC;
                        }

                        Rand.PopState();
                    }

                    joyGiverChances[joyGiverDef] = joyGiverDef.Worker.GetChance(pawn);
                    if (joyGiverDef.giverClass == typeof(JoyGiver_SocialRelax))
                    {
                        var defMap = joyGiverChances;
                        defMap[joyGiverDef] *= 3f;
                    }
                }

                IL_DC: ;
            }

            var num = 0;
            while (num < joyGiverChances.Count &&
                   allDefsListForReading.TryRandomElementByWeight(d => joyGiverChances[d], out var joyGiverDef3))
            {
                var job = TryGiveJobFromJoyGiverDefDirect(joyGiverDef3, pawn);
                if (job != null)
                {
                    return job;
                }

                joyGiverChances[joyGiverDef3] = 0f;
                num++;
            }

            return null;
        }
    }
}