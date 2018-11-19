using System.IO;
using NUnit.Framework;
using PV.Core;

namespace PV.Tests
{
    // These two read a captured statement file that used to sit in the dev
    // archive. Left here - someone may still have the file locally.
    [TestFixture]
    public class ArchiveFileTests
    {
        private const string PathToFile =
            @"..\..\..\..\archive\mt535\MT535_0010042_20181115_001.txt";

        [Test]
        public void ParseStatement_FromArchiveFile()
        {
            Assert.IsTrue(File.Exists(PathToFile));
            Mt535Statement st = SwiftParser.ParseStatement(
                File.ReadAllText(PathToFile), PathToFile);
            Assert.AreEqual("0010042", st.SafekeepingAcct);
        }

        [Test]
        public void StatementCount_MatchesSnapshot()
        {
            Mt535Statement st = SwiftParser.ParseStatement(
                File.ReadAllText(PathToFile), PathToFile);
            Assert.AreEqual(9, st.Lines.Count);
        }
    }
}
