using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SubSonic.SqlGeneration.Schema;

namespace Common.Data.Test.GUI {
    public class AssocValue : DbRecord, IEditableDbRecord {

        #region Properties

        [SubSonicNullString]
        public string Name { get; set; }

        #endregion

        #region DbRecord Members

        public static AssocValue Create() {
            return AssocValue.Create<AssocValue>();
        }

        public static AssocValue Read(Int64 Id) {
            return AssocValue.Read<AssocValue>(Id);
        }

        public static List<AssocValue> Read() {
            return AssocValue.Read<AssocValue>();
        }

        public void Update() {
            Update<AssocValue>();
        }

        public bool Delete() {
            return Delete<AssocValue>();
        }

        #endregion

        #region IDbRecord Members

        public void InitializeWithDefaults(Object Tag) {
            // TODO
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

        #region IEditableDbRecord Members

        public IEditableDbRecord Duplicate() {
            AssocValue duplicate = AssocValue.Create();

            // TODO

            return duplicate;
        }

        public static void ShowEditForm(IWin32Window Owner) {
            DbRecordEditForm.EditRecords(
                Owner,
                "AssocValue bearbeiten",
                typeof(AssocValue));
        }

        #endregion

    }
}
