using System.Collections.Generic;

namespace PV.Core
{
    // RJM 2013-04: a block-4 field. Raw keeps the leading qualifier text as sent
    // (e.g. ":SEME//STMT20260417893" or ":MRKT//PRCT/97,425"); continuation lines
    // are appended to Raw with \n so 35B keeps its ISIN/sedol/name lines.
    public class SwiftField
    {
        public string Tag;   // e.g. 20C, 98A, 35B
        public string Raw;   // everything after the tag colon(s), incl. continuations

        // value after the "//" separator, for qualified fields
        public string Content
        {
            get
            {
                int i = Raw.IndexOf("//");
                return i < 0 ? Raw.Trim() : Raw.Substring(i + 2).Trim();
            }
        }

        // qualifier between "::" and "//" (STAT, TRAD, HOLD, ...) or null
        public string Qualifier
        {
            get
            {
                string s = Raw;
                if (s.StartsWith(":")) s = s.Substring(1);
                int i = s.IndexOf("//");
                return i < 0 ? null : s.Substring(0, i);
            }
        }
    }

    public class SwiftMessage
    {
        public int MtType;
        public string SenderLT;
        public List<SwiftField> Fields = new List<SwiftField>();
        public string FileName;

        public SwiftField Find(string tag)
        {
            return Fields.Find(f => f.Tag == tag);
        }

        public List<SwiftField> FindAll(string tag)
        {
            return Fields.FindAll(f => f.Tag == tag);
        }

        // blocks like 16R:GENL .. 16S:GENL or 16R:FIN .. 16S:FIN
        public List<List<SwiftField>> Blocks(string blockId)
        {
            var blocks = new List<List<SwiftField>>();
            List<SwiftField> cur = null;
            foreach (SwiftField f in Fields)
            {
                if (f.Tag == "16R" && f.Raw.Trim() == blockId)
                {
                    cur = new List<SwiftField>();
                    blocks.Add(cur);
                }
                else if (f.Tag == "16S" && f.Raw.Trim() == blockId)
                {
                    cur = null;
                }
                else if (cur != null)
                {
                    cur.Add(f);
                }
            }
            return blocks;
        }
    }
}
