using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubSonic.SqlGeneration.Schema;

namespace Common.Data.Test {
    class DummyModelHasMany : DbRecord, IDbRecord {

        #region Properties

        public String Name { get; set; }

        public Int32 Value { get; set; }

        [SubSonicIgnore]
        public HasMany<DummyModel> Set { get; protected set; }
        protected byte[] SetAsBinary {
            get { return Set.BinaryData; }
            set { Set.BinaryData = value; }
        }

        #endregion

        #region DbRecord Members

        public static DummyModelHasMany Create(Object Tag) {
            return DummyModelHasMany.Create<DummyModelHasMany>(Tag);
        }

        public static DummyModelHasMany Read(Int64 Id) {
            return DummyModelHasMany.Read<DummyModelHasMany>(Id);
        }

        public static List<DummyModelHasMany> Read() {
            return DummyModelHasMany.Read<DummyModelHasMany>();
        }

        public void Update() {
            Update<DummyModelHasMany>();
        }

        public bool Delete() {
            return Delete<DummyModelHasMany>();
        }

        #endregion

        #region IDbRecord Members

        public void InitializeWithDefaults(Object Tag) {
            Set = new HasMany<DummyModel>();

            if (Tag is object[]) {
                object[] options = (object[])Tag;
                Name = (String)options[0];
                Value = (Int32)options[1];
            }
        }

        public void AfterLoad() {
            //
        }

        public bool BeforeUpdate() {
            return true;
        }

        public bool BeforeDelete() {
            return true;
        }

        #endregion

    }
}
