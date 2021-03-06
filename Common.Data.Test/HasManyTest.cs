﻿using Common.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Diagnostics;

namespace Common.Data.Test
{
    
    
    /// <summary>
    ///This is a test class for HasManySortablesTest and is intended
    ///to contain all HasManySortablesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HasManyTest {


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
            Global.BeforeTest(testContextInstance.TestDir);

            InitializeDummyData();
        }

        [TestCleanup()]
        public void MyTestCleanup() {
            Global.AfterTest();
        }

        static List<DummyModel> DummyData;
        static byte[] DummyBinaryData;

        static void InitializeDummyData() {
            DummyData = new List<DummyModel>();

            DummyData.Add(DummyModel.Create(new object[] { "key1", 21 }));
            DummyData.Add(DummyModel.Create(new object[] { "key2", 22 }));
            DummyData.Add(DummyModel.Create(new object[] { "key3", 23 }));

            DummyData.ForEach(d => d.Update());

            // Should be a list of three entries with these identifiers:
            //   - 0
            //   - 1
            //   - 2
            DummyBinaryData = new byte[] {
                31, 139, 8, 0, 0, 0, 0, 0, 4, 0, 236, 189, 7, 96, 28, 73, 150, 37, 38, 47, 109, 202, 123, 127, 74, 245, 74, 215, 224, 116, 161, 8, 128, 96, 19, 36, 216, 144, 64, 16, 236, 193, 136, 205, 230, 146, 236, 29, 105, 71, 35, 41, 171, 42, 129, 202, 101, 86, 101, 93, 102, 22, 64, 204, 237, 157, 188, 247, 222, 123, 239, 189, 247, 222, 123, 239, 189, 247, 186, 59, 157, 78, 39, 247, 223, 255, 63, 92, 102, 100, 1, 108, 246, 206, 74, 218, 201, 158, 33, 128, 170, 200, 31, 63, 126, 124, 31, 63, 34, 30, 36, 7, 201, 195, 228, 255, 9, 0, 0, 255, 255, 43, 191, 84, 52, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
            };
        }

        #endregion

        /// <summary>
        /// A test for Data
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Common.Data.dll")]
        public void DataTest() {
            // TODO: Fix test

            //HasMany_Accessor<DummyModel> target = new HasMany_Accessor<DummyModel>();

            //var expected = DummyData;

            //target.Data = expected;
            //var actual = target.Data;

            //Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void DataAfterResortTest() {
            // TODO: Fix test

            //HasMany_Accessor<DummyModel> target = new HasMany_Accessor<DummyModel>();

            //var input = DummyData;
            //var expected = new List<DummyModel>(DummyData.Count);
            //expected.AddRange(input);
            //expected.Sort((a, b) => String.Compare(b.Name, a.Name));

            //target.Data = DummyData;
            //target.Data.Sort((a, b) => String.Compare(b.Name, a.Name));
            //var binary = target.BinaryData;
            //target.BinaryData = binary;
            //var output = target.Data;

            //Assert.AreEqual(expected.Count, output.Count);

            //bool are_equal = true;
            //for (int i = 0; i < output.Count; i++) {
            //    if (expected[i].Id != output[i].Id) {
            //        are_equal = false;
            //        break;
            //    }
            //}
            //Assert.IsTrue(are_equal);
        }

        [TestMethod()]
        public void BinaryDataSerializeTest() {
            // TODO: Fix test

//            HasMany_Accessor<DummyModel> target = new HasMany_Accessor<DummyModel>();

//            var input = DummyData;
//            var expected = DummyBinaryData;

//            target.Data = input;
//            var output = target.BinaryData;

//            Assert.AreEqual(expected.Length, output.Length);

//#if DEBUG
//            // Code to output the binary data:
//            string[] test = new string[output.Length];
//            for (int i = 0; i < output.Length; i++) {
//                test[i] = Convert.ToString(output[i]);
//            }
//            Debug.WriteLine("Expexted data: " + String.Join(", ", test), "BinaryDataSerializeTest");
//#endif

//            bool are_equal = true;
//            for (int i = 0; i < output.Length; i++) {
//                if (expected[i] != output[i]) {
//                    are_equal = false;
//                    break;
//                }
//            }
//            Assert.IsTrue(are_equal);
        }

        [TestMethod()]
        public void BinaryDataDeserializeTest() {
            // TODO: Fix test

            //var target = new HasMany_Accessor<DummyModel>();

            //var input = DummyData;
            //var expected = DummyData;

            //target.Data = input;
            //byte[] bindata = target.BinaryData;
            //target.BinaryData = bindata;
            //var output = target.Data;

            //Assert.AreEqual(expected.Count, output.Count);

            //bool are_equal = true;
            //for (int i = 0; i < output.Count; i++) {
            //    if (expected[i].Id != output[i].Id) {
            //        are_equal = false;
            //        break;
            //    }
            //}
            //Assert.IsTrue(are_equal);
        }
    }
}
