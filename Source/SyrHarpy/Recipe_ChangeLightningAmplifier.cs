using System.Collections.Generic;
using RimWorld;
using Verse;

namespace SyrHarpy
{
    // Token: 0x0200002A RID: 42
    public class Recipe_ChangeLightningAmplifier : Recipe_ChangeImplantLevel
    {
        // Token: 0x06000072 RID: 114 RVA: 0x00004DBC File Offset: 0x00002FBC
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients,
            Bill bill)
        {
            var Hediff_Level =
                (Hediff_Level) pawn.health.hediffSet.GetFirstHediffOfDef(HarpyDefOf.LightningAmplifierHediff);
            if (Hediff_Level != null)
            {
                GenPlace.TryPlaceThing(
                    ThingMaker.MakeThing(Hediff_Level.level > 3
                        ? HarpyDefOf.LightningAmplifierAdvanced
                        : HarpyDefOf.LightningAmplifierBasic), billDoer.Position, billDoer.Map, ThingPlaceMode.Near);
            }

            base.ApplyOnPawn(pawn, part, billDoer, ingredients, bill);
        }
    }
}