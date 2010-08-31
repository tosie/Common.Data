using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Common.Data.Test {
    static class Global {

        static string tempdb = null;

        public static void BeforeTest(string testdir) {
            if (!String.IsNullOrEmpty(tempdb))
                return;

            tempdb = Path.Combine(testdir, "Data.sdb");
            if (File.Exists(tempdb))
                File.Delete(tempdb);

            SharedObjects.Instance.SetupRepository("Data Source=" + tempdb, "System.Data.SQLite");
        }

        public static void AfterTest() {
            DummyModel.Delete<DummyModel>(DummyModel.Read());
            DummyModelHasMany.Delete<DummyModelHasMany>(DummyModelHasMany.Read());
        }

    }
}
