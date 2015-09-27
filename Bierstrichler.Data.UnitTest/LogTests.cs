using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bierstrichler.Data.Global;
using System.Collections.Generic;

namespace Bierstrichler.Data.UnitTest
{
    [TestClass]
    public class LogTests
    {
        [TestMethod]
        public void TestFileStream()
        {
            Log.OpenLog();
            Log.WriteDebug("Debug");
            Log.WriteError("Error");
            Log.WriteInformation("Information");
            Log.WriteWarning("Warning");
            Log.CloseLog();
            string [] logfiles = Log.ShowLogFiles();
            //List<LogEntry> list = Log.ReadEntries(logfiles[0]);
        }
    }
}
