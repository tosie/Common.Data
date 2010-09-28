using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SubSonic.SqlGeneration.Schema;

namespace Common.Data.Test.GUI {
    public class AssocKey : DbRecord, IEditableDbRecord {

        #region Properties

        [SubSonicNullString]
        public string Name { get; set; }

        #endregion

        #region DbRecord Members

        public static AssocKey Create() {
            return AssocKey.Create<AssocKey>();
        }

        public static AssocKey Read(Int64 Id) {
            return AssocKey.Read<AssocKey>(Id);
        }

        public static List<AssocKey> Read() {
            return AssocKey.Read<AssocKey>();
        }

        public void Update() {
            Update<AssocKey>();
        }

        public bool Delete() {
            return Delete<AssocKey>();
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
            AssocKey duplicate = AssocKey.Create();

            // TODO

            return duplicate;
        }

        public static void ShowEditForm(IWin32Window Owner) {
            DbRecordEditForm.EditRecords(
                Owner,
                "AssocKey bearbeiten",
                typeof(AssocKey));
        }

        #endregion

    }
}
