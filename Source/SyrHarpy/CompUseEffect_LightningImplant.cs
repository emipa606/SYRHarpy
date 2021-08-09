using RimWorld;
using Verse;

namespace SyrHarpy
{
    // Token: 0x02000029 RID: 41
    public class CompUseEffect_LightningImplant : CompUseEffect_InstallImplant
    {
        // Token: 0x0600006F RID: 111 RVA: 0x00004BEC File Offset: 0x00002DEC
        public override void DoEffect(Pawn user)
        {
            base.DoEffect(user);
            var level = ((Hediff_Level) GetExistingImplant(user)).level;
            HarpyUtility.SwapLightningWeapon(user, level);
        }

        // Token: 0x06000070 RID: 112 RVA: 0x00004C1C File Offset: 0x00002E1C
        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            if ((!p.IsFreeColonist || p.HasExtraHomeFaction((Faction) null)) && !Props.allowNonColonists)
            {
                failReason = "InstallImplantNotAllowedForNonColonists".Translate();
                return false;
            }

            if (p.RaceProps.body.GetPartsWithDef(Props.bodyPart).FirstOrFallback() == null)
            {
                failReason = "InstallImplantNoBodyPart".Translate() + ": " + Props.bodyPart.LabelShort;
                return false;
            }

            if (p.def != HarpyDefOf.Harpy)
            {
                failReason = "HarpyLightningAmplifierNotHarpy".Translate();
                return false;
            }

            var existingImplant = GetExistingImplant(p);
            if (existingImplant != null)
            {
                if (!Props.canUpgrade)
                {
                    failReason = "InstallImplantAlreadyInstalled".Translate();
                    return false;
                }

                var Hediff_Level = (Hediff_Level) existingImplant;
                if (Hediff_Level.level >= Hediff_Level.def.maxSeverity)
                {
                    failReason = "InstallImplantAlreadyMaxLevel".Translate();
                    return false;
                }

                if (Hediff_Level.level > 2 && parent.def == HarpyDefOf.LightningAmplifierBasic)
                {
                    failReason = "HarpyLightningAmplifierBetter".Translate();
                    return false;
                }

                if (Hediff_Level.level < 3 && parent.def == HarpyDefOf.LightningAmplifierAdvanced)
                {
                    failReason = "HarpyLightningAmplifierWorse".Translate();
                    return false;
                }
            }

            if (existingImplant == null && parent.def == HarpyDefOf.LightningAmplifierAdvanced)
            {
                failReason = "HarpyLightningAmplifierWorse".Translate();
                return false;
            }

            failReason = null;
            return true;
        }
    }
}