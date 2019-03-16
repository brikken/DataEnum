using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataBiTemporal.Translators;
using System.IO;

namespace Tests.DataBiTemporal.Translator
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void Basic()
        {
            var fiSql = new FileInfo("basic.sql");
            var defs = BiTemporal.GetDefinitions(File.ReadAllText(fiSql.FullName));
            Assert.AreEqual(1, defs.Count);
            var def = defs.GetEnumerator().Current;
            Assert.AreEqual("bitemp", def.Options.);
        }
    }
}
