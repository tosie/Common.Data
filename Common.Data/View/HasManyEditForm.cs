using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

namespace Common.Data {
    public partial class HasManyEditForm : Form {

        #region Properties / Class Variables

        protected DbRecord owningRecord;

        public DbRecord OwningRecord {
            get {
                return owningRecord;
            }

            set {
                owningRecord = value;
                if (!String.IsNullOrEmpty(PropertyName))
                    UpdateData();
            }
        }

        protected String propertyName;

        public String PropertyName {
            get {
                return propertyName;
            }

            set {
                propertyName = value;
                if (OwningRecord != null)
                    UpdateData();
            }
        }

        #endregion

        #region Construtor / Initialization

        public HasManyEditForm() {
            InitializeComponent();
        }

        private void HasManyEditForm_Load(object sender, EventArgs e) {
            splitContainer.Dock = DockStyle.Fill;
        }

        #endregion

        #region Static Methods

        public static bool SelectRecords(IWin32Window Owner, DbRecord OwningRecord, String PropertyName) {
            using (var form = new HasManyEditForm()) {
                form.OwningRecord = OwningRecord;
                form.PropertyName = PropertyName;

                return (form.ShowDialog(Owner) == DialogResult.OK);
            }
        }

        #endregion

        #region Data Handling

        protected void UpdateData() {
            string debug_category = "Common.Data.HasManyEditForm";

            AvailableList.BeginUpdate();
            SelectedList.BeginUpdate();

            try {

                AvailableList.Items.Clear();
                SelectedList.Items.Clear();

                if (OwningRecord == null) {
                    Debug.WriteLine("OwningRecord is empty", debug_category);
                    return;
                }

                if (String.IsNullOrEmpty(PropertyName)) {
                    Debug.WriteLine("PropertyName is empty", debug_category);
                    return;
                }

                PropertyInfo pi = OwningRecord.GetType().GetProperty(PropertyName);
                if (pi == null) {
                    Debug.WriteLine(String.Format("Could not find property by name (\"{0}\")", PropertyName), debug_category);
                    return;
                }

                object hasmany = pi.GetValue(OwningRecord, null);
                if (hasmany == null) {
                    Debug.WriteLine("The property is null", debug_category);
                    return;
                }

                string compiled_name = hasmany.GetType().Namespace + "." + hasmany.GetType().Name;
                if (compiled_name != "Common.Data.HasMany`1") {
                    Debug.WriteLine("The properties type must return an object that is a Common.Data.HasMany class", debug_category);
                    return;
                }

                Type[] arguments = hasmany.GetType().GetGenericArguments();
                Trace.Assert(arguments.Length == 1);

                // Get a list of all available records = (all)
                // TODO

                // Get a list of all records that are already selected = (selected)
                // TODO

                // Build a set (all - selected) = (available)
                // TODO

                // Show (selected) and (available)
                // TODO

            } finally {
                AvailableList.EndUpdate();
                SelectedList.EndUpdate();
            }
        }

        #endregion

        #region GUI Event Handlers

        private void splitContainer_Resize(object sender, EventArgs e) {
            splitContainer.SplitterDistance = splitContainer.Width / 2;
        }

        private void List_Resize(object sender, EventArgs e) {
            ((ListView)sender).Columns[0].Width = ((ListView)sender).Width - 30;
        }

        #endregion

    }
}
