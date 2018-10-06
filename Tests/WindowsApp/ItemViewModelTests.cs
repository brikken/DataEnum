using System;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WindowsApp.Generic;

namespace Tests.WindowsApp
{
    [TestClass]
    public class ItemViewModelTests
    {
        /// <summary>
        /// Tests that a basic instance of IItemViewModel has a Properties collection of the fundamental property IsSelected
        /// </summary>
        [TestMethod]
        public void PropertiesCollectionTest()
        {
            var vm = new ItemViewModelTest();
            var props = vm.Properties;
            Assert.AreEqual(1, props.Count);
        }

        /// <summary>
        /// Must subclass, since ItemViewModel is abstract
        /// </summary>
        class ItemViewModelTest : ItemViewModel
        {
        }
    }
}
