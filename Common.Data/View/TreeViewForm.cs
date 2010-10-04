using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Common.Data {
    /// <summary>
    /// Provides a way of navigating through a lot of records using the <see cref="DbRecordTreeView"/> control in combination with <see cref="DbRecordView"/> and <see cref="DbRecordCollectionView"/>.
    /// </summary>
    public partial class TreeViewForm : Form {

        #region Properties / Class Variables

        /// <summary>
        /// Holds the list of all base records.
        /// </summary>
        [Browsable(false)]
        public List<IEditableDbRecord> RootRecords {
            get {
                return Tree.RootRecords;
            }

            set {
                Tree.RootRecords = value;
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

        #region Constructor / Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public TreeViewForm() {
            InitializeComponent();

            RecordView.Dock = DockStyle.Fill;
            CollectionView.Dock = DockStyle.Fill;
            CollectionWithValueView.Dock = DockStyle.Fill;
        }

        /// <summary>Initializes the form's basic properties. Call this before showing the form.</summary>
        /// <param name="Owner">Window that is the owner of the form that is shown.</param>
        /// <param name="Name">Name to use for the edit window (think user preferences = FormData).</param>
        /// <param name="Title">Text to show in the form's title bar and a caption label.</param>
        /// <param name="RootRecords">The base records from which a tree is built.</param>
        protected virtual void InitializeForm(IWin32Window Owner, String Name, String Title,
            List<IEditableDbRecord> RootRecords) {
            // Important for FormData.LoadFormData and FormData.SaveFormData
            this.Name = Name;

            this.Text = Title;
            this.HeaderText = Title;

            this.Tree.RootRecords = RootRecords;

            if (Owner == null)
                this.ShowInTaskbar = true;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Shows a form that allows a user to select a record and edit its properties.
        /// </summary>
        /// <param name="Owner">Window that is the owner of the form that is shown.</param>
        /// <param name="Name">Name to use for the edit window (think user preferences = FormData).</param>
        /// <param name="Title">Text to show in the form's title bar and a caption label.</param>
        /// <param name="RootRecords">The base records from which a tree is built.</param>
        public static void EditRecords(IWin32Window Owner, String Name, String Title,
                List<IEditableDbRecord> RootRecords) {
            using (var form = new TreeViewForm()) {
                form.InitializeForm(Owner, Name, Title, RootRecords);

                form.ShowDialog(Owner);
            }
        }

        #endregion

        #region GUI Support

        void ShowRecordView() {
            splitContainer.Panel2.Enabled = true;

            RecordView.SelectedRecord = Tree.SelectedRecord;

            RecordView.Show();
            CollectionView.Hide();
            CollectionWithValueView.Hide();
        }

        void ShowRecordCollection() {
            splitContainer.Panel2.Enabled = true;

            CollectionView.SelectedRecord = null;
            CollectionView.PropertyName = Tree.SelectedCollection.Name;
            CollectionView.SelectedRecord = Tree.SelectedRecord;

            CollectionView.Show();
            RecordView.Hide();
            CollectionWithValueView.Hide();
        }

        void ShowRecordCollectionWithValues() {
            splitContainer.Panel2.Enabled = true;

            CollectionWithValueView.SelectedRecord = null;
            CollectionWithValueView.PropertyName = Tree.SelectedCollection.Name;
            CollectionWithValueView.SelectedRecord = Tree.SelectedRecord;

            CollectionWithValueView.Show();
            CollectionView.Hide();
            RecordView.Hide();
        }

        void DisableRecordControls() {
            splitContainer.Panel2.Enabled = false;
        }

        #endregion

        #region GUI Events

        private void TreeViewForm_Load(object sender, EventArgs e) {
            FormData.LoadFormData(this);
        }

        private void TreeViewForm_FormClosing(object sender, FormClosingEventArgs e) {
            FormData.SaveFormData(this);
        }

        private void TreeViewForm_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        private void Tree_RecordSelected(Control sender, RecordEventArgs e) {
            if (e.Record == null) {
                DisableRecordControls();
                return;
            }

            ShowRecordView();
        }

        private void Tree_CollectionSelected(Control sender, CollectionEventArgs e) {
            if (e.Record == null) {
                DisableRecordControls();
                return;
            }

            if (e.Collection == null) {
                ShowRecordView();
            } else {
                if (RecordCollection.GetCollectionType(e.Collection) == RecordCollection.CollectionType.AssociationWithValue)
                    ShowRecordCollectionWithValues();
                else
                    ShowRecordCollection();
            }
        }

        #endregion

    }
}
