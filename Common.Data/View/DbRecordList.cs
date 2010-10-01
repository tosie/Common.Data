using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;
using System.Diagnostics;

namespace Common.Data {
    /// <summary>
    /// Provides a user control that shows a list of records of the IEditableDbRecord type. New records can be added and existing one be removed.
    /// </summary>
    public partial class DbRecordList : UserControl {

        #region Properties / Class Variables

        readonly string debug_category = "DbRecordList";

        /// <summary>
        /// Holds the actual user provided list of records to be shown in the list view.
        /// </summary>
        protected List<IEditableDbRecord> preloadedRecords = null;

        /// <summary>
        /// Gets/sets a list of the records that should be shown in the list view. If this is null, the list of records to be shown is determined by the static Read method of the type specified in <see cref="RecordType"/>.
        /// </summary>
        [Browsable(false)]
        public List<IEditableDbRecord> PreloadedRecords {
            get {
                return preloadedRecords;
            }

            set {
                if (preloadedRecords == value)
                    return;

                preloadedRecords = value;
                RefreshView();
            }
        }

        /// <summary>
        /// Gets the list of records that are shown in the list view.
        /// </summary>
        [Browsable(false)]
        public List<IEditableDbRecord> Records { get; protected set; }

        /// <summary>
        /// Stores the actual record type.
        /// </summary>
        protected Type recordType;

        /// <summary>
        /// Type of the record class that should be shown in the list. Setting this will show all records of this type that are available in the database.
        /// </summary>
        [Browsable(false)]
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

                if (PreloadedRecords == null)
                    RefreshView();
            }
        }

        /// <summary>
        /// Reference to the static Create method of the <see cref="RecordType"/> type. Set when assigning a value to <see cref="RecordType"/>.
        /// </summary>
        protected MethodInfo createRecord = null;

        /// <summary>
        /// Reference to the static Read method of the <see cref="RecordType"/> type. Set when assigning a value to <see cref="RecordType"/>.
        /// </summary>
        protected MethodInfo readRecord = null;

        /// <summary>
        /// Used to remember a previously selected record.
        /// </summary>
        protected virtual IEditableDbRecord LastSelection { get; set; }

        /// <summary>
        /// Holds the reference to the selected record (<seealso cref="SelectedRecord"/>).
        /// </summary>
        protected IEditableDbRecord selectedRecord = null;

        /// <summary>
        /// Gets the record that is currently selected in the left list view. Use <see cref="SelectRecord"/> to select a record.
        /// </summary>
        [Browsable(false)]
        public IEditableDbRecord SelectedRecord {
            get {
                return selectedRecord;
            }

            protected set {
                if (selectedRecord == value)
                    return;

                selectedRecord = value;

                // Notify registered event handlers of the selection change.
                RecordEvent.Fire(this, RecordSelected, selectedRecord);
            }
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
        public DbRecordList() {
            InitializeComponent();
            ListToolStrip.Renderer = new NoBorderToolStripRenderer();

            List.Items.Clear();
            List_Resize(List, null);
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
        /// Creates a new record and adds it to the list of shown records. Can be overwritten to accomodate custom requirements.
        /// </summary>
        /// <param name="SuggestedName">The name to use for the new record.</param>
        /// <returns>An instance of a new record of a type that is specified by <see cref="RecordType"/>.</returns>
        protected virtual IEditableDbRecord CreateNewRecord(String SuggestedName) {
            var args = RecordEvent.Fire(this, AddingRecord, null);

            if (!args.Continue)
                return null;

            IEditableDbRecord result = args.Record;
            
            if (result == null) {
                if (createRecord == null)
                    return null;

                result = (IEditableDbRecord)createRecord.Invoke(null, null);

                result.Name = SuggestedName;
                result.Update();
            }

            Records.Add(result);

            RecordEvent.Fire(this, AddedRecord, result);

            return result;
        }

        /// <summary>
        /// Deletes a record from the database and removes it from the list of shown records.
        /// </summary>
        /// <param name="Record">The record to delete.</param>
        protected virtual bool DeleteRecord(IEditableDbRecord Record) {
            var args = RecordEvent.Fire(this, DeletingRecord, Record);
            if (!args.Continue)
                return false;

            if (Record.Delete()) {

                Records.Remove(Record);

                RecordEvent.Fire(this, DeletedRecord, Record);

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

        #region Data Handling (Owning Records)

        /// <summary>
        /// Loads all records to be displayed list view. This is only done when <see cref="RecordType"/> is set.
        /// </summary>
        protected virtual void ReloadData() {
            Debug.WriteLine("Reloading data ...", debug_category);

            // Only use the static Read method if there are no predefined records
            if (PreloadedRecords == null) {
                Debug.WriteLine("Reading all available records ...", debug_category);

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

            } else {
                Debug.WriteLine("Showing preloaded data ...", debug_category);

                Records = new List<IEditableDbRecord>(preloadedRecords.Count);
                for (int i = 0; i < preloadedRecords.Count; i++) {
                    Records.Add((IEditableDbRecord)preloadedRecords[i]);
                }

            }
        }

        #endregion

        #region GUI Support

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

        /// <summary>
        /// Disabled list redrawing. Call this before updating many records and call <see cref="EndUpdate"/> afterwards.
        /// </summary>
        public void BeginUpdate() {
            List.BeginUpdate();
        }

        /// <summary>
        /// Re-enabled list redrawing after a call to <see cref="BeginUpdate"/>. Call this after updating many records.
        /// </summary>
        public void EndUpdate() {
            List.EndUpdate();
        }

        /// <summary>
        /// Updates the list view with new data.
        /// </summary>
        public virtual void RefreshView() {
            BeginUpdate();

            try {

                btnAddRecord.Visible = (createRecord != null || AddingRecord != null);

                List.Items.Clear();

                // Reload the whole list of items to be shown.
                ReloadData();

                // Show the records in the list view.
                AddRecordsToListView();

            } finally {
                EndUpdate();
            }
        }

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

        /// <summary>
        /// Adds all loaded records to the list view.
        /// </summary>
        void AddRecordsToListView() {
            if (Records == null || Records.Count <= 0)
                return;

            Records.ForEach(j => AddRecordToListView(j));

            List.Sort();

            if (List.Items.Count > 0)
                List.Items[0].Selected = true;
        }

        #endregion

        #region GUI Event Handlers

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

        private void btnAdvanced_DropDownOpening(object sender, EventArgs e) {
            var button = (ToolStripDropDownButton)sender;
            SetStateOfMenuItems(button.DropDownItems);
        }

        private void List_KeyDown(object sender, KeyEventArgs e) {
            // Delete a selected record?
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
            var record = (e.Item == null ? null : (IEditableDbRecord)e.Item.Tag);
            SelectedRecord = (e.IsSelected ? record : null);
            LastSelection = SelectedRecord;
        }

        private void List_Resize(object sender, EventArgs e) {
            ((ListView)sender).Columns[0].Width = ((ListView)sender).Width - 30;
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
