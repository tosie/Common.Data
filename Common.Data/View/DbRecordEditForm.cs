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
        /// Holds the list of the records that are shown in the list view.
        /// </summary>
        protected List<IEditableDbRecord> records;
        
        /// <summary>
        /// Gets/sets the records to be shown in the list view.
        /// </summary>
        public List<IEditableDbRecord> Records {
            get {
                return records;
            }

            set {
                records = value;
                ShowRecords();
            }
        }

        /// <summary>
        /// Contains the type of record that is the basis when adding a new record.
        /// </summary>
        protected Type recordType;

        /// <summary>
        /// Gets/sets the type of record that is used when creating a new record.
        /// </summary>
        public Type RecordType {
            get {
                return recordType;
            }

            set {
                recordType = value;

                try {
                    createRecord = recordType.GetMethod(
                        "Create",
                        BindingFlags.Public | BindingFlags.Static,
                        null,
                        new Type[] { },
                        null);
                } catch {
                    createRecord = null;
                }

                btnAddRecord.Visible = createRecord != null;
            }
        }

        /// <summary>
        /// Method that is invoked when a new record should be created. Set by <see cref="RecordType"/>.
        /// </summary>
        protected MethodInfo createRecord = null;

        /// <summary>
        /// Contains the last selection made in the list view.
        /// </summary>
        protected IEditableDbRecord LastSelection { get; set; }

        /// <summary>
        /// Gets the currently selected record from the list view.
        /// </summary>
        protected IEditableDbRecord SelectedRecord {
            get {
                if (List.SelectedItems.Count <= 0)
                    return null;

                return (IEditableDbRecord)List.SelectedItems[0].Tag;
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
        public DbRecordEditForm() {
            InitializeComponent();
            ListToolStrip.Renderer = new NoBorderToolStripRenderer();

            List.Items.Clear();
            List_Resize(null, null);
            EditSelectedRecord();
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

        #region Record Selection

        void SelectRecord(IEditableDbRecord Record, Boolean EnsureVisibility, Boolean EditAfterSelect) {
            if (List.Items.Count <= 0)
                return;

            foreach (ListViewItem item in List.Items) {
                if ((IEditableDbRecord)item.Tag != Record)
                    continue;

                SelectRecord(item, EnsureVisibility, EditAfterSelect);
                break;
            }
        }

        void SelectRecord(ListViewItem Item, Boolean EnsureVisibility, Boolean EditAfterSelect) {
            Item.Selected = true;

            if (EnsureVisibility || EditAfterSelect)
                Item.EnsureVisible();

            if (EditAfterSelect)
                Item.BeginEdit();
        }

        #endregion

        #region Record Creation / Duplication / Deletion

        /// <summary>
        /// Creates a new record and adds it to the list of shown records. Can be overwritten to accomodate custom requirements.
        /// </summary>
        /// <param name="SuggestedName">The name to use for the new record.</param>
        /// <returns>An instance of a new record of a type that is specified by <see cref="RecordType"/>.</returns>
        protected virtual IEditableDbRecord CreateNewRecord(String SuggestedName) {
            if (createRecord == null)
                return null;

            IEditableDbRecord result = (IEditableDbRecord)createRecord.Invoke(null, null);

            result.Name = SuggestedName;
            result.Update();

            Records.Add(result);

            return result;
        }

        /// <summary>
        /// Deletes a record from the database and removes it from the list of shown records.
        /// </summary>
        /// <param name="Record">The record to delete.</param>
        protected virtual void DeleteRecord(IEditableDbRecord Record) {
            if (!Record.Delete())
                return;

            Records.Remove(Record);
        }

        Boolean RecordNameAlreadyExists(String Name) {
            try {
                return Records.SingleOrDefault(j => j.Name == Name) != null;
            } catch {
                // Exception is thrown when the list contains more than one entry with the name
                return true;
            }
        }

        String FindNewNameForRecord(String Template) {
            String template_first = Template;
            String template_more = Template + " ({0})";

            if (!RecordNameAlreadyExists(template_first))
                return template_first;

            Int32 counter = 1;
            String current_name;

            do {
                counter++;
                current_name = String.Format(template_more, counter);

                Trace.Assert(counter < Int32.MaxValue, "The counter is too high.");
            } while (RecordNameAlreadyExists(current_name));

            return current_name;
        }

        #endregion

        #region GUI Support

        ListViewItem AddRecordToListView(IEditableDbRecord Record) {
            ListViewItem item = new ListViewItem();

            item.Text = Record.Name;
            item.Tag = Record;

            return List.Items.Add(item);
        }

        void ShowRecords() {
            List.Items.Clear();

            if (Records == null || Records.Count <= 0)
                return;

            Records.ForEach(j => AddRecordToListView(j));
            List.Sort();

            if (List.Items.Count > 0)
                List.Items[0].Selected = true;
        }

        void EditSelectedRecord() {
            if (SelectedRecord == null) {
                splitContainer.Panel2.Enabled = false;
                //Grid.BackColor = SystemColors.ControlLight;
            } else if (LastSelection != SelectedRecord) {
                splitContainer.Panel2.Enabled = true;
                //Grid.BackColor = SystemColors.Window;
            }

            RecordView.SelectedRecord = SelectedRecord;
        }

        /// <summary>
        /// Enabled or disables the given collection of ToolStripItems depending on their Tag value.
        /// </summary>
        /// <param name="items"></param>
        private void SetStateOfMenuItems(ToolStripItemCollection items) {
            var record_selected = SelectedRecord != null;

            foreach (ToolStripMenuItem item in items) {
                if (item.Tag == null || !(item.Tag is String))
                    continue;

                switch ((String)item.Tag) {
                    case "SelectedRecord != null":
                        // Enable if something in the left list view is selected
                        item.Enabled = record_selected;
                        break;
                    default:
                        break;
                }
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

        private void btnAddRecord_Click(object sender, EventArgs e) {
            // Create a new instance
            IEditableDbRecord new_record = CreateNewRecord(FindNewNameForRecord("Neuer Eintrag"));
            if (new_record == null)
                return;

            // Show it in the list view
            ListViewItem new_item = AddRecordToListView(new_record);
            SelectRecord(new_item, true, true);
        }

        private void btnDeleteRecord_Click(object sender, EventArgs e) {
            if (SelectedRecord == null)
                return;

            DeleteRecord(SelectedRecord);

            Int32 selected = List.SelectedItems[0].Index;
            List.SelectedItems[0].Remove();

            if (List.Items.Count > selected)
                SelectRecord(List.Items[selected], true, false);
            else if (List.Items.Count > 0)
                SelectRecord(List.Items[selected - 1], true, false);
        }

        private void List_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Delete || e.KeyValue == 8) { // 8 = Backspace
                e.Handled = true;
                btnDeleteRecord_Click(sender, null);
            }
        }

        private void List_AfterLabelEdit(object sender, LabelEditEventArgs e) {
            if (e.Label == null || e.Label == String.Empty) {
                e.CancelEdit = true;
                return;
            }

            IEditableDbRecord record = (IEditableDbRecord)List.Items[e.Item].Tag;
            record.Name = e.Label;
            record.Update();

            List.Sort();
        }

        private void List_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
            EditSelectedRecord();
            LastSelection = SelectedRecord;
        }

        private void List_Resize(object sender, EventArgs e) {
            List.Columns[0].Width = List.Width - 30;
        }

        private void btnRecordAdvanced_DropDownOpening(object sender, EventArgs e) {
            SetStateOfMenuItems(btnRecordAdvanced.DropDownItems);
        }

        private void smiRenameScenario_Click(object sender, EventArgs e) {
            SelectRecord(List.SelectedItems[0], true, true);
        }

        private void smiDuplicateScenario_Click(object sender, EventArgs e) {
            // Basisinstanz merken
            IEditableDbRecord template = SelectedRecord;

            // Neue Instanz erstellen
            IEditableDbRecord duplicate = template.Duplicate();
            duplicate.Name = FindNewNameForRecord(template.Name);
            duplicate.Update();
            Records.Add(duplicate);

            // In ListView anzeigen
            ListViewItem new_item = AddRecordToListView(duplicate);
            SelectRecord(new_item, true, true);
        }

        #endregion

    }
}
