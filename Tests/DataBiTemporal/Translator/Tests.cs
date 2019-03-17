using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using trans = DataBiTemporal.Translators;
using System.IO;
using System.Collections.Generic;

namespace Tests.DataBiTemporal.Translator
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void Basic()
        {
            var fiSql = new FileInfo(@"DataBiTemporal\Translator\basic.sql");
            var defs = trans.BiTemporal.GetDefinitions(File.ReadAllText(fiSql.FullName));
            Assert.AreEqual(1, defs.Count);
            var def = defs[0];
            Assert.AreEqual("bitemp", def.BtSchema.Name);
            Assert.AreEqual("MyBiTempTable", def.Table.Name);
            Assert.AreEqual(3, def.Columns.Count);
            Assert.AreEqual("id", def.PrimaryKey[0].Id.Name);
            Assert.AreEqual("PRIMARY KEY", def.Columns[0].Options.Raw);
        }
    }
}
