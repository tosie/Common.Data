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
    public partial class DbRecordEditForm : Form {

        #region Properties / Class Variables

        protected List<IEditableDbRecord> records;
        
        public List<IEditableDbRecord> Records {
            get {
                return records;
            }

            set {
                records = value;
                ShowRecords();
            }
        }

        protected Type recordType;

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

        protected MethodInfo createRecord = null;

        protected IEditableDbRecord LastSelection { get; set; }
        protected IEditableDbRecord SelectedRecord {
            get {
                if (List.SelectedItems.Count <= 0)
                    return null;

                return (IEditableDbRecord)List.SelectedItems[0].Tag;
            }
        }

        protected Boolean LoadingRecord { get; set; }

        #endregion

        #region Constructors / Initialization

        public DbRecordEditForm() {
            InitializeComponent();
            ListToolStrip.Renderer = new NoBorderToolStripRenderer();

            List.Items.Clear();
            LoadingRecord = false;
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

        #region Record Creation / Duplication

        protected IEditableDbRecord CreateNewRecord(String SuggestedName) {
            if (createRecord == null)
                return null;

            IEditableDbRecord result = (IEditableDbRecord)createRecord.Invoke(null, null);

            result.Name = SuggestedName;
            result.Update();

            return result;
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
            LoadingRecord = true;

            try {
                if (SelectedRecord == null) {
                    splitContainer.Panel2.Enabled = false;
                    //Grid.BackColor = SystemColors.ControlLight;

                    ConfigControl.Configuration = null;
                } else if (LastSelection != SelectedRecord) {
                    splitContainer.Panel2.Enabled = true;
                    //Grid.BackColor = SystemColors.Window;

                    ConfigControl.Configuration = GenericConfiguration.CreateFor(SelectedRecord);
                    foreach (ConfigurationEntry entry in ConfigControl.Configuration) {
                        entry.PropertyChanged += new PropertyChangedEventHandler(entry_PropertyChanged);
                    }
                }
            } finally {
                LoadingRecord = false;
            }
        }

        void entry_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "Value") {
                
                // TODO: Do not save here, bust just mark the record as dirty. Then save whenever
                //       another record is selected, the window is closed, etc.
                
                // Save any changes to the record
                SelectedRecord.Update();
            }
        }

        #endregion

        #region GUI Event Handlers

        private void ScenarioForm_Load(object sender, EventArgs e) {
            FormData.LoadFormData(this);
        }

        private void DbRecordEditFormForm_FormClosing(object sender, FormClosingEventArgs e) {
            FormData.SaveFormData(this);
        }

        private void DbRecordEditFormForm_KeyDown(object sender, KeyEventArgs e) {
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

            Int32 selected = List.SelectedItems[0].Index;

            if (!SelectedRecord.Delete())
                return;

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
            Boolean record_selected = SelectedRecord != null;

            foreach (ToolStripMenuItem item in btnRecordAdvanced.DropDownItems) {
                if (item.Tag == null || !(item.Tag is String))
                    continue;

                switch ((String)item.Tag) {
                    case "SelectedRecord != null":
                        // Enable if a Scenario is selected
                        item.Enabled = record_selected;
                        break;
                    default:
                        break;
                }
            }
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
