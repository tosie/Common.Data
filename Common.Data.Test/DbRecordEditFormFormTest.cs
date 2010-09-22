using Common.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;

namespace Common.Data.Test
{
    
    
    /// <summary>
    ///This is a test class for DbRecordEditFormFormTest and is intended
    ///to contain all DbRecordEditFormFormTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DbRecordEditFormFormTest {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }

        #region Additional test attributes

        static string tempdb;

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext) {
        }

        [ClassCleanup()]
        public static void MyClassCleanup() {
        }

        [TestInitialize()]
        public void MyTestInitialize() {
            tempdb = Path.Combine(testContextInstance.TestDir, "Data.sdb");
            if (File.Exists(tempdb))
                File.Delete(tempdb);

            SharedObjects.Instance.SetupRepository("Data Source=" + tempdb, "System.Data.SQLite");

            InitializeDummyData();
        }

        [TestCleanup()]
        public void MyTestCleanup() {
            if (File.Exists(tempdb))
                File.Delete(tempdb);
        }

        static List<DummyModel> DummyData;
        static List<DummyModelHasMany> DummySetData;

        static void InitializeDummyData() {
            DummyData = new List<DummyModel>();

            DummyData.Add(DummyModel.Create(new object[] { "key1", 21 }));
            DummyData.Add(DummyModel.Create(new object[] { "key2", 22 }));
            DummyData.Add(DummyModel.Create(new object[] { "key3", 23 }));
            DummyData.Add(DummyModel.Create(new object[] { "key4", 24 }));
            DummyData.Add(DummyModel.Create(new object[] { "key5", 25 }));
            DummyData.Add(DummyModel.Create(new object[] { "key6", 26 }));
            DummyData.Add(DummyModel.Create(new object[] { "key7", 27 }));
            DummyData.Add(DummyModel.Create(new object[] { "key8", 28 }));
            DummyData.Add(DummyModel.Create(new object[] { "key9", 29 }));

            DummyData.ForEach(d => d.Update());

            DummySetData = new List<DummyModelHasMany>();

            var set1 = DummyModelHasMany.Create(new object[] { "set1", 31 });
            DummySetData.Add(set1);
            set1.Set.Add(DummyData[0]);
            set1.Set.Add(DummyData[3]);
            set1.Set.Add(DummyData[5]);
            set1.Set.Add(DummyData[6]);
            set1.Set.Add(DummyData[2]);
            set1.Update();

            var set2 = DummyModelHasMany.Create(new object[] { "set2", 32 });
            DummySetData.Add(set2);
            set2.Set.Add(DummyData[3]);
            set2.Set.Add(DummyData[4]);
            set2.Set.Add(DummyData[5]);
            set2.Update();
        }

        #endregion


        /// <summary>
        ///A test for EditRecords
        ///</summary>
        [TestMethod()]
        [Ignore]
        public void EditRecordsTest() {
            DbRecordEditForm.EditRecords(
                null,
                "Test",
                typeof(DummyModelHasMany));
            
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
