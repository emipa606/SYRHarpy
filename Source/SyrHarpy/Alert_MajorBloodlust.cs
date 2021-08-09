using RimWorld;
using Verse;

namespace SyrHarpy
{
    // Token: 0x02000034 RID: 52
    public class Alert_MajorBloodlust : Alert_Critical
    {
        // Token: 0x06000097 RID: 151 RVA: 0x00005834 File Offset: 0x00003A34
        public override string GetLabel()
        {
            return "MajorBloodlustAlert".Translate();
        }

        // Token: 0x06000098 RID: 152 RVA: 0x00005845 File Offset: 0x00003A45
        public override void AlertActiveUpdate()
        {
        }

        // Token: 0x06000099 RID: 153 RVA: 0x00005847 File Offset: 0x00003A47
        public override TaggedString GetExplanation()
        {
            return BloodlustAlertUtility.cachedBloodlustExplanation;
        }

        // Token: 0x0600009A RID: 154 RVA: 0x00005853 File Offset: 0x00003A53
        public override AlertReport GetReport()
        {
            return AlertReport.CulpritsAre(BloodlustAlertUtility.pawnsAtMajorBloodlust);
        }
    }
}