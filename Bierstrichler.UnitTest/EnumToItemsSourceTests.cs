using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bierstrichler.Converters;
using Bierstrichler.Data.Enums;
using System.Collections.Generic;

namespace Bierstrichler.UnitTest
{
    [TestClass]
    public class EnumToItemsSourceTests
    {
        [TestMethod]
        public void TestConversion()
        {
            EnumToItemsSource c = new EnumToItemsSource(typeof(Gender));
            List<string> obj = (List<string>)c.ProvideValue(null);
            Assert.AreEqual("Männlich", obj[0]);
        }
    }
}
