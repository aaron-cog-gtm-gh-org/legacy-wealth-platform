using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace PV.Core
{
    // RJM 2013-04: MT535/MT54x parsing. Written against the Kestrel feed; their
    // messages are pretty regular so this is string-splitting rather than a
    // full FIN grammar. KP 2017-08 added the FAMT/qty-type capture.
    public static class SwiftParser
    {
        // Custodian sends continental decimals ("1.250,50" or "230,"). Also
        // tolerates "1250," which we see at month end.
        public static decimal ParseDecimal(string s)
        {
            s = s.Replace(',', '.');
            return decimal.Parse(s, NumberStyles.Number, CultureInfo.InvariantCulture);
        }

        public static SwiftMessage Parse(string text, string path)
        {
            var msg = new SwiftMessage();
            msg.FileName = path != null ? Path.GetFileName(path) : null;

            int b2s = text.IndexOf("{2:");
            if (b2s >= 0)
            {
                int b2e = text.IndexOf('}', b2s);
                string b2 = text.Substring(b2s + 3, b2e - b2s - 3);
                // O<mt><hhmm><yymmdd><LT12><sess4><isn6><yymmdd><hhmm>N
                if (b2.Length > 0 && b2[0] == 'O')
                {
                    msg.MtType = int.Parse(b2.Substring(1, 3));
                    msg.SenderLT = b2.Substring(11, 12);
                }
            }

            int b4s = text.IndexOf("{4:");
            if (b4s < 0) return msg;
            int b4e = text.IndexOf("\n-}", b4s);
            if (b4e < 0) b4e = text.IndexOf("-}", b4s);
            string body = text.Substring(b4s + 3, b4e - b4s - 3).Trim('\r', '\n');

            SwiftField cur = null;
            foreach (string line in body.Split('\n'))
            {
                string l = line.TrimEnd('\r');
                if (l.StartsWith(":"))
                {
                    // :TAG:value  or  :TAG::QUAL//content
                    int p = l.IndexOf(':', 1);
                    cur = new SwiftField();
                    cur.Tag = l.Substring(1, p - 1);
                    cur.Raw = l.Substring(p + 1);
                    msg.Fields.Add(cur);
                }
                else if (cur != null && l.Length > 0)
                {
                    cur.Raw += "\n" + l;
                }
            }
            return msg;
        }

        public static SwiftMessage ParseFile(string path)
        {
            return Parse(File.ReadAllText(path), path);
        }

        public static Mt535Statement ParseStatement(string text, string path)
        {
            SwiftMessage msg = Parse(text, path);
            var st = new Mt535Statement();
            st.FileName = msg.FileName;
            st.FileSeq = FileSeq(path);

            SwiftField f23 = msg.Find("23G");
            st.Function = f23 != null ? f23.Raw.Trim() : null;

            SwiftField f98 = msg.Find("98A");
            if (f98 != null) st.StatementDate = ParseYmd(f98.Content);

            SwiftField f97 = msg.Find("97A");
            st.SafekeepingAcct = f97 != null ? f97.Content : null;

            // TODO RJM 2013-09: SUBSAFE blocks are flattened, fine while
            // Kestrel only sends one sub-account per statement
            foreach (List<SwiftField> fin in msg.Blocks("FIN"))
            {
                var line = new Mt535Line();

                foreach (SwiftField f in fin)
                {
                    if (f.Tag == "35B")
                    {
                        // first line "ISIN xxxx", optional "/GB/sedol", then name
                        string[] parts = f.Raw.Split('\n');
                        if (parts.Length > 0 && parts[0].StartsWith("ISIN "))
                            line.Isin = parts[0].Substring(5).Trim();
                        line.MsgDescription = parts[parts.Length - 1].Trim();
                        //if (parts.Length > 2) line.Sedol = parts[1].Substring(4);
                    }
                    else if (f.Tag == "90A" || f.Tag == "90B")
                    {
                        // MRKT//PRCT/97,425 or MRKT//ACTU/GBP4,5686 - price is last bit
                        string v = f.Content;
                        int slash = v.LastIndexOf('/');
                        if (slash >= 0) v = v.Substring(slash + 1);
                        if (v.Length > 3 && char.IsLetter(v[0])) v = v.Substring(3);
                        line.Price = ParseDecimal(v);
                    }
                    else if (f.Tag == "93B" && f.Qualifier == "AGGR")
                    {
                        // AGGR//UNIT/x or AGGR//FAMT/x - we treat both as a number
                        string v = f.Content;
                        int slash = v.IndexOf('/');
                        line.QtyType = slash >= 0 ? v.Substring(0, slash) : v;
                        string num = slash >= 0 ? v.Substring(slash + 1) : v;
                        line.Qty = Math.Round(ParseDecimal(num), 4);
                    }
                    else if (f.Tag == "19A" && f.Qualifier == "HOLD")
                    {
                        string v = f.Content;   // e.g. EUR25197,88
                        string ccy = v.Substring(0, 3);
                        decimal mv = Math.Round(ParseDecimal(v.Substring(3)), 2);
                        if (line.Ccy == null)
                        {
                            line.Ccy = ccy;
                            line.MktVal = mv;
                            if (ccy == "GBP") line.MktValGbp = mv;
                        }
                        else if (ccy == "GBP")
                        {
                            line.MktValGbp = mv;
                        }
                    }
                }

                if (line.Isin != null)
                    st.Lines.Add(line);
            }
            return st;
        }

        public static SettlementConfirm ParseConfirm(string text, string path)
        {
            SwiftMessage msg = Parse(text, path);
            var c = new SettlementConfirm();
            c.MtType = msg.MtType;
            c.FilePath = path;
            c.Folder = path != null
                ? Path.GetFileName(Path.GetDirectoryName(path)) : null;

            SwiftField f20 = msg.Find("20C");
            c.Seme = f20 != null ? f20.Content : null;

            SwiftField f23 = msg.Find("23G");
            c.Function = f23 != null ? f23.Raw.Trim() : null;

            foreach (SwiftField f in msg.FindAll("20C"))
            {
                if (f.Qualifier == "PREV") c.Prev = f.Content;
            }

            foreach (SwiftField f in msg.FindAll("98A"))
            {
                if (f.Qualifier == "TRAD") c.TradeDate = ParseYmd(f.Content);
                else if (f.Qualifier == "SETT") c.SettleDate = ParseYmd(f.Content);
            }

            SwiftField f35 = msg.Find("35B");
            if (f35 != null)
            {
                string[] parts = f35.Raw.Split('\n');
                if (parts[0].StartsWith("ISIN ")) c.Isin = parts[0].Substring(5).Trim();
            }

            SwiftField f36 = msg.Find("36B");
            if (f36 != null)
            {
                string v = f36.Content;         // ESTT//UNIT/2924,90
                int slash = v.IndexOf('/');
                c.Qty = ParseDecimal(slash >= 0 ? v.Substring(slash + 1) : v);
            }

            SwiftField f97 = msg.Find("97A");
            c.custAcct = f97 != null ? f97.Content : null;

            SwiftField f19 = msg.Find("19A");
            if (f19 != null && f19.Qualifier == "ESTT")
            {
                c.AmountRaw = f19.Content;      // GBP4303,01
                if (c.AmountRaw.Length > 3) c.Ccy = c.AmountRaw.Substring(0, 3);
            }

            c.Direction = (msg.MtType == 544 || msg.MtType == 545) ? "REC" : "DEL";
            return c;
        }

        // yyyyMMdd as sent in 98A fields
        private static DateTime ParseYmd(string s)
        {
            return DateTime.ParseExact(s.Trim(), "yyyyMMdd", CultureInfo.InvariantCulture);
        }

        // trailing _NNN in the archive file name, 0 when absent
        private static int FileSeq(string path)
        {
            if (path == null) return 0;
            string n = Path.GetFileNameWithoutExtension(path);
            int us = n.LastIndexOf('_');
            if (us < 0) return 0;
            int seq;
            return int.TryParse(n.Substring(us + 1), out seq) ? seq : 0;
        }
    }
}
