using Verse;

namespace SyrHarpy
{
    // Token: 0x02000027 RID: 39
    public class HarpyMote : MoteThrown
    {
        // Token: 0x0600006A RID: 106 RVA: 0x00004B68 File Offset: 0x00002D68
        public override void Tick()
        {
            exactRotation = Rand.Range(-180, 180);
            base.Tick();
        }
    }
}