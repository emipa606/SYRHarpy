using Verse;

namespace SyrHarpy
{
    // Token: 0x02000028 RID: 40
    public class Verb_Lightning : Verb_Shoot
    {
        // Token: 0x0600006C RID: 108 RVA: 0x00004B90 File Offset: 0x00002D90
        protected override bool TryCastShot()
        {
            var result = base.TryCastShot();
            var casterPawn = CasterPawn;
            var thing = currentTarget.Thing;
            if (casterPawn.Spawned)
            {
                casterPawn.Drawer.Notify_MeleeAttackOn(thing);
            }

            return result;
        }

        // Token: 0x0600006D RID: 109 RVA: 0x00004BCA File Offset: 0x00002DCA
        public override bool Available()
        {
            return base.Available() && !CasterPawn.InMentalState;
        }
    }
}