using System.Xml;
using Verse;

namespace SyrHarpy
{
    // Token: 0x0200001F RID: 31
    public class TechHediffCount
    {
        // Token: 0x0400003B RID: 59
        public int count;

        // Token: 0x0400003A RID: 58
        public HediffDef hediffDef;

        // Token: 0x06000052 RID: 82 RVA: 0x00003F27 File Offset: 0x00002127
        public TechHediffCount()
        {
        }

        // Token: 0x06000053 RID: 83 RVA: 0x00003F2F File Offset: 0x0000212F
        public TechHediffCount(HediffDef hediffDef, int count)
        {
            this.hediffDef = hediffDef;
            this.count = count;
        }

        // Token: 0x06000054 RID: 84 RVA: 0x00003F48 File Offset: 0x00002148
        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            if (xmlRoot.ChildNodes.Count != 1)
            {
                Log.Error("");
                return;
            }

            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "hediffDef", xmlRoot.Name);
            count = ParseHelper.FromString<int>(xmlRoot.FirstChild.Value);
        }
    }
}