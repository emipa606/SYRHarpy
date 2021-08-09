using HarmonyLib;
using RimWorld;
using Verse;

namespace SyrHarpy
{
    // Token: 0x02000016 RID: 22
    [HarmonyPatch(typeof(InteractionWorker_RecruitAttempt), "DoRecruit", new[]
    {
        typeof(Pawn),
        typeof(Pawn),
        typeof(string),
        typeof(string),
        typeof(bool),
        typeof(bool)
    }, new[]
    {
        ArgumentType.Normal,
        ArgumentType.Normal,
        ArgumentType.Out,
        ArgumentType.Out,
        ArgumentType.Normal,
        ArgumentType.Normal
    })]
    public static class DoRecruitPatch
    {
        // Token: 0x06000049 RID: 73 RVA: 0x000039F8 File Offset: 0x00001BF8
        [HarmonyPostfix]
        public static void DoRecruit_Postfix(Pawn recruitee)
        {
            if (recruitee == null || recruitee.def != HarpyDefOf.Harpy)
            {
                return;
            }

            var equipment = recruitee.equipment;
            bool hasEquipment;
            if (equipment == null)
            {
                hasEquipment = false;
            }
            else
            {
                var primary = equipment.Primary;
                hasEquipment = primary?.def != null;
            }

            if (hasEquipment && HarpyUtility.IsHarpyLightningWeapon(recruitee.equipment.Primary.def))
            {
                return;
            }

            var thingWithComps = (ThingWithComps) ThingMaker.MakeThing(HarpyDefOf.Harpy_LightningWeaponZero);
            recruitee.equipment.DestroyEquipment(recruitee.equipment.Primary);
            recruitee.equipment.AddEquipment(thingWithComps);
        }
    }
}