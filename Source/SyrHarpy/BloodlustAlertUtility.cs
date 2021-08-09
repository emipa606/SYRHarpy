using System.Collections.Generic;
using System.Text;
using Verse;

namespace SyrHarpy
{
    // Token: 0x02000035 RID: 53
    public static class BloodlustAlertUtility
    {
        // Token: 0x04000047 RID: 71
        public static string cachedBloodlustExplanation = "";

        // Token: 0x04000048 RID: 72
        public static List<Pawn> pawnsAtMinorBloodlust = new List<Pawn>();

        // Token: 0x04000049 RID: 73
        public static List<Pawn> pawnsAtMajorBloodlust = new List<Pawn>();

        // Token: 0x17000017 RID: 23
        // (get) Token: 0x0600009D RID: 157 RVA: 0x00005934 File Offset: 0x00003B34
        public static string BloodlustAlertExplanation
        {
            get
            {
                var stringBuilder = new StringBuilder();
                if (pawnsAtMinorBloodlust.Any())
                {
                    var stringBuilder2 = new StringBuilder();
                    foreach (var pawn in pawnsAtMinorBloodlust)
                    {
                        stringBuilder2.AppendLine("  - " + pawn.NameShortColored.Resolve());
                    }

                    stringBuilder.Append("MinorBloodlustAlertDesc".Translate(stringBuilder2).Resolve());
                }

                if (!pawnsAtMajorBloodlust.Any())
                {
                    return stringBuilder.ToString();
                }

                if (stringBuilder.Length != 0)
                {
                    stringBuilder.AppendLine();
                }

                var stringBuilder3 = new StringBuilder();
                foreach (var pawn2 in pawnsAtMajorBloodlust)
                {
                    stringBuilder3.AppendLine("  - " + pawn2.NameShortColored.Resolve());
                }

                stringBuilder.Append("MajorBloodlustAlertDesc".Translate(stringBuilder3).Resolve());

                return stringBuilder.ToString();
            }
        }

        // Token: 0x0600009C RID: 156 RVA: 0x00005868 File Offset: 0x00003A68
        public static void Notify_BloodlustChanged(Pawn pawn, float needLevel)
        {
            if (needLevel <= 0.2f)
            {
                if (pawnsAtMinorBloodlust.Contains(pawn))
                {
                    pawnsAtMinorBloodlust.Remove(pawn);
                }

                if (!pawnsAtMajorBloodlust.Contains(pawn))
                {
                    pawnsAtMajorBloodlust.Add(pawn);
                }
            }
            else if (needLevel <= 0.5f)
            {
                if (!pawnsAtMinorBloodlust.Contains(pawn))
                {
                    pawnsAtMinorBloodlust.Add(pawn);
                }

                if (pawnsAtMajorBloodlust.Contains(pawn))
                {
                    pawnsAtMajorBloodlust.Remove(pawn);
                }
            }
            else if (needLevel > 0.5f)
            {
                if (pawnsAtMinorBloodlust.Contains(pawn))
                {
                    pawnsAtMinorBloodlust.Remove(pawn);
                }

                if (pawnsAtMajorBloodlust.Contains(pawn))
                {
                    pawnsAtMajorBloodlust.Remove(pawn);
                }
            }

            cachedBloodlustExplanation = BloodlustAlertExplanation;
        }
    }
}