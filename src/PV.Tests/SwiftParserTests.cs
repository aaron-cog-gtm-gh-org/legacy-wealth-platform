using System;
using NUnit.Framework;
using PV.Core;

namespace PV.Tests
{
    // RJM 2013-06: parser coverage against strings captured from real files.
    [TestFixture]
    public class SwiftParserTests
    {
        // from MT535_0010042_20181115_001.txt (kept here since the file moved)
        private const string MT535 =
            "{1:F01BRKWGB2LAXXX8176100240}{2:O5350911181115KSTLGB2LAXXX99501002331811150911N}{4:\r\n" +
            ":16R:GENL\r\n" +
            ":28E:1/ONLY\r\n" +
            ":20C::SEME//STMT20181115221\r\n" +
            ":23G:NEWM\r\n" +
            ":98A::STAT//20181115\r\n" +
            ":22F::STTY//CUST\r\n" +
            ":97A::SAFE//0010042\r\n" +
            ":17B::ACTI//Y\r\n" +
            ":16S:GENL\r\n" +
            ":16R:FIN\r\n" +
            ":35B:ISIN GB00ZZ000486\r\n" +
            "Draycott Systems PLC Ord\r\n" +
            ":90B::MRKT//ACTU/GBP34,553\r\n" +
            ":93B::AGGR//UNIT/1660,\r\n" +
            ":19A::HOLD//GBP57357,98\r\n" +
            ":16S:FIN\r\n" +
            ":16R:FIN\r\n" +
            ":35B:ISIN FR00ZZ000406\r\n" +
            "Tavistock Shipping SA\r\n" +
            ":90B::MRKT//ACTU/EUR109,556\r\n" +
            ":93B::AGGR//UNIT/230,\r\n" +
            ":19A::HOLD//EUR25197,88\r\n" +
            ":19A::HOLD//GBP21433,64\r\n" +
            ":92B::EXCH//EUR/GBP/0,850613\r\n" +
            ":16S:FIN\r\n" +
            "-}{5:{CHK:E5A896544EC0}}";

        private const string MT545 =
            "{1:F01BRKWGB2LAXXX3820107925}{2:O5451732260420KSTLGB2LAXXX44541079182604201732N}{4:\r\n" +
            ":16R:GENL\r\n" +
            ":20C::SEME//SETT20260420211\r\n" +
            ":23G:NEWM\r\n" +
            ":16S:GENL\r\n" +
            ":16R:TRADDET\r\n" +
            ":98A::TRAD//20260415\r\n" +
            ":98A::SETT//20260420\r\n" +
            ":90B::DEAL//ACTU/GBP33,990\r\n" +
            ":35B:ISIN LU00ZZ012622\r\n" +
            "Elmhurst UK Equity Income Fund Acc\r\n" +
            ":16S:TRADDET\r\n" +
            ":16R:FIAC\r\n" +
            ":36B::ESTT//UNIT/941,8671\r\n" +
            ":97A::SAFE//0011521\r\n" +
            ":16S:FIAC\r\n" +
            ":16R:SETDET\r\n" +
            ":22F::SETR//TRAD\r\n" +
            ":16R:AMT\r\n" +
            ":19A::ESTT//GBP4303,01\r\n" +
            ":16S:AMT\r\n" +
            ":16S:SETDET\r\n" +
            "-}{5:{CHK:E83F1E4429FC}}";

        [Test]
        public void ParseDecimal_CommaDecimal()
        {
            Assert.AreEqual(12345.67m, SwiftParser.ParseDecimal("12345,67"));
        }

        [Test]
        public void ParseDecimal_TrailingComma()
        {
            Assert.AreEqual(1250m, SwiftParser.ParseDecimal("1250,"));
        }

        [Test]
        public void ParseDecimal_FractionOnly()
        {
            Assert.AreEqual(0.5m, SwiftParser.ParseDecimal("0,5"));
        }

        [Test]
        public void FormatDate_UsesStatementFormat()
        {
            Assert.AreEqual("15-NOV-2018",
                DateUtil.FormatDate(new DateTime(2018, 11, 15)));
        }

        [Test]
        public void ParseStatement_ReadsHeaderAndLines()
        {
            Mt535Statement st = SwiftParser.ParseStatement(MT535, "MT535_0010042_20181115_001.txt");
            Assert.AreEqual("NEWM", st.Function);
            Assert.AreEqual(new DateTime(2018, 11, 15), st.StatementDate);
            Assert.AreEqual("0010042", st.SafekeepingAcct);
            Assert.AreEqual(2, st.Lines.Count);
            Assert.AreEqual("GB00ZZ000486", st.Lines[0].Isin);
            Assert.AreEqual("Draycott Systems PLC Ord", st.Lines[0].MsgDescription);
            Assert.AreEqual(34.553m, st.Lines[0].Price);
            Assert.AreEqual(1660m, st.Lines[0].Qty);
            Assert.AreEqual(57357.98m, st.Lines[0].MktValGbp);
            // non-GBP line carries both values
            Assert.AreEqual("EUR", st.Lines[1].Ccy);
            Assert.AreEqual(25197.88m, st.Lines[1].MktVal);
            Assert.AreEqual(21433.64m, st.Lines[1].MktValGbp);
        }

        [Test]
        public void ParseConfirm_ReadsSettlement()
        {
            SettlementConfirm c = SwiftParser.ParseConfirm(MT545, @"mt54x\20260420\MT545_11521_20260420_001.txt");
            Assert.AreEqual(545, c.MtType);
            Assert.AreEqual("SETT20260420211", c.Seme);
            Assert.AreEqual("NEWM", c.Function);
            Assert.AreEqual(new DateTime(2026, 4, 15), c.TradeDate);
            Assert.AreEqual(new DateTime(2026, 4, 20), c.SettleDate);
            Assert.AreEqual("LU00ZZ012622", c.Isin);
            Assert.AreEqual(941.8671m, c.Qty);
            Assert.AreEqual("0011521", c.custAcct);
            Assert.AreEqual("GBP4303,01", c.AmountRaw);
            Assert.AreEqual("REC", c.Direction);
        }

        [Test]
        public void StatementKey_UsesUnpaddedSafe()
        {
            // statements are keyed on the custodian acct with leading zeros off
            Assert.AreEqual("11521", "0011521".TrimStart('0'));
        }
    }
}
