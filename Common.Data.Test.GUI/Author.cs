using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SubSonic.SqlGeneration.Schema;
using Common.Configuration;

namespace Common.Data.Test.GUI {
    public class Author : DbRecord, IEditableDbRecord {

        #region Properties
        
        [SubSonicNullString]
        public string Name { get; set; }

        [DbRecordCollectionView.Column(1, "Key.Name", "Key", -2)]
        [DbRecordCollectionView.Column(2, "Value.Name", "Value", -2)]
        [SubSonicIgnore]
        public Association<AssocKey, AssocValue> Associations { get; set; }

        #region Support for: Associations

        public byte[] AssociationsAsBinary {
            get { return Associations.BinaryData; }
            set { Associations.BinaryData = value; }
        }

        #endregion

        [Configuration("Bücher", 10,
            ControlType = ConfigurationEntry.ControlTypes.None)]
        [DbRecordCollectionView.Column(1, "Key.Name", "Buch", -1)]
        [SubSonicIgnore]
        public HasMany<Book> Books { get; set; }

        #region Support for: Books

        public byte[] BooksAsBinary {
            get { return Books.BinaryData; }
            set { Books.BinaryData = value; }
        }

        #endregion

        [Configuration("ConfigTest for Author", 10,
            ControlType = ConfigurationEntry.ControlTypes.TextBox)]
        public string ConfigTest { get; set; }

        #endregion

        #region Custom Model Methods

        public Author()
            : base() {
            Associations = new Association<AssocKey, AssocValue>();
            Books = new HasMany<Book>();
        }

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

        protected void AddAssociation(string key, string value) {
            var assockey = AssocKey.Create();
            assockey.Name = key;
            assockey.Update();

            var assocvalue = AssocValue.Create();
            assocvalue.Name = value;
            assocvalue.Update();

            Associations[assockey] = assocvalue;
        }

        protected void AddBook(string name) {
            var book = Book.Create();
            book.Name = name;
            book.Update();

            Books.Add(book);
        }

        public void InitializeWithDefaults(Object Tag) {
            AddAssociation("1", "=> 1 " + DateTime.Now.ToString());
            AddAssociation("2", "=> 2");
            AddAssociation("3", "=> 3");
            AddAssociation("4", "=> 4");
            AddAssociation("5", "=> 5");
            AddAssociation("6", "=> 6");

            AddBook("Book 1");
            AddBook("Book 2");
            AddBook("Book 3");
            AddBook("Book 4");
        }

        public void AfterLoad() {
            //
        }

        public bool BeforeUpdate() {
            // Delete all associations
            // TODO

            // Delete all books
            // TODO

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
