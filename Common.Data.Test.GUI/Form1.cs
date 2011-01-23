using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Common.Data.Test.GUI {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            //var a = Author.Create();
            //a.Name = "Jackson";
            //a.Books.Add(Book.Create("Book 1"));
            //a.Books.Add(Book.Create("Book 2"));
            //a.Update();

            //for (int i = 0; i < 100; i++) {
            //    var a = Author.Create();
            //    a.Name = "Author " + i.ToString();
            //    a.Books.Add(Book.Create("Book 1 of Author " + i.ToString()));
            //    a.Books.Add(Book.Create("Book 2 or Author " + i.ToString()));
            //    a.Update();
            //}

            Author.ShowEditForm(this);
        }

        private void button2_Click(object sender, EventArgs e) {
            CollectionEditForm.EditRecords(this,
                "Title",
                typeof(Author),
                "Associations");
        }

        private void button3_Click(object sender, EventArgs e) {
            CollectionEditForm.EditRecords(this,
                "Title",
                typeof(Author),
                "Books");
        }

        private void button4_Click(object sender, EventArgs e) {
            var authors = Author.Read().Select(a => (IEditableDbRecord)a).ToList();
            TreeViewForm.EditRecords(this, "Author", "Autoren bearbeiten", authors);
        }

        private void button5_Click(object sender, EventArgs e) {
            var record = DbRecordSelectorForm.SelectRecord(
                this,
                "RecordSelector",
                "Select a book",
                typeof(Book),
                ListExtensions.ConvertTo<Book, IEditableDbRecord>(Book.Read()),
                null);

            if (record == null)
                MessageBox.Show("Nothing selected.");
            else
                MessageBox.Show("Selected: " + record.Name);
        }
    }
}
