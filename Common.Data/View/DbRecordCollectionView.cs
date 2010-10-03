using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.Collections;

namespace Common.Data {
    /// <summary>
    /// Provides a user control to edit collections of a class type that implements the IEditableDbRecord interface.
    /// </summary>
    public partial class DbRecordCollectionView : UserControl {

        #region Properties / Class Variables

        static readonly string debug_category = "DbRecordCollectionView";

        /// <summary>
        /// The record collection that holds all the data.
        /// </summary>
        protected RecordCollection Collection = new RecordCollection();

        /// <summary>
        /// Gets or sets the record that is currently selected and thus whose collection should be shown.
        /// </summary>
        [Browsable(false)]
        public IEditableDbRecord SelectedRecord {
            get {
                return Collection.Record as IEditableDbRecord;
            }

            set {
                if (Collection.Record == value)
                    return;

                Collection.Record = value;
                Columns = null;
                InitializeMenus();
                EditSelectedRecord();
            }
        }

        /// <summary>
        /// Name of the property that represents the collection. Set this before setting the <see cref="SelectedRecord"/>.
        /// </summary>
        [Browsable(false)]
        public string PropertyName {
            get {
                return Collection.PropertyName;
            }

            set {
                if (Collection.PropertyName == value)
                    return;

                Collection.PropertyName = value;
                Columns = null;
                InitializeMenus();
                EditSelectedRecord();
            }
        }

        /// <summary>
        /// Reference to a method that is called everytime either the context menu or the drop down menu of the advanced button is opening. Set by <see cref="InitializeMenus"/>.
        /// </summary>
        protected MethodInfo CollectionMenuOpening = null;

        /// <summary>
        /// Contains the definitions for the columns to show in the list view.
        /// </summary>
        protected virtual ColumnDefinition[] Columns { get; set; }

        /// <summary>
        /// Returns the list of selected items from the list view. The Tag of each item either represents a DbRecord or a KeyValuePair&lt;DbRecord, DbRecord&gt; type, depending on <see cref="RecordCollection.PropertyCollectionType"/>.
        /// </summary>
        [Browsable(false)]
        public ListView.SelectedListViewItemCollection SelectedCollectionItems {
            get {
                return List.SelectedItems;
            }
        }

        /// <summary>
        /// Determines whether the elements in the list view can be rearranged by user interaction. Depends on the collection type.
        /// </summary>
        public Boolean AllowReordering {
            get {
                return Collection.AllowReordering && List.AllowDrop;
            }
        }

        #endregion

        #region Constructors / Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public DbRecordCollectionView() {
            InitializeComponent();
            ToolStrip.Renderer = new NoBorderToolStripRenderer();
        }

        #endregion

        #region Data Handling (Collection)

        /// <summary>
        /// Initializes the columns of the passed list view with the given column definition.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when <see cref="RecordCollection.KeyType"/> is null. Also thrown when <see cref="RecordCollection.ValueType"/> is null and it is used.</exception>
        protected virtual void SetupColumns() {
            if (Collection.KeyType == null)
                throw new ArgumentNullException("Collection.KeyType");

            if (Collection.PropertyCollectionType == RecordCollection.CollectionType.Association && Collection.ValueType == null)
                throw new ArgumentNullException("Collection.ValueType");

            List.BeginUpdate();

            try {

                List.Columns.Clear();

                if (Columns == null || Columns.Length <= 0) {
                    Columns = ColumnAttribute.GetColumnsForCollection(Collection.Property);

                    if (Columns.Length <= 0) {
                        Columns = new ColumnDefinition[] {
                            new ColumnDefinition("Key.Name", "Name", 250)
                        };
                    }
                }

                for (int i = 0; i < Columns.Length; i++) {
                    ColumnDefinition column = Columns[i];

                    var list_column = List.Columns.Add(column.HeaderText, column.InitialWidth);
                    list_column.Tag = column;

                    string property_name = column.PropertyName;
                    Type record_type = Collection.KeyType;

                    if (property_name.StartsWith("Key.")) {
                        property_name = property_name.Remove(0, "Key.".Length);
                    } else if (Collection.PropertyCollectionType == RecordCollection.CollectionType.Association && property_name.StartsWith("Value.")) {
                        property_name = property_name.Remove(0, "Value.".Length);
                        record_type = Collection.ValueType;
                    } else {
                        // Handle it just like the "Key." case above.
                    }



                    try {
                        column.Property = record_type.GetProperty(property_name);
                    } catch {
                        column.Property = null;
                    }

                    string method_name = property_name + "FormatCollectionValue";
                    try {
                        column.FormatValueMethod = record_type.GetMethod(method_name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
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
        /// Adds all records of the collection specified by <see cref="SelectedRecord"/> and <see cref="PropertyName"/> to the list view.
        /// </summary>
        protected virtual void AddRecordsToListView() {
            // Either add the HasMany<T> or the Association<TKey, TValue> records to the list.
            if (Collection.PropertyCollectionType == RecordCollection.CollectionType.HasMany)
                AddRecordsToListView(Collection.CollectionList); // HasMany<T>
            else if (Collection.PropertyCollectionType == RecordCollection.CollectionType.Association)
                AddRecordsToListView(Collection.CollectionDictionary); // Association<TKey, TValue>
            else
                Trace.Assert(false, "Do not know what records to add to the list views.");

            UpdateColumnWidths();
        }

        /// <summary>
        /// Updates the list views with new data after the <see cref="SelectedRecord"/> or <see cref="PropertyName"/> have changed.
        /// </summary>
        /// <see cref="SelectedRecord"/>
        /// <see cref="PropertyName"/>
        protected virtual void ReloadData() {
            // Reset basic properties before initializing the property reflection.
            Collection.LoadData();

            // Make sure the properties are set
            if (SelectedRecord == null || String.IsNullOrEmpty(PropertyName))
                return;

            // Initialize the list view columns
            SetupColumns();
        }

        #endregion

        #region Data Handling (Collection: HasMany<T>)

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
        protected virtual void AddRecordsToListView(List<IDbRecord> Records) {
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
        /// Call this method whenever the value of a collection item has changed.
        /// </summary>
        /// <param name="Record">The collection item that has changed.</param>
        public void RecordChanged(IDbRecord Record) {
            if (SelectedRecord == null)
                return;

            UpdateRecordInListView(Record);
        }

        #endregion

        #region Data Handling (Collection: Association<TKey, TValue>)

        /// <summary>
        /// Updates the list view item with the information provided by the record and according the the column definition.
        /// </summary>
        /// <param name="Record">The record to use.</param>
        /// <param name="Item">The item to apply the information too.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when one of the input parameters is null.</exception>
        protected virtual void UpdateListViewItemForRecord(KeyValuePair<IDbRecord, IDbRecord> Record, ListViewItem Item) {
            if (Record.Key == null)
                throw new ArgumentNullException("Record.Key");

            if (Item == null)
                throw new ArgumentNullException("Item");

            // Use the column definition to set up the text to be shown
            for (int i = 0; i < Columns.Length; i++) {
                ColumnDefinition column = Columns[i];

                string text = "";
                if (column.PropertyName.StartsWith("Value."))
                    text = column.GetFormattedValue(Record.Value);
                else
                    text = column.GetFormattedValue(Record.Key);

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
        protected virtual void UpdateRecordInListView(KeyValuePair<IDbRecord, IDbRecord> Record) {
            if (Record.Key == null)
                throw new ArgumentNullException("Record.Key");

            foreach (ListViewItem item in List.Items) {
                var item_record = (KeyValuePair<IDbRecord, IDbRecord>)item.Tag;
                if (item_record.Key != Record.Key)
                    continue;

                UpdateListViewItemForRecord(Record, item);
                break;
            }
        }

        /// <summary>
        /// Adds a specific record to the given list view.
        /// </summary>
        /// <param name="Record">The KeyValuePair of two records to be added to the list view.</param>
        /// <returns>The newly created list view item.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when one of the input parameters is null.</exception>
        protected virtual ListViewItem AddRecordToListView(KeyValuePair<IDbRecord, IDbRecord> Record) {
            if (Record.Key == null)
                throw new ArgumentNullException("Record.Key");

            ListViewItem item = new ListViewItem();

            UpdateListViewItemForRecord(Record, item);

            return List.Items.Add(item);
        }

        /// <summary>
        /// Adds all records passed to the given list view.
        /// </summary>
        /// <param name="Records">Dictionary of DbRecords to be added to the list view.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when one of the input parameters is null.</exception>
        protected virtual void AddRecordsToListView(Dictionary<IDbRecord, IDbRecord> Records) {
            if (Records == null)
                throw new ArgumentNullException("Records");

            List.BeginUpdate();

            try {
                foreach (KeyValuePair<IDbRecord, IDbRecord> kv in Records) {
                    AddRecordToListView(kv);
                }
            } finally {
                List.EndUpdate();
            }
        }

        /// <summary>
        /// Call this method whenever the value of an association has changed.
        /// </summary>
        /// <param name="Record">The association that has changed.</param>
        public void RecordChanged(KeyValuePair<IDbRecord, IDbRecord> Record) {
            if (SelectedRecord == null)
                return;

            UpdateRecordInListView(Record);
        }

        #endregion

        #region GUI Support

        /// <summary>
        /// Tries to load the menu initializer handler of a property. If will do so by looking for an instance method named "[PropertyName]Menus" and calling that, if it exists, with the parameters (this, contextmenuitems, dropdownitems). The last two parameters represent a <see cref="ToolStripItemCollection"/> that can be modified.
        /// 
        /// Additionally a method named "[PropertyName]MenuOpening" will be searched that will be loaded everytime a menu is opening.
        /// </summary>
        void InitializeMenus() {
            // Clear all menus first.
            contextMenu.Items.Clear();
            btnAdvanced.DropDownItems.Clear();

            // Can only initialize something, if a collection has been set.
            if (SelectedRecord == null || String.IsNullOrEmpty(PropertyName))
                return;

            // Use reflection to run a method that can initialize menu items.
            var recordType = SelectedRecord.GetType();

            var initMethodName = PropertyName + "Menus";
            try {
                var initMethod = recordType.GetMethod(initMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (initMethod != null)
                    initMethod.Invoke(SelectedRecord, new object[] {
                        this,
                        this.contextMenu.Items,
                        this.btnAdvanced.DropDownItems
                    });
            } catch { }

            var loadMethodName = PropertyName + "MenuOpening";
            try {
                CollectionMenuOpening = recordType.GetMethod(initMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            } catch {
                CollectionMenuOpening = null;
            }

            // Workaround for setting and retrieving the visibility of the buttons.
            // See the comment at http://msdn.microsoft.com/en-us/library/system.windows.forms.toolstripitem.visible.aspx.
            bool advanced_visible = (btnAdvanced.DropDownItems.Count > 0);

            // Make sure the advanced button is only visible if it has drop down items.
            btnAdvanced.Visible = advanced_visible;

            // If there is no visible button on the tool strip, hide it.
            UpdateToolbarVisibility();
        }

        /// <summary>
        /// Called whenever <see cref="SelectedRecord"/> is set.
        /// </summary>
        void EditSelectedRecord() {
            if (SelectedRecord == null) {
                Enabled = false;
                List.Items.Clear();
            } else {
                Enabled = true;
                RefreshView();
            }
        }

        /// <summary>
        /// Enabled or disables the given collection of ToolStripItems depending on their Tag value.
        /// </summary>
        /// <param name="items"></param>
        private void SetStateOfMenuItems(ToolStripItemCollection items) {
            var record_selected = SelectedRecord != null;

            foreach (ToolStripMenuItem item in items) {
                Debug.WriteLine(String.Format("[SetStateOfMenuItems] Processing menu item {0} ...", item.Text), debug_category);

                var condition = "";
                if (item.Tag is string) {
                    condition = (item.Tag as string);
                    Debug.WriteLine(String.Format("[SetStateOfMenuItems] Tag is string, new condition = {0}.", condition), debug_category);
                } else if (item.Tag is object[] && (item.Tag as object[]).Length >= 1 && (item.Tag as object[])[0] is string) {
                    condition = (item.Tag as object[])[0] as string;
                    Debug.WriteLine(String.Format("[SetStateOfMenuItems] Tag is object[] with [0] as string, new condition = {0}.", condition), debug_category);
                } else {
                    Debug.WriteLine(String.Format("[SetStateOfMenuItems] No new condition detected.", condition), debug_category);
                }

                if (String.IsNullOrEmpty(condition))
                    continue;

                switch (condition) {
                    case "SelectedRecord != null":
                        // Enable if there is an owning record.
                        item.Enabled = record_selected;
                        break;
                    case "SelectedCollectionItems.Count > 0":
                        // Enable if something from the collection has been selected.
                        item.Enabled = SelectedCollectionItems.Count > 0;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Disables list redrawing. Call this before updating many records and call <see cref="EndUpdate"/> afterwards.
        /// </summary>
        public void BeginUpdate() {
            List.BeginUpdate();
        }

        /// <summary>
        /// Re-enables list redrawing after a call to <see cref="BeginUpdate"/>. Call this after updating many records using the <see cref="RecordChanged"/> method.
        /// </summary>
        public void EndUpdate() {
            UpdateColumnWidths();

            List.EndUpdate();
        }

        /// <summary>
        /// Updates the list view with new data after the <see cref="SelectedRecord"/> or <see cref="PropertyName"/> have changed.
        /// </summary>
        /// <seealso cref="SelectedRecord"/>
        /// <seealso cref="PropertyName"/>
        public virtual void RefreshView() {
            List.BeginUpdate();

            try {

                List.Items.Clear();

                // Reload the whole collection.
                ReloadData();

                // Make sure the properties are set.
                if (SelectedRecord == null || String.IsNullOrEmpty(PropertyName))
                    return;

                // Show records in the list view.
                AddRecordsToListView();

            } finally {
                List.EndUpdate();
            }
        }

        /// <summary>
        /// Shows/Hides the toolbar according to the visibility of all its buttons. If no button is visible, the toolbar is hidden and the list view's height is increased.
        /// </summary>
        protected virtual void UpdateToolbarVisibility() {
            // Workaround for setting and retrieving the visibility of the buttons.
            // See the comment at http://msdn.microsoft.com/en-us/library/system.windows.forms.toolstripitem.visible.aspx.
            bool add_visible = btnAddRecord.Visible;
            bool remove_visible = btnRemoveRecord.Visible;
            bool advanced_visible = btnAdvanced.Visible;

            // If there is no visible button on the tool strip, hide it.
            bool toolstrip_visible = add_visible
                || remove_visible
                || advanced_visible;
            ToolStrip.Visible = toolstrip_visible;

            // If the tool strip itself is not visible, increase the height of the list view.
            if (!toolstrip_visible)
                List.Height = Height - List.Margin.Top - List.Margin.Bottom;
        }

        /// <summary>
        /// Called by the GUI event handlers whenever a menu is opening.
        /// </summary>
        /// <param name="items">The menu item collection that is opening.</param>
        protected virtual void OnCollectionMenuOpening(ToolStripItemCollection items) {
            if (CollectionMenuOpening != null) {
                try {
                    CollectionMenuOpening.Invoke(SelectedRecord, new object[] { items });
                } catch { }
            }
        }

        #endregion

        #region GUI Event Handlers

        private void btnAdvanced_DropDownOpening(object sender, EventArgs e) {
            SetStateOfMenuItems(btnAdvanced.DropDownItems);

            OnCollectionMenuOpening(btnAdvanced.DropDownItems);
        }

        private void contextMenu_Opening(object sender, CancelEventArgs e) {
            SetStateOfMenuItems(contextMenu.Items);

            OnCollectionMenuOpening(contextMenu.Items);
        }

        private void btnAddRecord_Click(object sender, EventArgs e) {
            // TODO
        }

        private void btnRemoveRecord_Click(object sender, EventArgs e) {
            // TODO
        }

        private void List_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Delete || e.KeyValue == 8) { // 8 = Backspace
                // Delete the selected record
                e.Handled = true;
                btnRemoveRecord_Click(sender, null);
            }
        }

        #endregion

        #region Drag & Drop

        ListView DragDropSource = null;

        protected virtual void ProcessDragDropItems(ListView.SelectedListViewItemCollection Items, Int32 DragIndex,
                ListView Source, ListView Target) {
            if (Items == null)
                throw new ArgumentNullException("Items");

            if (Source == null)
                throw new ArgumentNullException("Source");

            if (Target == null)
                throw new ArgumentNullException("Target");

            ListViewItem[] selected = new ListViewItem[Items.Count];
            for (int i = 0; i < Items.Count; i++) {
                selected[i] = Items[i];
            }

            Source.BeginUpdate();
            Target.BeginUpdate();

            try {

                for (int i = 0; i < selected.Length; i++) {
                    ListViewItem dragItem = selected[i];
                    int itemIndex = DragIndex;

                    if (dragItem.Index < itemIndex)
                        itemIndex++;
                    else
                        itemIndex = DragIndex + i;

                    // Insert the item at the mouse pointer
                    ListViewItem insertItem = (ListViewItem)dragItem.Clone();
                    if (itemIndex >= Target.Items.Count)
                        Target.Items.Add(insertItem);
                    else
                        Target.Items.Insert(itemIndex, insertItem);

                    // Remove the item from the source list
                    Source.Items.Remove(selected[i]);
                }
            } finally {
                Source.EndUpdate();
                Target.EndUpdate();
            }

            // TODO: Update SelectedRecord with the new order of the items
        }

        private void SelectedList_ItemDrag(object sender, ItemDragEventArgs e) {
            var list = (ListView)sender;
            DragDropSource = list;
            try {
                list.DoDragDrop(list.SelectedItems, DragDropEffects.Move);
            } finally {
                DragDropSource = null;
            }
        }

        private void SelectedList_DragEnter(object sender, DragEventArgs e) {
            // Accept data only from our own list views
            if (DragDropSource == null)
                return;

            var list = (ListView)sender;

            int length = e.Data.GetFormats().Length;
            for (int i = 0; i < length; i++) {
                if (e.Data.GetFormats()[i].Equals("System.Windows.Forms.ListView+SelectedListViewItemCollection")) {
                    // The data from the drag source is moved to the target
                    e.Effect = DragDropEffects.Move;
                }
            }
        }

        private void SelectedList_DragDrop(object sender, DragEventArgs e) {
            // Accept data only from our own list views
            if (DragDropSource == null)
                return;

            var list = (ListView)sender;

            // Returns the location of the mouse pointer in the ListView control
            Point cp = list.PointToClient(new Point(e.X, e.Y));

            // Obtain the item that is located at the specified location of the mouse pointer
            ListViewItem dragToItem = list.GetItemAt(cp.X, cp.Y);

            // Obtain the index of the item at the mouse pointer
            int dragIndex = (dragToItem == null ? list.Items.Count : dragToItem.Index);

            bool fromExternalSource = DragDropSource != list;
            if (fromExternalSource) {
                // Move items from one list view to another one

                int length = e.Data.GetFormats().Length;
                for (int k = 0; k < length; k++) {
                    if (!e.Data.GetFormats()[k].Equals("System.Windows.Forms.ListView+SelectedListViewItemCollection"))
                        continue;

                    Type datatype = typeof(ListView.SelectedListViewItemCollection);
                    ListView.SelectedListViewItemCollection data;
                    data = (ListView.SelectedListViewItemCollection)e.Data.GetData(datatype);

                    ProcessDragDropItems(data, dragIndex, DragDropSource, list);
                }
            } else {
                // Reorder items inside one list view

                ProcessDragDropItems(list.SelectedItems, dragIndex, list, list);
            }
        }

        #endregion

    }
}
