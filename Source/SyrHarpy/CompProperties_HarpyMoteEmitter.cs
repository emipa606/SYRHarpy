using Verse;

namespace SyrHarpy
{
    // Token: 0x02000003 RID: 3
    public class CompProperties_HarpyMoteEmitter : CompProperties
    {
        // Token: 0x04000008 RID: 8
        public IntRange angle;

        // Token: 0x04000004 RID: 4
        public int emissionInterval = -1;

        // Token: 0x04000003 RID: 3
        public ThingDef mote;

        // Token: 0x04000007 RID: 7
        public IntRange rotation;

        // Token: 0x04000006 RID: 6
        public IntRange rotationRate;

        // Token: 0x04000005 RID: 5
        public FloatRange scale;

        // Token: 0x04000009 RID: 9
        public FloatRange speed;

        // Token: 0x06000006 RID: 6 RVA: 0x0000223D File Offset: 0x0000043D
        public CompProperties_HarpyMoteEmitter()
        {
            compClass = typeof(Comp_HarpyMoteEmitter);
        }
    }
}