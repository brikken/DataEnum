using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using attr = BiTemporal.Attributes;
using System.Reflection;

namespace Tests.ConsoleApps.BiTemporal
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void ConfigurationAttributes()
        {
            var config = new Configuration();
            Assert.AreEqual((false, Configuration.missingMessage), attr.Configuration.Validate(config));
            config.PropRequired = "";
            Assert.IsTrue(attr.Configuration.Validate(config).valid);
        }

        class Configuration
        {
            public const string missingMessage = "missing message";
            [attr.Required(MissingMessage = missingMessage)]
            public string PropRequired { get; set; }
            public string PropNotRequired { get; set; }
        }
    }
}
