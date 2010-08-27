using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubSonic.SqlGeneration.Schema;
using SubSonic.Repository;

namespace Common.Data.Test {
    class DummyModel : DbRecord, IDbRecord {

        #region Properties

        public String Name { get; set; }

        public Int32 Value { get; set; }

        #endregion

        #region DbRecord Members

        public static DummyModel Create(Object Tag) {
            return DummyModel.Create<DummyModel>(Tag);
        }

        public static DummyModel Read(Int64 Id) {
            return DummyModel.Read<DummyModel>(Id);
        }

        public static List<DummyModel> Read() {
            return DummyModel.Read<DummyModel>();
        }

        public void Update() {
            Update<DummyModel>();
        }

        public bool Delete() {
            return Delete<DummyModel>();
        }

        #endregion

        #region IDbRecord Members

        public void InitializeWithDefaults(Object Tag) {
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
