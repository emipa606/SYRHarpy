using HarmonyLib;
using Verse;

namespace SyrHarpy
{
    // Token: 0x02000013 RID: 19
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "TryDropEquipment")]
    public static class TryDropEquipmentPatch
    {
        // Token: 0x06000046 RID: 70 RVA: 0x00003847 File Offset: 0x00001A47
        [HarmonyPrefix]
        public static bool TryDropEquipment_Prefix(ThingWithComps eq)
        {
            return eq == null || !HarpyUtility.IsHarpyLightningWeapon(eq.def);
        }
    }
}