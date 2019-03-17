using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataBiTemporal.Parser;

namespace Tests.DataBiTemporal.Parser
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void RootContextTest()
        {
            var code = File.ReadAllText(@"DataBiTemporal\Parser\simple.sql");
            Assert.IsNotNull(ParserHelpers.GetRootContext(code) as BiTempDefParser.CompileUnitContext);
        }

        [TestMethod]
        public void WalkTest()
        {
            var code = File.ReadAllText(@"DataBiTemporal\Parser\simple.sql");
            Assert.AreEqual(33, ParserHelpers.WalkTree(code));
        }
    }
}
