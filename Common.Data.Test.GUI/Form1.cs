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
            Author.ShowEditForm(this);
        }

        private void button2_Click(object sender, EventArgs e) {
            var columns = new CollectionEditForm.ColumnDefinition[] {
                new CollectionEditForm.ColumnDefinition("Key.Name", "Key", 150),
                new CollectionEditForm.ColumnDefinition("Value.Name", "Value", 100)
            };

            CollectionEditForm.DropDownMenuInitializer menuinit = (Sender, DropDownItems, List) => {
                var tag = new object[] { Sender, DropDownItems, List };
            };

            CollectionEditForm.DropDownMenuLoading menuload = (Sender, DropDownItems, List) => {
                // Nothing to do for now
            };

            CollectionEditForm.EditRecords(this,
                "Title",
                typeof(Author),
                "Associations",
                columns,
                null,
                null);
        }

        private void button3_Click(object sender, EventArgs e) {
            var columns = new CollectionEditForm.ColumnDefinition[] {
                new CollectionEditForm.ColumnDefinition("Name", "Name", 150),
            };

            CollectionEditForm.DropDownMenuInitializer menuinit = (Sender, DropDownItems, List) => {
                var tag = new object[] { Sender, DropDownItems, List };
            };

            CollectionEditForm.DropDownMenuLoading menuload = (Sender, DropDownItems, List) => {
                // Nothing to do for now
            };

            CollectionEditForm.EditRecords(this,
                "Title",
                typeof(Author),
                "Books",
                columns,
                null,
                null);
        }
    }
}
