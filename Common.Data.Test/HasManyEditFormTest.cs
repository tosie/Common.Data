﻿using Common.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Diagnostics;

namespace Common.Data.Test
{
    
    
    /// <summary>
    ///This is a test class for HasManyEditFormTest and is intended
    ///to contain all HasManyEditFormTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HasManyEditFormTest {


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

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext) {
        }

        [ClassCleanup()]
        public static void MyClassCleanup() {
        }

        [TestInitialize()]
        public void MyTestInitialize() {
            Global.BeforeTest(testContextInstance.TestDir);

            InitializeDummyData();
        }

        [TestCleanup()]
        public void MyTestCleanup() {
            Global.AfterTest();
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
        }

        #endregion


        /// <summary>
        ///A test for OwningRecord
        ///</summary>
        [TestMethod()]
        public void UpdateDataTest() {
            var target = new HasManyEditForm();

            var input_record = DummySetData[0];
            var input_property = "Set";

            target.OwningRecord = input_record;
            target.PropertyName = input_property;

            Assert.AreEqual(DummyData.Count, target.AllRecords.Count, "AllRecords count");
            Assert.AreEqual(DummySetData[0].Set.Count, target.SelectedRecords.Count, "SelectedRecords count");
            Assert.AreEqual(DummyData.Count - DummySetData[0].Set.Count, target.UnselectedRecords.Count, "UnselectedRecords count");

            Assert.AreEqual(DummyData[0].Value, ((DummyModel)target.SelectedRecords[0]).Value, "First selected record");
            Assert.AreEqual(DummyData[1].Value, ((DummyModel)target.UnselectedRecords[0]).Value, "First unselected record");
        }

        /// <summary>
        /// Shows the HasManyEditForm (thus, this test is ignored and must be called manually).
        ///</summary>
        [TestMethod()]
        [Ignore]
        public void SelectRecordsTest() {
            var input_record = DummySetData[0];
            var input_property = "Set";

            Assert.AreEqual(21, input_record.Set[0].Value);

            HasManyEditForm.SelectRecords(null,
                "Move the first item to the last position, then add the last item from the right list to the last position on the left.",
                input_record, input_property);

            Assert.AreEqual(21, input_record.Set[input_record.Set.Count - 2].Value);
            Assert.AreEqual(29, input_record.Set[input_record.Set.Count - 1].Value);
        }
    }
}
