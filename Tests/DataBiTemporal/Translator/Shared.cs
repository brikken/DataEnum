using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataBiTemporal.Definitions;
using System.Collections.Generic;

namespace Tests.DataBiTemporal.Translator
{
    [TestClass]
    public class Shared
    {
        [TestMethod]
        public void TestImplicit()
        {
            var list = new List<string>() { "1", "2", "3" };
            Defined<List<string>> def = list;
            def.Raw = "1,2,3";
            Assert.AreEqual(3, def.Content.Count);
            List<string> list2 = def;
            Assert.AreEqual(3, list2.Count);
        }
    }
}
