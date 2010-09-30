﻿using System;
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

            Author.ShowEditForm(this);
        }

        private void button2_Click(object sender, EventArgs e) {
            var columns = new DbRecordCollectionView.ColumnDefinition[] {
                new DbRecordCollectionView.ColumnDefinition("Key.Name", "Key", 150),
                new DbRecordCollectionView.ColumnDefinition("Value.Name", "Value", 100)
            };

            DbRecordCollectionView.ContextMenuInitializer menuinit = (Sender, DropDownItems, List) => {
                var tag = new object[] { Sender, DropDownItems, List };
            };

            DbRecordCollectionView.ContextMenuLoading menuload = (Sender, DropDownItems, List) => {
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
            var columns = new DbRecordCollectionView.ColumnDefinition[] {
                new DbRecordCollectionView.ColumnDefinition("Name", "Name", 150),
            };

            DbRecordCollectionView.ContextMenuInitializer menuinit = (Sender, DropDownItems, List) => {
                var tag = new object[] { Sender, DropDownItems, List };
            };

            DbRecordCollectionView.ContextMenuLoading menuload = (Sender, DropDownItems, List) => {
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
