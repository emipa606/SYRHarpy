using UnityEngine;
using Verse;

namespace SyrHarpy
{
    // Token: 0x02000002 RID: 2
    public class Comp_HarpyMoteEmitter : ThingComp
    {
        // Token: 0x04000002 RID: 2
        protected Mote mote;

        // Token: 0x04000001 RID: 1
        public int ticksSinceLastEmitted;

        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        private CompProperties_HarpyMoteEmitter Props => (CompProperties_HarpyMoteEmitter) props;

        // Token: 0x06000002 RID: 2 RVA: 0x00002060 File Offset: 0x00000260
        public override void CompTick()
        {
            if (Props.emissionInterval == -1)
            {
                if (mote == null)
                {
                    Emit();
                }

                return;
            }

            if (ticksSinceLastEmitted >= Props.emissionInterval)
            {
                Emit();
                ticksSinceLastEmitted = 0;
                return;
            }

            ticksSinceLastEmitted++;
        }

        // Token: 0x06000003 RID: 3 RVA: 0x000020BC File Offset: 0x000002BC
        protected void Emit()
        {
            mote = HarpyMote(parent.DrawPos, parent.Map, Props.mote, Props.scale, Props.rotationRate, Props.rotation,
                Props.angle, Props.speed);
        }

        // Token: 0x06000004 RID: 4 RVA: 0x0000212C File Offset: 0x0000032C
        public static Mote HarpyMote(Vector3 loc, Map map, ThingDef moteDef, FloatRange scale, IntRange rotationRate,
            IntRange rotation, IntRange angle, FloatRange speed)
        {
            if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
            {
                return null;
            }

            var moteThrown = (MoteThrown) ThingMaker.MakeThing(moteDef);
            moteThrown.Scale = Rand.Range(scale.min, scale.max);
            moteThrown.rotationRate = Rand.Range(rotationRate.min, rotationRate.max);
            moteThrown.exactRotation = Rand.Range(rotation.min, rotation.max);
            moteThrown.exactPosition = loc;
            moteThrown.exactPosition -= new Vector3(0.5f, 0f, 0.5f);
            moteThrown.exactPosition += new Vector3(Rand.Value, 0f, Rand.Value);
            moteThrown.SetVelocity(Rand.Range(angle.min, angle.max), Rand.Range(speed.min, speed.max));
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map);
            return moteThrown;
        }
    }
}