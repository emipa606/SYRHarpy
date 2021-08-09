using Verse;

namespace SyrHarpy
{
    // Token: 0x02000025 RID: 37
    public class CompProperties_HarpyComp : CompProperties
    {
        // Token: 0x04000041 RID: 65
        public float cooldown = 5f;

        // Token: 0x04000042 RID: 66
        public float range = 40f;

        // Token: 0x06000065 RID: 101 RVA: 0x000046E0 File Offset: 0x000028E0
        public CompProperties_HarpyComp()
        {
            compClass = typeof(HarpyComp);
        }
    }
}