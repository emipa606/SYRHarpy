using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace SyrHarpy
{
    // Token: 0x02000024 RID: 36
    public class HarpyComp : ThingComp
    {
        // Token: 0x0400003C RID: 60
        public float cooldownTicks;

        // Token: 0x0400003D RID: 61
        public bool drafted;

        // Token: 0x04000040 RID: 64
        public Dictionary<ThingDef, int> drugsNursedToday = new Dictionary<ThingDef, int>();

        // Token: 0x0400003F RID: 63
        public bool soundQueued;

        // Token: 0x0400003E RID: 62
        public int timeToImpactMax;

        // Token: 0x17000012 RID: 18
        // (get) Token: 0x0600005B RID: 91 RVA: 0x0000445D File Offset: 0x0000265D
        public CompProperties_HarpyComp Props => (CompProperties_HarpyComp) props;

        // Token: 0x17000013 RID: 19
        // (get) Token: 0x0600005D RID: 93 RVA: 0x000044D4 File Offset: 0x000026D4
        public float AdjustedRange
        {
            get
            {
                if (parent is Pawn pawn)
                {
                    return Props.range * HarpyUtility.FlightCapability(pawn) / 2f;
                }

                return Props.range;
            }
        }

        // Token: 0x0600005C RID: 92 RVA: 0x0000446C File Offset: 0x0000266C
        public override void CompTick()
        {
            base.CompTick();
            if (cooldownTicks > 0f)
            {
                cooldownTicks -= 1f;
            }

            if (!soundQueued)
            {
                return;
            }

            HarpyDefOf.Harpy_FlySound.PlayOneShot(new TargetInfo(parent.Position, Find.CurrentMap));
            soundQueued = false;
        }

        // Token: 0x0600005E RID: 94 RVA: 0x00004514 File Offset: 0x00002714
        public void TriggerCooldown()
        {
            cooldownTicks = Props.cooldown * 60f;
        }

        // Token: 0x0600005F RID: 95 RVA: 0x0000452D File Offset: 0x0000272D
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            var pawn = parent as Pawn;
            if (pawn?.Map == null || !pawn.IsColonistPlayerControlled || pawn.Downed ||
                !pawn.Spawned || Find.Selector.SingleSelectedThing != pawn)
            {
                yield break;
            }

            string defaultLabel = "HarpyFlyLabel".Translate();
            string defaultDesc = "HarpyFlyDescription".Translate();
            var icon = ContentFinder<Texture2D>.Get(Command_HarpyFly.IconPath);
            yield return new Command_HarpyFly
            {
                defaultLabel = defaultLabel,
                defaultDesc = defaultDesc,
                icon = icon,
                pawn = pawn,
                abilityRange = AdjustedRange,
                hotKey = KeyBindingDefOf.Misc3
            };
        }

        // Token: 0x06000060 RID: 96 RVA: 0x00004540 File Offset: 0x00002740
        public void FlyAbility(Pawn pawn, IntVec3 targetCell)
        {
            if (cooldownTicks > 0f)
            {
                Messages.Message("HarpyFly_Cooldown".Translate(), MessageTypeDefOf.RejectInput, false);
                return;
            }

            soundQueued = true;
            RotateFlyer(pawn, targetCell, out var num);
            var speed = HarpyDefOf.FlyAbilitySkyfaller.skyfaller.speed;
            var num2 = (int) (pawn.Position.DistanceTo(targetCell) / speed);
            num += 270f;
            if (num >= 360f)
            {
                num -= 360f;
            }

            var skyfaller = SkyfallerMaker.SpawnSkyfaller(HarpyDefOf.FlyAbilitySkyfaller, targetCell, pawn.Map);
            var flyAbilitySkyfaller = skyfaller as FlyAbilitySkyfaller;
            skyfaller.ticksToImpact = num2;
            if (flyAbilitySkyfaller != null)
            {
                flyAbilitySkyfaller.ticksToImpactMax = num2;
                flyAbilitySkyfaller.skyfallerPawn = new PawnRenderer(pawn);
            }

            skyfaller.angle = num;
            drafted = pawn.Drafted;
            pawn.DeSpawn();
            skyfaller.innerContainer.TryAdd(pawn, false);
        }

        // Token: 0x06000061 RID: 97 RVA: 0x00004618 File Offset: 0x00002818
        internal void RotateFlyer(Pawn pawn, IntVec3 targetCell, out float angle)
        {
            angle = pawn.Position.ToVector3().AngleToFlat(targetCell.ToVector3());
            var num = angle + 90f;
            if (num > 360f)
            {
                num -= 360f;
            }

            var rotation = Rot4.FromAngleFlat(num);
            pawn.Rotation = rotation;
        }

        // Token: 0x06000062 RID: 98 RVA: 0x00004668 File Offset: 0x00002868
        public override void CompTickRare()
        {
            drugsNursedToday.RemoveAll(kvp => kvp.Value < Find.TickManager.TicksGame - 60000);
        }

        // Token: 0x06000063 RID: 99 RVA: 0x00004698 File Offset: 0x00002898
        public override void PrePreTraded(TradeAction action, Pawn playerNegotiator, ITrader trader)
        {
            base.PrePreTraded(action, playerNegotiator, trader);
            if (action != TradeAction.PlayerBuys)
            {
                return;
            }

            if (parent is Pawn pawn)
            {
                HarpyUtility.SwapLightningWeapon(pawn, HarpyUtility.HarpyAmplifierLevel(pawn));
            }
        }
    }
}