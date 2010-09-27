using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SubSonic.SqlGeneration.Schema;

namespace Common.Data.Test.GUI {
    public class Author : DbRecord, IEditableDbRecord {

        #region Properties
        
        [SubSonicNullString]
        public string Name { get; set; }

        #endregion

        #region DbRecord Members

        public static Author Create() {
            return Author.Create<Author>();
        }

        public static Author Read(Int64 Id) {
            return Author.Read<Author>(Id);
        }

        public static List<Author> Read() {
            return Author.Read<Author>();
        }

        public void Update() {
            Update<Author>();
        }

        public bool Delete() {
            return Delete<Author>();
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
            Author duplicate = Author.Create();

            // TODO

            return duplicate;
        }

        public static void ShowEditForm(IWin32Window Owner) {
            DbRecordEditForm.EditRecords(
                Owner,
                "Autorenliste bearbeiten",
                typeof(Author));
        }

        #endregion

    }
}
