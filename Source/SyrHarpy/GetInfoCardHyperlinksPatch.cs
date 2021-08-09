using HarmonyLib;
using RimWorld;

namespace SyrHarpy
{
    // Token: 0x02000014 RID: 20
    [HarmonyPatch(typeof(StatWorker_MeleeDPS), "GetInfoCardHyperlinks")]
    public static class GetInfoCardHyperlinksPatch
    {
        // Token: 0x06000047 RID: 71 RVA: 0x0000385C File Offset: 0x00001A5C
        [HarmonyPrefix]
        public static bool GetInfoCardHyperlinks_Prefix(StatRequest statRequest)
        {
            return statRequest.Thing.def != HarpyDefOf.Harpy;
        }
    }
}