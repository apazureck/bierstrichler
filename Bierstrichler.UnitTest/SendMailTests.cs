using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bierstrichler.Functional;
using Bierstrichler.Data.Persons;
using System.Windows;

namespace Bierstrichler.UnitTest
{
    [TestClass]
    public class SendMailTests
    {
        [TestMethod]
        public void SendReminderMailTest()
        {
            Gast g = new Gast()
            {
                Nachname = "Pazureck",
                Vorname = "Andreas",
                Email = "a.pazureck@bearpatrol.de"
            };
            MailScheduler.SendReminderToUser(g);
        }

        [TestMethod]
        public void TestSecureStringSaving()
        {
            System.Security.SecureString sT = new System.Security.SecureString();  
            string secureStringTest = "This is a SecureString test";  
 
            for (int t = 0; t < secureStringTest.Length; t++)  
            {  
                sT.AppendChar(secureStringTest[t]);  
            }  
 
            // create a second.   
            System.Security.SecureString sT2 = new System.Security.SecureString();  
            string secureStringTest2 = "This is a SecureString test";  
 
            for (int t = 0; t < secureStringTest2.Length; t++)  
            {  
                sT2.AppendChar(secureStringTest2[t]);  
            }  
 
            // check sT and sT2 for equality.   
            MessageBox.Show((sT2 == sT).ToString());  
 
 
            Properties.Settings.Default.secureTest = sT;
            Properties.Settings.Default.Save();  
 
            // check sT and Properties.Settings.Default.secureTest for equality.   
            MessageBox.Show((sT == Properties.Settings.Default.secureTest).ToString());  
 
            Properties.Settings.Default.Reload();  
 
            // check sT and Properties.Settings.Default.secureTest for equality.   
            // after reloading.   
            MessageBox.Show((sT == Properties.Settings.Default.secureTest).ToString());  
 
            // check the length of the two objects after reloading.   
            MessageBox.Show((sT.Length == Properties.Settings.Default.secureTest.Length).ToString());
        }
    }
}
