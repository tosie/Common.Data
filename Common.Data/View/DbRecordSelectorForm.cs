using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Common.Data {
    public partial class DbRecordSelectorForm : Form {

        #region Properties / Class Variables

        /// <summary>
        /// Gets/sets the record that is currently selected.
        /// </summary>
        public IEditableDbRecord SelectedRecord {
            get {
                return RecordSelector.SelectedRecord;
            }

            set {
                RecordSelector.SelectedRecord = value;
            }
        }

        /// <summary>
        /// Gets/sets the records to be shown in the list view.
        /// </summary>
        public List<IEditableDbRecord> Records {
            get {
                return RecordSelector.Records;
            }

            set {
                RecordSelector.Records = value;
            }
        }

        /// <summary>
        /// Gets/sets the type of record that is shown in the list view. This is
        /// used to compile the column definition.
        /// </summary>
        public Type RecordType {
            get {
                return RecordSelector.RecordType;
            }

            set {
                RecordSelector.RecordType = value;
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

        #region Constructors / Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public DbRecordSelectorForm() {
            InitializeComponent();
        }

        /// <summary>Initializes the form's basic properties. Call this before showing the form.</summary>
        /// <param name="Owner">Window that is the owner of the form that is shown.</param>
        /// <param name="Name">Name to use for the edit window (think user preferences = FormData).</param>
        /// <param name="Title">Text to show in the form's title bar and a caption label.</param>
        /// <param name="Records">The base records from which a user may select a record for editing.</param>
        /// <param name="RecordType">The type of the DbRecord subclass to show the form for.</param>
        /// <param name="SelectedRecord">Record to be selected when showing the form for the first time.</param>
        protected virtual void InitializeForm(IWin32Window Owner, String Name, String Title,
                Type RecordType, List<IEditableDbRecord> Records, IEditableDbRecord SelectedRecord) {
            // Important for FormData.LoadFormData and FormData.SaveFormData
            this.Name = Name;

            this.Text = Title;
            this.HeaderText = Title;

            this.Records = Records;
            this.RecordType = RecordType;
            this.SelectedRecord = SelectedRecord;

            if (Owner == null)
                this.ShowInTaskbar = true;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Shows a form that allows a user to select a record.
        /// </summary>
        /// <param name="Owner">Window that is the owner of the form that is shown</param>
        /// <param name="Name">Name to use for the edit window (think user preferences = FormData).</param>
        /// <param name="Title">Text to show in the form's title bar and a caption label.</param>
        /// <param name="Records">The base records from which a user may select a record for editing.</param>
        /// <param name="RecordType">The type of the DbRecord subclass to show the form for.</param>
        /// <param name="SelectedRecord">Record to be selected when showing the form for the first time.</param>
        public static IEditableDbRecord SelectRecord(IWin32Window Owner, String Name, String Title,
                Type RecordType, List<IEditableDbRecord> Records, IEditableDbRecord SelectedRecord) {

            using (var form = new DbRecordSelectorForm()) {
                form.InitializeForm(Owner, Name, Title, RecordType, Records, SelectedRecord);

                if (form.ShowDialog(Owner) == DialogResult.OK)
                    return form.SelectedRecord;
                else
                    return null;
            }
        }

        #endregion

        #region GUI Event Handlers

        private void DbRecordSelectorForm_Load(object sender, EventArgs e) {
            FormData.LoadFormData(this);
        }

        private void DbRecordSelectorForm_FormClosing(object sender, FormClosingEventArgs e) {
            FormData.SaveFormData(this);
        }

        private void DbRecordSelectorForm_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape)
                Close();
            else if (e.KeyCode == Keys.Enter && btnOk.Enabled)
                btnOk_Click(sender, null);
        }

        private void RecordSelector_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (btnOk.Enabled)
                btnOk_Click(sender, null);
        }

        private void RecordSelector_RecordSelected(Control sender, RecordEventArgs e) {
            btnOk.Enabled = e.Record != null;
        }

        private void btnOk_Click(object sender, EventArgs e) {
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        #endregion

    }
}
