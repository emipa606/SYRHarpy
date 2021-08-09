using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace SyrHarpy
{
    // Token: 0x02000032 RID: 50
    public class Need_Bloodlust : Need
    {
        // Token: 0x04000046 RID: 70
        private int lastGainTick;

        // Token: 0x0600008C RID: 140 RVA: 0x000054FD File Offset: 0x000036FD
        public Need_Bloodlust(Pawn pawn) : base(pawn)
        {
            threshPercents = new List<float>
            {
                0.2f,
                0.5f
            };
        }

        // Token: 0x17000014 RID: 20
        // (get) Token: 0x0600008D RID: 141 RVA: 0x00005527 File Offset: 0x00003727
        protected override bool IsFrozen => base.IsFrozen || pawn.InBed() || pawn.IsCaravanMember();

        // Token: 0x17000015 RID: 21
        // (get) Token: 0x0600008E RID: 142 RVA: 0x0000554B File Offset: 0x0000374B
        public override int GUIChangeArrow
        {
            get
            {
                if (IsFrozen)
                {
                    return 0;
                }

                if (!GainingBloodlust)
                {
                    return -1;
                }

                return 1;
            }
        }

        // Token: 0x17000016 RID: 22
        // (get) Token: 0x0600008F RID: 143 RVA: 0x00005562 File Offset: 0x00003762
        private bool GainingBloodlust => Find.TickManager.TicksGame < lastGainTick + 150;

        // Token: 0x06000090 RID: 144 RVA: 0x0000557C File Offset: 0x0000377C
        public override void SetInitialLevel()
        {
            CurLevel = Rand.Range(0.8f, 1f);
        }

        // Token: 0x06000091 RID: 145 RVA: 0x00005594 File Offset: 0x00003794
        public override void NeedInterval()
        {
            if (!IsFrozen)
            {
                var curLevel = CurLevel;
                if (pawn.LastAttackedTarget.Pawn != null &&
                    pawn.LastAttackTargetTick + 150 > Find.TickManager.TicksGame)
                {
                    CurLevel += 0.05f;
                    lastGainTick = Find.TickManager.TicksGame;
                }
                else if (CurLevel < 0.2f)
                {
                    CurLevel -= 0.000375f;
                }
                else if (CurLevel < 0.5f)
                {
                    CurLevel -= 0.0005f;
                }
                else
                {
                    CurLevel -= 0.000625f;
                }

                if (curLevel <= 0.2f && CurLevel >= 0.2f || curLevel >= 0.2f && CurLevel <= 0.2f ||
                    curLevel <= 0.5f && CurLevel >= 0.5f || curLevel >= 0.5f && CurLevel <= 0.5f)
                {
                    BloodlustAlertUtility.Notify_BloodlustChanged(pawn, CurLevel);
                }
            }

            if (!pawn.RaceProps.Humanlike || !pawn.Spawned && !pawn.IsCaravanMember() ||
                pawn.MentalStateDef != null || !(CurLevel < 0.2f) ||
                !Rand.MTBEventOccurs(CurLevel * 5f, 60000f, 150f) || pawn.Downed || !pawn.Awake() || pawn.InMentalState)
            {
                return;
            }

            pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk,
                "HarpyNeed_BloodlustBreak".Translate(), false, true);
            GainBloodlust(0.25f);
        }

        // Token: 0x06000092 RID: 146 RVA: 0x00005790 File Offset: 0x00003990
        public void GainBloodlust(float amount)
        {
            if (amount <= 0f)
            {
                return;
            }

            amount = Mathf.Min(amount, 1f - CurLevel);
            curLevelInt += amount;
            BloodlustAlertUtility.Notify_BloodlustChanged(pawn, curLevelInt);
            lastGainTick = Find.TickManager.TicksGame;
        }
    }
}