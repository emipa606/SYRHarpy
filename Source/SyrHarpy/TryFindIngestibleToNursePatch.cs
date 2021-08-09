using HarmonyLib;
using RimWorld;
using Verse;

namespace SyrHarpy
{
    // Token: 0x02000019 RID: 25
    [HarmonyPatch(typeof(JoyGiver_SocialRelax), "TryFindIngestibleToNurse")]
    public static class TryFindIngestibleToNursePatch
    {
        // Token: 0x0600004C RID: 76 RVA: 0x00003C68 File Offset: 0x00001E68
        [HarmonyPostfix]
        public static void TryFindIngestibleToNurse_Postfix(ref bool __result, IntVec3 center, Pawn ingester,
            Thing ingestible)
        {
            if (!((ingester != null && ingester.def == HarpyDefOf.Harpy) & __result) || ingestible == null)
            {
                return;
            }

            var harpyComp = ingester.TryGetComp<HarpyComp>();
            if (harpyComp == null)
            {
                return;
            }

            foreach (var keyValuePair in harpyComp.drugsNursedToday)
            {
                var str = "Ingestible: ";
                var key = keyValuePair.Key;
                Log.Message($"{str}{key} | Ticks: {Find.TickManager.TicksGame - keyValuePair.Value}");
            }

            if (harpyComp.drugsNursedToday.ContainsKey(ingestible.def))
            {
                __result = false;
                return;
            }

            harpyComp.drugsNursedToday.Add(ingestible.def, Find.TickManager.TicksGame);
        }
    }
}