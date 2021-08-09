using System;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace SyrHarpy
{
    // Token: 0x02000007 RID: 7
    public class FlyAbilitySkyfaller : Skyfaller, IActiveDropPod
    {
        // Token: 0x04000035 RID: 53
        private bool anticipationSoundPlayed;

        // Token: 0x04000037 RID: 55
        private Material cachedShadowMaterial;

        // Token: 0x04000038 RID: 56
        public PawnRenderer skyfallerPawn;

        // Token: 0x04000036 RID: 54
        public int ticksToImpactMax;

        // Token: 0x1700000D RID: 13
        // (get) Token: 0x0600002A RID: 42 RVA: 0x00002C98 File Offset: 0x00000E98
        private float TimeInAnimation => 1f - (ticksToImpact / (float) ticksToImpactMax);

        // Token: 0x1700000E RID: 14
        // (get) Token: 0x0600002B RID: 43 RVA: 0x00002CB0 File Offset: 0x00000EB0
        private Material ShadowMaterial
        {
            get
            {
                if (cachedShadowMaterial == null && !def.skyfaller.shadow.NullOrEmpty())
                {
                    cachedShadowMaterial = MaterialPool.MatFrom(def.skyfaller.shadow, ShaderDatabase.Transparent);
                }

                return cachedShadowMaterial;
            }
        }

        // Token: 0x1700000C RID: 12
        // (get) Token: 0x06000028 RID: 40 RVA: 0x00002C67 File Offset: 0x00000E67
        // (set) Token: 0x06000029 RID: 41 RVA: 0x00002C7F File Offset: 0x00000E7F
        public ActiveDropPodInfo Contents
        {
            get => ((ActiveDropPod) innerContainer[0]).Contents;
            set => ((ActiveDropPod) innerContainer[0]).Contents = value;
        }

        // Token: 0x06000024 RID: 36 RVA: 0x00002968 File Offset: 0x00000B68
        public override void Tick()
        {
            innerContainer.ThingOwnerTick();
            ticksToImpact--;
            var drawPos = base.DrawPos;
            var pawn = GetThingForGraphic() as Pawn;
            drawPos.z += ((Mathf.Pow((2f * TimeInAnimation) - 1f, 2f) * -1f) + 1f) * (ticksToImpactMax / (float) 30);
            foreach (var intVec in GenAdjFast.AdjacentCells8Way(drawPos.ToIntVec3()))
            {
                Map.fogGrid.Unfog(intVec);
            }

            Map.fogGrid.Unfog(drawPos.ToIntVec3());
            if (ticksToImpact % 3 == 0)
            {
                var num = Math.Max(1, def.skyfaller.motesPerCell);
                for (var i = 0; i < num; i++)
                {
                    HarpyUtility.ThrowFeathersMote(drawPos, Map, pawn);
                }
            }

            if (ticksToImpact == 15)
            {
                base.HitRoof();
            }

            if (!anticipationSoundPlayed && def.skyfaller.anticipationSound != null &&
                ticksToImpact < def.skyfaller.anticipationSoundTicks)
            {
                anticipationSoundPlayed = true;
                def.skyfaller.anticipationSound.PlayOneShot(new TargetInfo(Position, Map));
            }

            if (ticksToImpact == 3)
            {
                EjectPilot();
            }

            if (ticksToImpact == 0)
            {
                base.Impact();
                return;
            }

            if (ticksToImpact >= 0)
            {
                return;
            }

            Log.Error("ticksToImpact < 0. Was there an exception? Destroying skyfaller.");
            EjectPilot();
            Destroy();
        }

        // Token: 0x06000025 RID: 37 RVA: 0x00002B40 File Offset: 0x00000D40
        private void EjectPilot()
        {
            var thingForGraphic = GetThingForGraphic();
            var comp = thingForGraphic.TryGetComp<HarpyComp>();
            if (thingForGraphic != null)
            {
                GenPlace.TryPlaceThing(thingForGraphic, Position, Map, ThingPlaceMode.Near, delegate(Thing t, int _)
                {
                    PawnUtility.RecoverFromUnwalkablePositionOrKill(t.Position, t.Map);
                    if (t.def.Fillage == FillCategory.Full && def.skyfaller.CausesExplosion &&
                        def.skyfaller.explosionDamage.isExplosive &&
                        t.Position.InHorDistOf(Position, def.skyfaller.explosionRadius))
                    {
                        Map.terrainGrid.Notify_TerrainDestroyed(t.Position);
                    }

                    CheckDrafting(t);
                    comp.TriggerCooldown();
                });
            }
        }

        // Token: 0x06000026 RID: 38 RVA: 0x00002B9C File Offset: 0x00000D9C
        internal void CheckDrafting(Thing thing)
        {
            if (thing == null || thing is not Pawn pawn)
            {
                return;
            }

            var harpyComp = pawn.TryGetComp<HarpyComp>();
            if (harpyComp == null || !harpyComp.drafted)
            {
                return;
            }

            harpyComp.drafted = false;
            pawn.drafter.Drafted = true;
        }

        // Token: 0x06000027 RID: 39 RVA: 0x00002BE0 File Offset: 0x00000DE0
        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            DrawDropSpotShadow(drawLoc, Rotation, ShadowMaterial, def.skyfaller.shadowSize, ticksToImpact);
            drawLoc.z += ((Mathf.Pow((2f * TimeInAnimation) - 1f, 2f) * -1f) + 1f) * (ticksToImpactMax / (float) 30);
            if (skyfallerPawn != null)
            {
                skyfallerPawn.RenderPawnAt(drawLoc);
            }
        }

        // Token: 0x0600002C RID: 44 RVA: 0x00002D08 File Offset: 0x00000F08
        private Thing GetThingForGraphic()
        {
            Thing thing = null;
            if (!innerContainer.Any || innerContainer.Count <= 0)
            {
                return null;
            }

            foreach (var thing2 in innerContainer)
            {
                if (thing2 is Pawn)
                {
                    thing = thing2;
                }
            }

            return thing as Pawn;
        }
    }
}