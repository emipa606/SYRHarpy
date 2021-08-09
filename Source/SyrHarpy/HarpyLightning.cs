using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace SyrHarpy
{
    // Token: 0x02000026 RID: 38
    public class HarpyLightning : Bullet
    {
        // Token: 0x04000043 RID: 67
        private int rotation;

        // Token: 0x06000066 RID: 102 RVA: 0x00004710 File Offset: 0x00002910
        public override void Draw()
        {
            Graphics.DrawMesh(MeshPool.GridPlane(def.graphicData.drawSize), DrawPos, Quaternion.Euler(0f, rotation, 0f),
                def.DrawMatSingle, 0);
            Comps_PostDraw();
        }

        // Token: 0x06000067 RID: 103 RVA: 0x00004765 File Offset: 0x00002965
        public override void Tick()
        {
            base.Tick();
            rotation = Rand.Range(0, 360);
        }

        // Token: 0x06000068 RID: 104 RVA: 0x00004780 File Offset: 0x00002980
        protected override void Impact(Thing hitThing)
        {
            var map = Map;
            var vector = usedTarget.Thing?.TrueCenter() ?? Position.ToVector3();

            var harpyMote = (HarpyMote) ThingMaker.MakeThing(HarpyDefOf.HarpyMote_LightningThree);
            harpyMote.Scale = Rand.Range(1f, 1.4f);
            harpyMote.exactRotation = Rand.Range(-180, 180);
            harpyMote.exactPosition = vector;
            GenSpawn.Spawn(harpyMote, vector.ToIntVec3(), map);
            MoteMaker.MakeStaticMote(vector.ToIntVec3(), map, HarpyDefOf.HarpyMote_DistortionImpact);
            HarpyDefOf.Harpy_LightningImpact.PlayOneShot(new TargetInfo(vector.ToIntVec3(), map));
            GenClamor.DoClamor(this, 2.1f, ClamorDefOf.Impact);
            Destroy();
            var num = 0f;
            var num2 = 0;
            if (launcher is Pawn pawn)
            {
                num2 = HarpyUtility.HarpyAmplifierLevel(pawn);
                num = 14 + (2 * num2);
            }

            if (num2 >= 6)
            {
                GenExplosion.DoExplosion(Position, map, 2.4f, HarpyDefOf.HarpyLightning, launcher, (int) (num * 0.25f),
                    0.5f, null, equipmentDef, def, intendedTarget.Thing);
                num *= 0.75f;
            }

            if (hitThing != null)
            {
                var battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(launcher, hitThing,
                    intendedTarget.Thing, equipmentDef, def, targetCoverDef);
                Find.BattleLog.Add(battleLogEntry_RangedImpact);
                var damageInfo = new DamageInfo(def.projectile.damageDef, num, ArmorPenetration,
                    ExactRotation.eulerAngles.y, launcher, null, equipmentDef, 0, intendedTarget.Thing);
                hitThing.TakeDamage(damageInfo).AssociateWithLog(battleLogEntry_RangedImpact);
                if (hitThing is Pawn pawn2)
                {
                    if (pawn2.stances != null && pawn2.BodySize <= def.projectile.StoppingPower + 0.001f)
                    {
                        pawn2.stances.StaggerFor((int) (def.projectile.StoppingPower - (pawn2.BodySize * 30f)));
                    }

                    if (num2 >= 4)
                    {
                        var hediff = HediffMaker.MakeHediff(HarpyDefOf.HarpyParalyzed, pawn2);
                        hediff.Severity = 1f;
                        pawn2.health.AddHediff(hediff);
                    }
                }

                if (def.projectile.extraDamages != null)
                {
                    foreach (var extraDamage in def.projectile.extraDamages)
                    {
                        if (!Rand.Chance(extraDamage.chance))
                        {
                            continue;
                        }

                        var damageInfo2 = new DamageInfo(extraDamage.def, extraDamage.amount,
                            extraDamage.AdjustedArmorPenetration(), ExactRotation.eulerAngles.y, launcher, null,
                            equipmentDef, 0, intendedTarget.Thing);
                        hitThing.TakeDamage(damageInfo2).AssociateWithLog(battleLogEntry_RangedImpact);
                    }
                }
            }

            SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(Position, map));
            if (Position.GetTerrain(map).takeSplashes)
            {
                FleckMaker.WaterSplash(ExactPosition, map, Mathf.Sqrt(DamageAmount) * 1f, 6f);
                return;
            }

            FleckMaker.Static(ExactPosition, map, FleckDefOf.ShotHit_Dirt, 3f);
        }
    }
}