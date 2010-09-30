using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Common.Configuration;
using System.Reflection;
using System.Collections;

namespace Common.Data {
    /// <summary>
    /// A form that allows a list of IEditableDbRecord instances to be edited.
    /// </summary>
    public partial class DbRecordEditForm : Form {

        #region Properties / Class Variables

        /// <summary>
        /// Gets/sets the records to be shown in the list view.
        /// </summary>
        public List<IEditableDbRecord> Records {
            get {
                return RecordList.PreloadedRecords;
            }

            set {
                RecordList.PreloadedRecords = value;
            }
        }

        /// <summary>
        /// Gets/sets the type of record that is used when creating a new record.
        /// </summary>
        public Type RecordType {
            get {
                return RecordList.RecordType;
            }

            set {
                RecordList.RecordType = value;
            }
        }

        /// <summary>
        /// Gets/sets the text that is shown as a header text of the form (not its title, but like a subtitle).
        /// </summary>
        public String HeaderText {
            get { return lblText.Text; }
            set { lblText.Text = value; }
        }

        #endregion

        #region Events

        /// <summary>
        /// This event is fired before a new record is created.
        /// </summary>
        public event RecordEventHandler AddingRecord;

        /// <summary>
        /// This event is fired after a new record has been added to the list and database.
        /// </summary>
        public event RecordEventHandler AddedRecord;

        /// <summary>
        /// This event is fired before a record is deleted from the database and the list.
        /// </summary>
        public event RecordEventHandler DeletingRecord;

        /// <summary>
        /// This event is fired after a record is deleted from the database and the list.
        /// </summary>
        public event RecordEventHandler DeletedRecord;

        /// <summary>
        /// Fired when a record is selected and when a selected record is deselected.
        /// </summary>
        public event RecordEventHandler RecordSelected;

        #endregion

        #region Constructors / Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public DbRecordEditForm() {
            InitializeComponent();
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Shows a form that allows a user to select a record and edit its properties.
        /// </summary>
        /// <param name="Owner">Window that is the owner of the form that is shown</param>
        /// <param name="Title">Text to show in the form's title bar and a caption label</param>
        /// <param name="RecordType">The type of the DbRecord subclass to show the form for</param>
        public static void EditRecords(IWin32Window Owner, String Title, Type RecordType) {
            String Name = RecordType.Name;
            List<IEditableDbRecord> Records;

            try {
                MethodInfo read = RecordType.GetMethod(
                    "Read",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new Type[] { },
                    null);

                IList read_result = (IList)read.Invoke(null, null);

                Records = new List<IEditableDbRecord>(read_result.Count);
                for (int i = 0; i < read_result.Count; i++) {
                    Records.Add((IEditableDbRecord)read_result[i]);
                }
            } catch {
                Records = new List<IEditableDbRecord>();
            }

            EditRecords(Owner, Name, Title, RecordType, Records);
        }

        /// <summary>
        /// Shows a form that allows a user to select a record and edit its properties.
        /// </summary>
        /// <param name="Owner">Window that is the owner of the form that is shown</param>
        /// <param name="Name">Name to use for the edit window (think user preferences = FormData)</param>
        /// <param name="Title">Text to show in the form's title bar and a caption label</param>
        /// <param name="Records">The base records from which a user may select a record for editing</param>
        /// <param name="RecordType">The type of the DbRecord subclass to show the form for</param>
        public static void EditRecords(IWin32Window Owner, String Name, String Title,
                Type RecordType, List<IEditableDbRecord> Records) {
            using (DbRecordEditForm form = new DbRecordEditForm()) {
                // Important for FormData.LoadFormData and FormData.SaveFormData
                form.Name = Name;

                form.Text = Title;
                form.lblText.Text = Title;

                form.Records = Records;
                form.RecordType = RecordType;

                if (Owner == null)
                    form.ShowInTaskbar = true;

                form.ShowDialog(Owner);
            }
        }

        #endregion

        #region GUI Event Handlers

        private void DbRecordEditForm_Load(object sender, EventArgs e) {
            FormData.LoadFormData(this);
        }

        private void DbRecordEditForm_FormClosing(object sender, FormClosingEventArgs e) {
            FormData.SaveFormData(this);
        }

        private void DbRecordEditForm_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        private void RecordList_AddingRecord(Control sender, RecordEventArgs e) {
            var args = RecordEvent.Fire(this, AddingRecord, e.Record);
            e.CopyFrom(args);
        }

        private void RecordList_AddedRecord(Control sender, RecordEventArgs e) {
            var args = RecordEvent.Fire(this, AddedRecord, e.Record);
            e.CopyFrom(args);
        }

        private void RecordList_DeletingRecord(Control sender, RecordEventArgs e) {
            var args = RecordEvent.Fire(this, DeletingRecord, e.Record);
            e.CopyFrom(args);
        }

        private void RecordList_DeletedRecord(Control sender, RecordEventArgs e) {
            var args = RecordEvent.Fire(this, DeletedRecord, e.Record);
            e.CopyFrom(args);
        }

        private void RecordList_RecordSelected(Control sender, RecordEventArgs e) {
            RecordView.SelectedRecord = e.Record;

            var args = RecordEvent.Fire(this, RecordSelected, e.Record);
            e.CopyFrom(args);
        }

        #endregion

    }
}
