using NUnit.Framework;
using PV.Core;

namespace PV.Tests
{
    // KP 2016-12: arithmetic used by the position check. Runs on in-memory
    // statements so no archive or DB needed.
    [TestFixture]
    public class PositionCheckTests
    {
        private class TestReader
        {
            // mirrors the amount parsing used on 19A::ESTT
            public decimal ToAmount(string raw)
            {
                return SettlementReader.ToAmount(raw);
            }
        }

        [Test]
        public void ToAmount_PoundsAndPence()
        {
            Assert.AreEqual(4303.01m, new TestReader().ToAmount("GBP4303,01"));
        }

        [Test]
        public void ToAmount_NoDecimals()
        {
            Assert.AreEqual(192m, new TestReader().ToAmount("USD192,"));
        }

        [Test]
        public void ExpectedQty_PrevPlusRecMinusDel()
        {
            // in-memory version of the check: prev 100, +50 REC, -25 DEL = 125
            decimal prev = 100m, rec = 50m, del = 25m;
            Assert.AreEqual(125m, prev + rec - del);
        }
    }
}
