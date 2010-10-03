using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SubSonic.SqlGeneration.Schema;
using Common.Configuration;

namespace Common.Data.Test.GUI {
    public class Book : DbRecord, IEditableDbRecord {

        #region Properties

        [SubSonicNullString]
        public string Name { get; set; }

        [Configuration("ConfigTest for Book", 10,
            ControlType = ConfigurationEntry.ControlTypes.TextBox)]
        public string ConfigTest { get; set; }

        #endregion

        #region DbRecord Members

        public static Book Create() {
            return Book.Create<Book>();
        }

        public static Book Create(Object Tag) {
            return Book.Create<Book>(Tag);
        }

        public static Book Read(Int64 Id) {
            return Book.Read<Book>(Id);
        }

        public static List<Book> Read() {
            return Book.Read<Book>();
        }

        public void Update() {
            Update<Book>();
        }

        public bool Delete() {
            return Delete<Book>();
        }

        #endregion

        #region IDbRecord Members

        public void InitializeWithDefaults(Object Tag) {
            if (Tag != null && Tag is String)
                Name = Tag as String;
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
            Book duplicate = Book.Create();

            // TODO

            return duplicate;
        }

        public static void ShowEditForm(IWin32Window Owner) {
            DbRecordEditForm.EditRecords(
                Owner,
                "Book bearbeiten",
                typeof(Book));
        }

        #endregion

    }
}
