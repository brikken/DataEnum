using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataBiTemporal.Parser;

namespace Tests.DataBiTemporal.Parser
{
    [TestClass]
    public class LexerTests
    {
        [TestMethod]
        public void TokenCountTest()
        {
            var code = File.ReadAllText(@"DataBiTemporal\Parser\simple.sql");
            Assert.AreEqual(56, LexerHelpers.GetTokenCount(code));
        }
    }
}
