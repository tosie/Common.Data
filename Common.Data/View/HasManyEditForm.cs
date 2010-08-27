using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Common.Data.View {
    public partial class HasManyEditForm : Form {
        
        public HasManyEditForm() {
            InitializeComponent();
        }

        private void HasManyEditForm_Load(object sender, EventArgs e) {
            splitContainer.Dock = DockStyle.Fill;
        }

        private void splitContainer_Resize(object sender, EventArgs e) {
            splitContainer.SplitterDistance = splitContainer.Width / 2;
        }

        private void List_Resize(object sender, EventArgs e) {
            ((ListView)sender).Columns[0].Width = ((ListView)sender).Width - 30;
        }

    }
}
