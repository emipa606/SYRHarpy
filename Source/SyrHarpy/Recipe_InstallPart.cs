using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace SyrHarpy
{
    // Token: 0x02000036 RID: 54
    public class Recipe_InstallPart : Recipe_Surgery
    {
        // Token: 0x0600009F RID: 159 RVA: 0x00005AA0 File Offset: 0x00003CA0
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            return MedicalRecipesUtility.GetFixedPartsToApplyOn(recipe, pawn, delegate(BodyPartRecord record)
            {
                var source = from x in pawn.health.hediffSet.hediffs
                    where x.Part == record
                    select x;
                return (source.Count() != 1 || source.First().def != recipe.addsHediff) &&
                       (pawn.health.hediffSet.GetNotMissingParts().Contains(record) ||
                        pawn.health.hediffSet.HasDirectlyAddedPartFor(record) ||
                        pawn.health.hediffSet.PartIsMissing(record) &&
                        pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(record.parent));
            });
        }

        // Token: 0x060000A0 RID: 160 RVA: 0x00005AE0 File Offset: 0x00003CE0
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients,
            Bill bill)
        {
            if (billDoer != null)
            {
                if (CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
                {
                    return;
                }

                TaleRecorder.RecordTale(TaleDefOf.DidSurgery, billDoer, pawn);
                MedicalRecipesUtility.RestorePartAndSpawnAllPreviousParts(pawn, part, billDoer.Position, billDoer.Map);
                if (MedicalRecipesUtility.IsClean(pawn, part) && !PawnGenerator.IsBeingGenerated(pawn) &&
                    IsViolationOnPawn(pawn, part, Faction.OfPlayer) && part.def.spawnThingOnRemoved != null)
                {
                    ThoughtUtility.GiveThoughtsForPawnOrganHarvested(pawn, billDoer);
                }

                if (!PawnGenerator.IsBeingGenerated(pawn) && IsViolationOnPawn(pawn, part, Faction.OfPlayer))
                {
                    ReportViolation(pawn, billDoer, pawn.HomeFaction, -70);
                    // TranslatorFormattedStringExtensions.Translate("GoodwillChangedReason_NeedlesslyInstalledWorseBodyPart", this.recipe.addsHediff.label)
                }
            }
            else if (pawn.Map != null)
            {
                MedicalRecipesUtility.RestorePartAndSpawnAllPreviousParts(pawn, part, pawn.Position, pawn.Map);
            }
            else
            {
                pawn.health.RestorePart(part);
            }

            pawn.health.AddHediff(recipe.addsHediff, part);
        }

        // Token: 0x060000A1 RID: 161 RVA: 0x00005BF0 File Offset: 0x00003DF0
        public override bool IsViolationOnPawn(Pawn pawn, BodyPartRecord part, Faction billDoerFaction)
        {
            return (pawn.Faction != billDoerFaction && pawn.Faction != null || pawn.IsQuestLodger()) &&
                   (recipe.addsHediff.addedPartProps == null || !recipe.addsHediff.addedPartProps.betterThanNatural) &&
                   HealthUtility.PartRemovalIntent(pawn, part) == 0;
        }
    }
}