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
using Common.Data;

namespace Common.Data {
    public partial class CollectionEditForm : Form {

        #region Properties / Class Variables

        /// <summary>
        /// List of records to show in the list view to the left.
        /// </summary>
        public List<IEditableDbRecord> Records { get; protected set; }

        /// <summary>
        /// Stores the actual record type.
        /// </summary>
        protected Type recordType;

        /// <summary>
        /// Type of the record class that should be shown on the left.
        /// This is necessary to load all records.
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

                LoadRecords();
                ShowRecords();
            }
        }

        /// <summary>
        /// Reference to the static +Create+ method of the <see cref="RecordType"/> class. Set when
        /// assigning a value to <see cref="RecordType"/>.
        /// </summary>
        protected MethodInfo createRecord = null;

        /// <summary>
        /// Reference to the static +Read+ method of the <see cref="RecordType"/> class. Set when
        /// assigning a value to <see cref="RecordType"/>.
        /// </summary>
        protected MethodInfo readRecord = null;

        /// <summary>
        /// Used to remember a previously selected record.
        /// </summary>
        protected virtual IEditableDbRecord LastSelection { get; set; }

        /// <summary>
        /// Gets or sets the record that is currently selected in the left list view.
        /// </summary>
        public IEditableDbRecord SelectedRecord {
            get {
                if (List.SelectedItems.Count <= 0)
                    return null;

                return (IEditableDbRecord)List.SelectedItems[0].Tag;
            }

            set {
                SelectRecord(value, true, false);
            }
        }

        /// <summary>
        /// Name of the collection relation. Must correspond to a property name of the owning record that is of type Association&lt;TKey, TValue&gt; or HasMany&lt;T&gt;.
        /// </summary>
        public String PropertyName {
            get {
                return Collection.PropertyName;
            }
        }

        #endregion

        #region Constructors / Initialization

        /// <summary>
        /// Constructor. Use the static <see cref="EditRecords"/> method instead.
        /// </summary>
        protected CollectionEditForm() {
            InitializeComponent();
            ListToolStrip.Renderer = new NoBorderToolStripRenderer();

            List.Items.Clear();
            List_Resize(List, null);

            EditSelectedRecord();
        }

        protected virtual void InitializeForm(String Title, Type RecordType,
                String PropertyName, DbRecordCollectionView.ColumnDefinition[] Columns,
                DbRecordCollectionView.ContextMenuInitializer MenuInitiliazer,
                DbRecordCollectionView.ContextMenuLoading MenuLoading) {
            // Important for FormData.LoadFormData and FormData.SaveFormData
            string Name = RecordType.Name;
            this.Name = Name;

            this.Text = Title;
            this.lblText.Text = Title;

            this.RecordType = RecordType;

            Collection.PrepareControl(PropertyName, Columns, MenuInitiliazer, MenuLoading);
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Shows a form that allows a user to select a record and edit its properties.
        /// </summary>
        /// <param name="Owner">Window that is the owner of the form that is shown</param>
        /// <param name="Title">Text to show in the form's title bar and a caption label</param>
        /// <param name="RecordType">The type of the DbRecord subclass to show the form for</param>
        /// <param name="PropertyName">Name of the property of the RecordType class, that represents the HasMany&lt;T&gt; relation</param>
        /// <param name="Columns">The column definitions to use for the right list view.</param>
        public static void EditRecords(IWin32Window Owner, String Title, Type RecordType,
                String PropertyName, DbRecordCollectionView.ColumnDefinition[] Columns,
                DbRecordCollectionView.ContextMenuInitializer MenuInitiliazer,
                DbRecordCollectionView.ContextMenuLoading MenuLoading) {
            using (var form = new CollectionEditForm()) {
                form.InitializeForm(Title, RecordType, PropertyName, Columns, MenuInitiliazer, MenuLoading);

                if (Owner == null)
                    form.ShowInTaskbar = true;

                form.ShowDialog(Owner);
            }
        }

        #endregion

        #region Record Selection

        /// <summary>
        /// Select a record in the left list view.
        /// </summary>
        /// <param name="Record">The record to be selected.</param>
        /// <param name="EnsureVisibility">Make the list view item associated with the record visible?</param>
        /// <param name="EditAfterSelect">Start editing the record's name after selecting it?</param>
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

        /// <summary>
        /// Select a list item from the left list view.
        /// </summary>
        /// <param name="Record">The item to be selected.</param>
        /// <param name="EnsureVisibility">Make the list view item visible?</param>
        /// <param name="EditAfterSelect">Start editing the name of the record associated with the list item after selecting it?</param>
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
        /// Creates a new database record based in the <see cref="createRecord"/> method with <paramref name="SuggestedName"/> as its name.
        /// </summary>
        /// <param name="SuggestedName">The new record is named after this.</param>
        /// <returns>A new record with its name set to <paramref name="SuggestedName"/>.</returns>
        protected virtual IEditableDbRecord CreateNewRecord(String SuggestedName) {
            if (createRecord == null)
                return null;

            IEditableDbRecord result = (IEditableDbRecord)createRecord.Invoke(null, null);

            result.Name = SuggestedName;
            result.Update();

            Records.Add(result);

            return result;
        }

        protected virtual bool DeleteRecord(IEditableDbRecord Record) {
            if (Record.Delete()) {
                Records.Remove(Record);
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Finds out if there is record with a name like <paramref name="Name"/>.
        /// </summary>
        /// <param name="Name">The name to look for.</param>
        /// <returns>True if there is a record with the searched name, otherwise false.</returns>
        Boolean RecordNameAlreadyExists(String Name) {
            try {
                return Records.SingleOrDefault(j => j.Name == Name) != null;
            } catch {
                // Exception is thrown when the list contains more than one entry with the name
                return true;
            }
        }

        /// <summary>
        /// Determines what a record's name should be given the list of already existing records.
        /// </summary>
        /// <param name="Template">The template to use for naming. If there is a record with this name, a number will be appended to this and it will be increased until there is no record with that name.</param>
        /// <returns>A string representing a possible name for a new record.</returns>
        /// <remarks>Assume there are three records ("Red", "Blue", "Yellow"). If this method is called with <paramref name="Template"/> set to "Green", it will return "Green". If called with "Blue", it will return "Blue (2)". If called again with "Blue", it will return "Blue (3)".</remarks>
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

        /// <summary>
        /// Adds the given record the the left list view.
        /// </summary>
        /// <param name="Record"></param>
        /// <returns></returns>
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

                Collection.SelectedRecord = null;
            } else if (LastSelection != SelectedRecord) {
                splitContainer.Panel2.Enabled = true;
                //Grid.BackColor = SystemColors.Window;

                Collection.SelectedRecord = SelectedRecord;
            }
        }

        #endregion

        #region Data Handling (Owning Records)

        /// <summary>
        /// Loads all records to be displayed in the left list view.
        /// </summary>
        protected virtual void LoadRecords() {
            try {
                readRecord = recordType.GetMethod(
                    "Read",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new Type[] { },
                    null);

                IList read_result = (IList)readRecord.Invoke(null, null);

                Records = new List<IEditableDbRecord>(read_result.Count);
                for (int i = 0; i < read_result.Count; i++) {
                    Records.Add((IEditableDbRecord)read_result[i]);
                }
            } catch {
                readRecord = null;
                Records = new List<IEditableDbRecord>();
            }
        }

        #endregion

        #region GUI Event Handlers

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

        private void CollectionEditForm_Load(object sender, EventArgs e) {
            FormData.LoadFormData(this);
        }

        private void CollectionEditForm_FormClosing(object sender, FormClosingEventArgs e) {
            FormData.SaveFormData(this);
        }

        private void CollectionEditForm_KeyDown(object sender, KeyEventArgs e) {
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

            if (!DeleteRecord(SelectedRecord))
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
            ((ListView)sender).Columns[0].Width = ((ListView)sender).Width - 30;
        }

        private void btnAdvanced_DropDownOpening(object sender, EventArgs e) {
            var button = (ToolStripDropDownButton)sender;
            SetStateOfMenuItems(button.DropDownItems);
        }

        private void smiRenameRecord_Click(object sender, EventArgs e) {
            SelectRecord(List.SelectedItems[0], true, true);
        }

        private void smiDuplicateRecord_Click(object sender, EventArgs e) {
            // Remember the base instance
            IEditableDbRecord template = SelectedRecord;

            // Create a new instance
            IEditableDbRecord duplicate = template.Duplicate();
            duplicate.Name = FindNewNameForRecord(template.Name);
            duplicate.Update();
            Records.Add(duplicate);

            // Add the new instance to the list view
            ListViewItem new_item = AddRecordToListView(duplicate);
            SelectRecord(new_item, true, true);
        }

        #endregion

    }
}
