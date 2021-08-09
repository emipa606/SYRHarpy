using Verse;
using Verse.AI;

namespace SyrHarpy
{
    // Token: 0x02000031 RID: 49
    public class ThinkNode_ConditionalRace : ThinkNode_Conditional
    {
        // Token: 0x04000045 RID: 69
        public string race;

        // Token: 0x0600008A RID: 138 RVA: 0x000054DD File Offset: 0x000036DD
        protected override bool Satisfied(Pawn pawn)
        {
            return pawn.def.defName == race;
        }
    }
}