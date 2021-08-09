using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace SyrHarpy
{
    // Token: 0x0200002B RID: 43
    public static class HarpyUtility
    {
        // Token: 0x06000074 RID: 116 RVA: 0x00004E36 File Offset: 0x00003036
        public static bool FlightCapabable(Pawn pawn)
        {
            return pawn != null && pawn.def == HarpyDefOf.Harpy && FlightCapability(pawn) > 1f;
        }

        // Token: 0x06000075 RID: 117 RVA: 0x00004E58 File Offset: 0x00003058
        public static float FlightCapability(Pawn pawn)
        {
            if (pawn == null || pawn.def != HarpyDefOf.Harpy)
            {
                return 0f;
            }

            var body = pawn.RaceProps.body;
            var num = 0f;
            foreach (var bodyPartRecord in body.GetPartsWithDef(HarpyDefOf.WingHarpy))
            {
                num += PawnCapacityUtility.CalculatePartEfficiency(pawn.health.hediffSet, bodyPartRecord);
            }

            return num;
        }

        // Token: 0x06000076 RID: 118 RVA: 0x00004EE0 File Offset: 0x000030E0
        public static int HarpyAmplifierLevel(Pawn pawn)
        {
            var Hediff_Level =
                (Hediff_Level) pawn.health.hediffSet.GetFirstHediffOfDef(HarpyDefOf.LightningAmplifierHediff);
            if (Hediff_Level != null)
            {
                return Hediff_Level.level;
            }

            return 0;
        }

        // Token: 0x06000077 RID: 119 RVA: 0x00004F14 File Offset: 0x00003114
        public static bool IsHarpyLightningWeapon(ThingDef thingDef)
        {
            return thingDef == HarpyDefOf.Harpy_LightningWeaponZero ||
                   thingDef == HarpyDefOf.Harpy_LightningWeaponOne || thingDef == HarpyDefOf.Harpy_LightningWeaponTwo ||
                   thingDef == HarpyDefOf.Harpy_LightningWeaponThree ||
                   thingDef == HarpyDefOf.Harpy_LightningWeaponFour ||
                   thingDef == HarpyDefOf.Harpy_LightningWeaponFive || thingDef == HarpyDefOf.Harpy_LightningWeaponSix;
        }

        // Token: 0x06000078 RID: 120 RVA: 0x00004F50 File Offset: 0x00003150
        public static void SwapLightningWeapon(Pawn pawn, int level)
        {
            ThingDef thingDef = null;
            switch (level)
            {
                case 0:
                    thingDef = HarpyDefOf.Harpy_LightningWeaponZero;
                    break;
                case 1:
                    thingDef = HarpyDefOf.Harpy_LightningWeaponOne;
                    break;
                case 2:
                    thingDef = HarpyDefOf.Harpy_LightningWeaponTwo;
                    break;
                case 3:
                    thingDef = HarpyDefOf.Harpy_LightningWeaponThree;
                    break;
                case 4:
                    thingDef = HarpyDefOf.Harpy_LightningWeaponFour;
                    break;
                case 5:
                    thingDef = HarpyDefOf.Harpy_LightningWeaponFive;
                    break;
                case 6:
                    thingDef = HarpyDefOf.Harpy_LightningWeaponSix;
                    break;
            }

            if (thingDef == null)
            {
                return;
            }

            var thingWithComps = (ThingWithComps) ThingMaker.MakeThing(thingDef);
            if (pawn.equipment.Primary != null)
            {
                pawn.equipment.DestroyEquipment(pawn.equipment.Primary);
            }

            pawn.equipment.AddEquipment(thingWithComps);
        }

        // Token: 0x06000079 RID: 121 RVA: 0x00004FFC File Offset: 0x000031FC
        public static void ThrowFeathersMote(Vector3 loc, Map map, Pawn pawn)
        {
            if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
            {
                return;
            }

            var moteThrown = (MoteThrown) ThingMaker.MakeThing(HarpyDefOf.HarpyMote_Feathers);
            moteThrown.Scale = Rand.Range(0.3f, 0.6f);
            moteThrown.rotationRate = Rand.Range(-180, 180);
            moteThrown.exactRotation = Rand.Range(-180, 180);
            moteThrown.exactPosition = loc;
            moteThrown.exactPosition -= new Vector3(0.5f, 0f, 0.5f);
            moteThrown.exactPosition += new Vector3(Rand.Value, 0f, Rand.Value);
            moteThrown.SetVelocity(Rand.Range(0, 360), Rand.Range(0.2f, 2f));
            if (pawn != null)
            {
                var color = Color.Lerp(pawn.story.hairColor, new Color(1f, 1f, 1f, 1f), Rand.Value);
                if (Rand.Value < 0.5f)
                {
                    color = Color.Lerp(pawn.story.hairColor, color, Rand.Value);
                }

                moteThrown.instanceColor = color;
            }

            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map);
        }

        // Token: 0x0600007A RID: 122 RVA: 0x0000514F File Offset: 0x0000334F
        [DebugAction("Pawns", "Bloodlust -20%", actionType = DebugActionType.ToolMap,
            allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void OffsetBloodlustNegative20()
        {
            OffsetNeed(DefDatabase<NeedDef>.GetNamed("Bloodlust"), -0.2f);
        }

        // Token: 0x0600007B RID: 123 RVA: 0x00005168 File Offset: 0x00003368
        private static void OffsetNeed(NeedDef nd, float offsetPct)
        {
            foreach (var pawn in (from t in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell())
                where t is Pawn
                select t).Cast<Pawn>())
            {
                var need = pawn.needs.TryGetNeed(nd);
                if (need == null)
                {
                    continue;
                }

                need.CurLevel += offsetPct * need.MaxLevel;
                BloodlustAlertUtility.Notify_BloodlustChanged(pawn, need.CurLevel);
                DebugActionsUtility.DustPuffFrom(pawn);
            }
        }
    }
}