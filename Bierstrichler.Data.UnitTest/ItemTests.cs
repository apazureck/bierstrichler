using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Bierstrichler.Data.Items;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Runtime.Serialization;

namespace Bierstrichler.Data.UnitTest
{
    [TestClass]
    public class ItemTests
    {
        [TestMethod]
        public void SaveAndLoadTest()
        {
            Item itm = new Item();
            itm.Name = "TestItem";
            itm.Description = "TestDescription";
            itm.Available = true;
            itm.Category = "Testcategory";
            itm.Add(new Persons.Korpos.Bundesbruder(), 10);
            itm.Add(new Persons.Korpos.Bundesbruder());
            itm.Remove(new Persons.Korpos.Bundesbruder(), 5);
            itm.Remove(new Persons.Korpos.Bundesbruder());
            itm.Coleurfaehig = true;
            itm.RelativeImagePath = "test.img";
            itm.PriceBuying = 1;
            itm.PriceSelling = 2;
            using(FileStream fs = new FileStream("test.bin", FileMode.OpenOrCreate))
                itm.AddToFile(fs);

            Item itm2 = LoadFromFile("test.bin");
            CompareMembers<Item>(itm, itm2);
        }

        private void CompareMembers<T>(T itm, T itm2)
        {
            Type t = itm.GetType();
            PropertyInfo[] miArr = t.GetProperties();
            foreach (PropertyInfo pi in miArr)
                Assert.AreEqual(pi.GetValue(itm), pi.GetValue(itm2), "Property: " + pi.Name);
        }

        private Item LoadFromFile(string url)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(url, FileMode.Open);
                BinaryFormatter binForm = new BinaryFormatter();
                return (Item)binForm.Deserialize(fs);
            }
            finally
            {
                if (fs != null) fs.Close();
            }
        }

        [TestMethod]
        public void TestGetObjectData()
        {
            StreamingContext context = new StreamingContext();
            SerializationInfo info = new SerializationInfo(typeof(Item), new FormatterConverter());
            Item itm = new Item();
            itm.GetObjectData(info, context);
        }
    }
}
