using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace Common.Data {
    public partial class DbRecordSelector : UserControl {

        #region Properties / Class Variables

        private Type recordType = null;

        /// <summary>
        /// Defines the type of records shown in the list view. You must set this.
        /// </summary>
        [Browsable(false)]
        public Type RecordType {
            get {
                return recordType;
            }

            set {
                if (recordType == value)
                    return;

                recordType = value;
                RefreshView();
            }
        }

        private List<IEditableDbRecord> records = null;

        /// <summary>
        /// The list of records to be shown in the list view.
        /// </summary>
        [Browsable(false)]
        public List<IEditableDbRecord> Records {
            get {
                return records;
            }

            set {
                if (records == value)
                    return;

                records = value;
                RefreshView();
            }
        }

        /// <summary>
        /// Gets or sets the record that is currently selected. If nothing is selected, this returns null.
        /// </summary>
        [Browsable(false)]
        public IEditableDbRecord SelectedRecord {
            get {
                if (List.SelectedItems.Count > 0)
                    return ((IEditableDbRecord)List.SelectedItems[0].Tag);
                else
                    return null;
            }

            set {
                // Unselect all
                List.SelectedItems.Clear();

                // Select the one specified
                foreach (ListViewItem item in List.Items) {
                    if (item.Tag == value) {
                        item.Selected = true;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Contains the definitions for the columns to show in the list view.
        /// </summary>
        protected virtual ColumnDefinition[] Columns { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// This event is fired whenever a record has been selected or a selection has been removed.
        /// </summary>
        [Description("This event is fired whenever a record has been selected or a selection has been removed.")]
        public event RecordEventHandler RecordSelected;

        /// <summary>
        /// Occurs when the control is double clicked by the mouse.
        /// </summary>
        [Description("Occurs when the control is double clicked by the mouse.")]
        new public event MouseEventHandler MouseDoubleClick;

        #endregion

        #region Constructors / Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public DbRecordSelector() {
            InitializeComponent();
        }

        #endregion

        #region Data Handling

        /// <summary>
        /// Initializes the columns of the passed list view with the given column definition.
        /// </summary>
        protected virtual void SetupColumns() {
            List.BeginUpdate();

            try {

                List.Columns.Clear();

                if (Columns == null || Columns.Length <= 0) {
                    Columns = ColumnAttribute.GetColumns(RecordType);

                    if (Columns.Length <= 0 && RecordType != null) {
                        Columns = new ColumnDefinition[] {
                            new ColumnDefinition(
                                RecordType.GetProperty("Name"),
                                "Name",
                                250)
                        };
                    }
                }

                for (int i = 0; i < Columns.Length; i++) {
                    ColumnDefinition column = Columns[i];

                    var list_column = List.Columns.Add(column.HeaderText, column.InitialWidth);
                    list_column.Tag = column;

                    try {
                        column.Property = column.Property;
                    } catch {
                        column.Property = null;
                    }

                    string method_name = column.Property.Name + "FormatValueForCell";
                    try {
                        column.FormatValueMethod = RecordType.GetMethod(method_name,
                            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    } catch {
                        column.FormatValueMethod = null;
                    }
                }

            } finally {

                List.EndUpdate();

            }
        }

        /// <summary>
        /// Updates the width of the columns after the contents of the list view have changed. Only columns with an
        /// initial width with a negative value (-1, -2) are affected, as those have a special meaning the to list
        /// view. All other columns remain unchanged.
        /// </summary>
        protected virtual void UpdateColumnWidths() {
            // Update the width of columns (only negative widths are re-applied as they have a special meaning
            // to the list view).
            for (int i = 0; i < List.Columns.Count; i++) {
                var list_column = List.Columns[i];
                var column = list_column.Tag as ColumnDefinition;

                if (column.InitialWidth < 0)
                    list_column.Width = column.InitialWidth;
            }
        }

        /// <summary>
        /// Updates the list view item with the information provided by the record and according the the column definition.
        /// </summary>
        /// <param name="Record">The record to use.</param>
        /// <param name="Item">The item to apply the information too.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when one of the input parameters is null.</exception>
        protected virtual void UpdateListViewItemForRecord(IDbRecord Record, ListViewItem Item) {
            if (Record == null)
                throw new ArgumentNullException("Record");

            if (Item == null)
                throw new ArgumentNullException("Item");

            // Use the column definition to set up the text to be shown
            for (int i = 0; i < Columns.Length; i++) {
                ColumnDefinition column = Columns[i];

                string text = column.GetFormattedValue(Record);

                if (i == 0) {
                    Item.Text = text;
                } else {
                    // Item.Text counts towards Item.SubItems.Count, thus -1
                    if (Item.SubItems.Count - 1 < i)
                        Item.SubItems.Add(text);
                    else
                        Item.SubItems[i].Text = text;
                }
            }

            Item.Tag = Record;
        }

        /// <summary>
        /// Updates a single record in the right list view without the need to recreate the whole list view.
        /// </summary>
        /// <param name="Record">The record whose changes should be propagated to the list view.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when one of the input parameters is null.</exception>
        protected virtual void UpdateRecordInListView(IDbRecord Record) {
            if (Record == null)
                throw new ArgumentNullException("Record");

            foreach (ListViewItem item in List.Items) {
                if ((IEditableDbRecord)item.Tag != Record)
                    continue;

                UpdateListViewItemForRecord(Record, item);
                break;
            }
        }

        /// <summary>
        /// Adds a specific record to the given list view.
        /// </summary>
        /// <param name="Record">The record to add to the list view.</param>
        /// <returns>The newly created list view item.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when one of the input parameters is null.</exception>
        protected virtual ListViewItem AddRecordToListView(IDbRecord Record) {
            if (Record == null)
                throw new ArgumentNullException("Record");

            ListViewItem item = new ListViewItem();

            UpdateListViewItemForRecord(Record, item);

            return List.Items.Add(item);
        }

        /// <summary>
        /// Adds all records passed to the given list view.
        /// </summary>
        /// <param name="Records">List of DbRecords to be added to the list view.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when one of the input parameters is null.</exception>
        protected virtual void AddRecordsToListView(List<IEditableDbRecord> Records) {
            if (Records == null)
                throw new ArgumentNullException("Records");

            List.BeginUpdate();

            try {
                foreach (var record in Records) {
                    AddRecordToListView(record);
                }
            } finally {
                List.EndUpdate();
            }
        }

        /// <summary>
        /// Adds all records of the collection specified by <see cref="Records"/> to the list view.
        /// </summary>
        protected virtual void AddRecordsToListView() {
            AddRecordsToListView(Records);
            UpdateColumnWidths();
        }

        /// <summary>
        /// Updates the list view with new data after the <see cref="Records"/> or <see cref="RecordType"/> properties have changed.
        /// </summary>
        protected virtual void ReloadData() {
            // Make sure the properties are set
            if (RecordType == null)
                return;

            // Initialize the list view columns
            SetupColumns();
        }

        #endregion

        #region GUI Support

        /// <summary>
        /// Updates the list view with new data after the <see cref="Records"/> or <see cref="RecordType"/> properties have changed.
        /// </summary>
        public virtual void RefreshView() {
            List.BeginUpdate();

            try {

                List.Items.Clear();

                // Reload the whole collection.
                ReloadData();

                // Make sure the properties are set.
                if (Records == null || Records.Count <= 0 || RecordType == null)
                    return;

                // Show records in the list view.
                AddRecordsToListView();

            } finally {
                List.EndUpdate();
            }
        }


        #endregion

        #region GUI Event Handlers

        private void List_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
            if (e.IsSelected && e.Item != null)
                RecordEvent.Fire(this, RecordSelected, e.Item.Tag as IEditableDbRecord);
            else
                RecordEvent.Fire(this, RecordSelected, null);
        }

        private void List_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (MouseDoubleClick != null)
                MouseDoubleClick(sender, e);
        }

        #endregion

    }
}
