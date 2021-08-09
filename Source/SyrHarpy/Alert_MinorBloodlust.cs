using RimWorld;
using Verse;

namespace SyrHarpy
{
    // Token: 0x02000033 RID: 51
    public class Alert_MinorBloodlust : Alert
    {
        // Token: 0x06000093 RID: 147 RVA: 0x000057E9 File Offset: 0x000039E9
        public Alert_MinorBloodlust()
        {
            defaultPriority = AlertPriority.High;
        }

        // Token: 0x06000094 RID: 148 RVA: 0x000057F8 File Offset: 0x000039F8
        public override string GetLabel()
        {
            return "MinorBloodlustAlert".Translate();
        }

        // Token: 0x06000095 RID: 149 RVA: 0x00005809 File Offset: 0x00003A09
        public override TaggedString GetExplanation()
        {
            return BloodlustAlertUtility.cachedBloodlustExplanation;
        }

        // Token: 0x06000096 RID: 150 RVA: 0x00005815 File Offset: 0x00003A15
        public override AlertReport GetReport()
        {
            if (!BloodlustAlertUtility.pawnsAtMajorBloodlust.Any())
            {
                return AlertReport.CulpritsAre(BloodlustAlertUtility.pawnsAtMinorBloodlust);
            }

            return false;
        }
    }
}