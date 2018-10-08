using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.WindowsApp
{
    [TestClass]
    public class ReflectionTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var myList = new List<string>();
            Type[] interfaces = myList.GetType().GetInterfaces();
            Assert.IsTrue(interfaces.Count(i => i.Name == "IEnumerable") > 0);
        }
    }
}
