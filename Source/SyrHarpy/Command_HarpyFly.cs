using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace SyrHarpy
{
    // Token: 0x02000006 RID: 6
    [StaticConstructorOnStartup]
    public class Command_HarpyFly : Command, ITargetingSource
    {
        // Token: 0x04000033 RID: 51
        private static readonly Texture2D cooldownBarTex =
            SolidColorMaterials.NewSolidColorTexture(new Color32(192, 192, 192, 64));

        // Token: 0x04000034 RID: 52
        [NoTranslate] public static string IconPath = "Things/Special/HarpyFlyIcon";

        // Token: 0x04000032 RID: 50
        public float abilityRange;

        // Token: 0x04000031 RID: 49
        public Pawn pawn;

        // Token: 0x04000030 RID: 48
        public IntVec3 targetCell;

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x0600000D RID: 13 RVA: 0x0000245C File Offset: 0x0000065C
        public TargetingParameters targetParams
        {
            get
            {
                var targetingParameters = new TargetingParameters
                {
                    canTargetLocations = true,
                    canTargetSelf = false,
                    canTargetPawns = false,
                    canTargetFires = false,
                    canTargetBuildings = false,
                    canTargetItems = false,
                    validator = x => DropCellFinder.IsGoodDropSpot(x.Cell, x.Map, true, false, false)
                };
                return targetingParameters;
            }
        }

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000011 RID: 17 RVA: 0x000026CD File Offset: 0x000008CD
        public bool MultiSelect => false;

        public virtual bool HidePawnTooltips => false;

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x06000012 RID: 18 RVA: 0x000026D0 File Offset: 0x000008D0
        public Thing Caster => pawn;

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x06000013 RID: 19 RVA: 0x000026D8 File Offset: 0x000008D8
        public Pawn CasterPawn => pawn;

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x06000014 RID: 20 RVA: 0x000026E0 File Offset: 0x000008E0
        public Verb GetVerb => null;

        // Token: 0x17000007 RID: 7
        // (get) Token: 0x06000015 RID: 21 RVA: 0x000026E3 File Offset: 0x000008E3
        public bool CasterIsPawn => true;

        // Token: 0x17000008 RID: 8
        // (get) Token: 0x06000016 RID: 22 RVA: 0x000026E6 File Offset: 0x000008E6
        public bool IsMeleeAttack => false;

        // Token: 0x17000009 RID: 9
        // (get) Token: 0x06000017 RID: 23 RVA: 0x000026E9 File Offset: 0x000008E9
        public bool Targetable => true;

        // Token: 0x1700000A RID: 10
        // (get) Token: 0x06000018 RID: 24 RVA: 0x000026EC File Offset: 0x000008EC
        public Texture2D UIIcon => icon;

        // Token: 0x1700000B RID: 11
        // (get) Token: 0x06000019 RID: 25 RVA: 0x000026F4 File Offset: 0x000008F4
        public ITargetingSource DestinationSelector => null;

        // Token: 0x0600001A RID: 26 RVA: 0x000026F8 File Offset: 0x000008F8
        public bool CanHitTarget(LocalTargetInfo target)
        {
            return pawn.Map != null && target.Cell.DistanceTo(pawn.Position) <= abilityRange &&
                   target.Cell.Standable(pawn.Map) && target.Cell.InBounds(pawn.Map) && !target.Cell.Roofed(pawn.Map);
        }

        // Token: 0x0600001C RID: 28 RVA: 0x0000284C File Offset: 0x00000A4C
        public void DrawHighlight(LocalTargetInfo target)
        {
            GenDraw.DrawRadiusRing(pawn.Position, abilityRange);
            if (target.IsValid && !target.Cell.Roofed(pawn.Map))
            {
                GenDraw.DrawTargetHighlight(target);
            }
        }

        // Token: 0x0600001D RID: 29 RVA: 0x0000288C File Offset: 0x00000A8C
        public void OnGUI(LocalTargetInfo target)
        {
            if (target.IsValid)
            {
                GenUI.DrawMouseAttachment(UIIcon);
                return;
            }

            GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
        }

        // Token: 0x0600001E RID: 30 RVA: 0x000028AD File Offset: 0x00000AAD
        public void OrderForceTarget(LocalTargetInfo target)
        {
            SetTarget(target);
            QueueCastingJob(target);
        }

        // Token: 0x0600001B RID: 27 RVA: 0x00002780 File Offset: 0x00000980
        public bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            if (!target.Cell.Standable(pawn.Map) || !target.Cell.InBounds(pawn.Map))
            {
                Messages.Message("HarpyFly_InvalidCell".Translate(), MessageTypeDefOf.RejectInput, false);
                return false;
            }

            if (target.Cell.DistanceTo(pawn.Position) > abilityRange)
            {
                Messages.Message("HarpyFly_Distance".Translate(), MessageTypeDefOf.RejectInput, false);
                return false;
            }

            if (!target.Cell.Roofed(pawn.Map))
            {
                return true;
            }

            Messages.Message("HarpyFly_RoofTarget".Translate(), MessageTypeDefOf.RejectInput, false);
            return false;
        }

        // Token: 0x0600000C RID: 12 RVA: 0x0000242D File Offset: 0x0000062D
        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            pawn.TryGetComp<HarpyComp>();
            SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
            Find.Targeter.BeginTargeting(this);
        }

        // Token: 0x0600000E RID: 14 RVA: 0x000024BD File Offset: 0x000006BD
        public override void GizmoUpdateOnMouseover()
        {
            if (Find.CurrentMap != null && abilityRange > 0f)
            {
                GenDraw.DrawRadiusRing(pawn.Position, abilityRange);
            }
        }

        // Token: 0x0600000F RID: 15 RVA: 0x000024EC File Offset: 0x000006EC
        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            disabled = false;
            var harpyComp = pawn.TryGetComp<HarpyComp>();
            if (harpyComp.cooldownTicks > 180f)
            {
                DisableWithReason("HarpyFly_Cooldown".Translate());
            }

            if (pawn == null)
            {
                DisableWithReason("HarpyFly_NoPawn".Translate());
            }

            if (pawn.IsBurning())
            {
                DisableWithReason("HarpyFly_OnFire".Translate());
            }

            if (pawn.Dead)
            {
                DisableWithReason("HarpyFly_Dead".Translate());
            }

            if (pawn.InMentalState)
            {
                DisableWithReason("HarpyFly_MentalState".Translate());
            }

            if (pawn.Downed || pawn.stances.stunner.Stunned || !pawn.Awake())
            {
                DisableWithReason("HarpyFly_Downed".Translate());
            }

            if (!HarpyUtility.FlightCapabable(pawn))
            {
                DisableWithReason("HarpyFly_Wings".Translate());
            }

            var result = base.GizmoOnGUI(topLeft, maxWidth, parms);
            if (!(harpyComp.cooldownTicks > 0f))
            {
                return result;
            }

            if (harpyComp.props is CompProperties_HarpyComp compProperties_HarpyComp)
            {
                var num = Mathf.InverseLerp(0f, compProperties_HarpyComp.cooldown * 60f, harpyComp.cooldownTicks);
                var rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
                Widgets.FillableBar(rect, Mathf.Clamp01(num), cooldownBarTex, null, false);
                Text.Font = GameFont.Tiny;
                Text.Anchor = TextAnchor.UpperCenter;
                Widgets.Label(rect, (harpyComp.cooldownTicks / 60f).ToStringByStyle(ToStringStyle.Integer));
            }

            Text.Anchor = TextAnchor.UpperLeft;

            return result;
        }

        // Token: 0x06000010 RID: 16 RVA: 0x000026BD File Offset: 0x000008BD
        protected void DisableWithReason(string reason)
        {
            disabledReason = reason;
            disabled = true;
        }

        // Token: 0x0600001F RID: 31 RVA: 0x000028C0 File Offset: 0x00000AC0
        public virtual void QueueCastingJob(LocalTargetInfo target)
        {
            var harpyComp = pawn.TryGetComp<HarpyComp>();
            if (harpyComp is {cooldownTicks: > 0f})
            {
                return;
            }

            var job = JobMaker.MakeJob(HarpyDefOf.HarpyJob_FlyAbility);
            job.targetA = target;
            pawn.jobs.TryTakeOrderedJob(job, 0);
        }

        // Token: 0x06000020 RID: 32 RVA: 0x0000290F File Offset: 0x00000B0F
        public void SetTarget(LocalTargetInfo target)
        {
            targetCell = target.Cell;
        }

        // Token: 0x06000021 RID: 33 RVA: 0x0000291E File Offset: 0x00000B1E
        public virtual void SelectDestination()
        {
            Find.Targeter.BeginTargeting(this);
        }
    }
}