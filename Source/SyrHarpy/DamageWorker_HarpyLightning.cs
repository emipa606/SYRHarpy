using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace SyrHarpy
{
    // Token: 0x02000004 RID: 4
    public class DamageWorker_HarpyLightning : DamageWorker_AddInjury
    {
        // Token: 0x06000007 RID: 7 RVA: 0x0000225C File Offset: 0x0000045C
        protected override void ExplosionDamageThing(Explosion explosion, Thing t, List<Thing> damagedThings,
            List<Thing> ignoredThings, IntVec3 cell)
        {
            base.ExplosionDamageThing(explosion, t, damagedThings, ignoredThings, cell);
            var pawn = explosion.instigator as Pawn;
            if (pawn != null && t is Pawn pawn2 && pawn2.HostileTo(pawn) && HarpyUtility.HarpyAmplifierLevel(pawn) >= 4)
            {
                var hediff = HediffMaker.MakeHediff(HarpyDefOf.HarpyParalyzed, pawn2);
                hediff.Severity = 0.5f;
                pawn2.health.AddHediff(hediff);
            }

            if (pawn == null || t == null || HarpyUtility.HarpyAmplifierLevel(pawn) < 5)
            {
                return;
            }

            var damageInfo = new DamageInfo(DamageDefOf.EMP, 20f, 0f, -1f, pawn, null, explosion.weapon, 0,
                explosion.intendedTarget);
            t.TakeDamage(damageInfo).AssociateWithLog(new BattleLogEntry_ExplosionImpact(explosion.instigator, t,
                explosion.weapon, explosion.projectile, def));
        }

        // Token: 0x06000008 RID: 8 RVA: 0x00002334 File Offset: 0x00000534
        public override void ExplosionStart(Explosion explosion, List<IntVec3> cellsToAffect)
        {
            if (def.explosionHeatEnergyPerCell > 1.401298E-45f)
            {
                GenTemperature.PushHeat(explosion.Position, explosion.Map,
                    def.explosionHeatEnergyPerCell * cellsToAffect.Count);
            }

            FleckMaker.Static(explosion.Position, explosion.Map, FleckDefOf.ExplosionFlash, explosion.radius * 6f);
            ExplosionVisualEffectCenter(explosion);
        }

        // Token: 0x06000009 RID: 9 RVA: 0x000023A4 File Offset: 0x000005A4
        protected override void ExplosionVisualEffectCenter(Explosion explosion)
        {
            if (def.explosionInteriorMote == null)
            {
                return;
            }

            var num = Mathf.RoundToInt(3.14159274f * explosion.radius * explosion.radius / 6f);
            for (var i = 0; i < num; i++)
            {
                MoteMaker.ThrowExplosionInteriorMote(
                    explosion.Position.ToVector3Shifted() + Gen.RandomHorizontalVector(explosion.radius * 0.7f),
                    explosion.Map, def.explosionInteriorMote);
            }
        }
    }
}