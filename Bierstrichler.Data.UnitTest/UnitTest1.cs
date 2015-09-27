using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bierstrichler.Data.UnitTest
{
    [TestClass]
    public class AddressTests
    {
        [TestMethod]
        public void TestFromString()
        {
            string totest = "Am Messehaus 8, 90489 Nürnberg, Deutschland";
            Address adresse = new Address();
            adresse.FromString(totest);
            Assert.AreEqual("Am Messehaus 8", adresse.Street);
            Assert.AreEqual("90489", adresse.ZipCode);
            Assert.AreEqual("Nürnberg", adresse.City);
            Assert.AreEqual("Deutschland", adresse.Country);
        }
    }
}
