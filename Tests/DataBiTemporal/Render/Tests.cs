using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using def = DataBiTemporal.Definitions;
using render = DataBiTemporal.Renderers;

namespace Tests.DataBiTemporal.Render
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void SimpleRender()
        {
            Assert.AreEqual("test", render.BiTemporal.Render(null, "test"));
        }
    }
}
