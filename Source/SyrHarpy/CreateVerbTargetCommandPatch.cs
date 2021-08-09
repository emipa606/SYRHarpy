using HarmonyLib;
using RimWorld;
using Verse;

namespace SyrHarpy
{
    // Token: 0x02000015 RID: 21
    [HarmonyPatch(typeof(VerbTracker), "CreateVerbTargetCommand")]
    public static class CreateVerbTargetCommandPatch
    {
        // Token: 0x06000048 RID: 72 RVA: 0x00003874 File Offset: 0x00001A74
        [HarmonyPostfix]
        public static void CreateVerbTargetCommand_Postfix(Command_VerbTarget __result, VerbTracker __instance,
            Thing ownerThing, Verb verb)
        {
            if (ownerThing == null || !HarpyUtility.IsHarpyLightningWeapon(ownerThing.def) || verb.caster == null)
            {
                return;
            }

            if (verb.caster is Pawn pawn)
            {
                __result.defaultDesc = ownerThing.def.description.CapitalizeFirst() + "\n\n" +
                                       "HarpyLightning_Damage".Translate() + ": " +
                                       (14 + (2 * HarpyUtility.HarpyAmplifierLevel(pawn))).ToString() + " (" +
                                       verb.verbProps.defaultProjectile.projectile.damageDef.armorCategory
                                           .ToString() + ") " + "\n" + "HarpyLightning_Range".Translate() + ": " +
                                       (verb.verbProps.range + 0.1f).ToString() + "\n" +
                                       "HarpyLightning_Casttime".Translate() + ": " +
                                       verb.verbProps.warmupTime.ToString() + "\n" +
                                       "HarpyLightning_Cooldown".Translate() + ": " + ownerThing
                                           .GetStatValue(StatDefOf.RangedWeapon_Cooldown).ToString();
            }
        }
    }
}