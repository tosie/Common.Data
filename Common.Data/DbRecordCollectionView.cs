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
    /// Provides a user control to edit collections of DbRecord instances.
    /// </summary>
    public partial class DbRecordCollectionView : UserControl {

        #region Data Structures

        /// <summary>
        /// Used by <see cref="EditRecords"/> as a tuple to determine what columns
        /// to show in the right list view.
        /// </summary>
        public class ColumnDefinition {
            
            /// <summary>
            /// The value shown in a cell is retrieved using this property name.
            /// </summary>
            public String PropertyName { get; set; }

            /// <summary>
            /// The text to be shown as the column header.
            /// </summary>
            public String HeaderText { get; set; }

            /// <summary>
            /// The width with which the column is initialized.
            /// </summary>
            public Int32 InitialWidth { get; set; }

            /// <summary>
            /// Reference to the property info that corresponds to the <see cref="PropertyName"/> of the <see cref="KeyType"/> of the <see cref="SelectedRecord"/>. Set by <see cref="SetupColumns"/>.
            /// </summary>
            public PropertyInfo Property { get; set; }

            /// <summary>
            /// Reference to the "&lt;PropertyName&gt;FormatCollectionValue" method of the <see cref="CollectionEditForm.KeyType"/> of the <see cref="CollectionEditForm.SelectedRecord"/>. Set by <see cref="CollectionEditForm.SetupColumns"/>.
            /// </summary>
            /// <remarks>The method's signature should look like this: (object {collection item}) => (object).</remarks>
            public MethodInfo FormatValueMethod { get; set; }

            /// <summary>
            /// Create a new instance of this class.
            /// </summary>
            /// <param name="Property">The value shown in a cell is retrieved using this property name.</param>
            /// <param name="HeaderText">The text to be shown as the column header.</param>
            public ColumnDefinition(String Property, String HeaderText) {
                this.PropertyName = Property;
                this.HeaderText = HeaderText;

                this.InitialWidth = 150;
            }

            /// <summary>
            /// Create a new instance of this class.
            /// </summary>
            /// <param name="Property">The value shown in a cell is retrieved using this property name.</param>
            /// <param name="HeaderText">The text to be shown as the column header.</param>
            /// <param name="InitialWidth">The width with which the column is initialized.</param>
            public ColumnDefinition(String Property, String HeaderText, Int32 InitialWidth) {
                this.PropertyName = Property;
                this.HeaderText = HeaderText;
                this.InitialWidth = InitialWidth;
            }

            /// <summary>
            /// Returns a formatted string of the value of the property linked to by <see cref="Property"/>.
            /// </summary>
            /// <param name="Instance">Instance of an object that is based on the HasMany-Type and implements the <see cref="Property"/>.</param>
            /// <returns>A string to be shown in a list view cell.</returns>
            /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="Instance"/> is null.</exception>
            public String GetFormattedValue(Object Instance) {
                if (Instance == null)
                    return ""; // TODO
                    // throw new ArgumentNullException("Instance");

                if (Property == null)
                    return "";

                object value = Property.GetValue(Instance, null);

                if (FormatValueMethod != null) {
                    value = FormatValueMethod.Invoke(Instance, new object[] { value });
                }

                if (value == null)
                    return "";
                else
                    return value.ToString();
            }

        }

        /// <summary>
        /// Delegate to use when specifying a method that is responsible for initializing a drop down menu for the right list view.
        /// </summary>
        /// <param name="Sender">Reference to the form that called this method.</param>
        /// <param name="DropDownItems">Collection of drop down menu items to show.</param>
        /// <param name="List">Reference to the list that the menu items should be associated with.</param>
        public delegate void ContextMenuInitializer(DbRecordCollectionView Sender, ToolStripItemCollection DropDownItems, ListView List);

        /// <summary>
        /// Delegate to use when specifying a method that is called whenever the menu created by <see cref="ContextMenuInitializer"/> is opened.
        /// </summary>
        /// <param name="Sender">Reference to the form that called this method.</param>
        /// <param name="DropDownItems">Collection of drop down menu items to show.</param>
        /// <param name="List">Reference to the list that the menu items should be associated with.</param>
        public delegate void ContextMenuLoading(DbRecordCollectionView Sender, ToolStripItemCollection DropDownItems, ListView List);

        /// <summary>
        /// Enumeration of all supported collection types
        /// </summary>
        public enum CollectionType { HasMany, Association }

        /// <summary>
        /// Dictionary to convert from a full namespace to a collection type.
        /// </summary>
        protected readonly Dictionary<String, CollectionType> CollectionTypeNames = new Dictionary<String, CollectionType>() {
            { "Common.Data.HasMany`1", CollectionType.HasMany },
            { "Common.Data.Association`2", CollectionType.Association }
        };

        /// <summary>
        /// Dictionary to validate the number of generc arguments for a given collection type.
        /// </summary>
        protected readonly Dictionary<CollectionType, Int32> CollectionTypeArguments = new Dictionary<CollectionType, Int32>() {
            { CollectionType.HasMany, 1 },
            { CollectionType.Association, 2 }
        };

        #endregion

        #region Properties / Class Variables

        /// <summary>
        /// Used to remember a previously selected record.
        /// </summary>
        protected virtual IEditableDbRecord LastSelection { get; set; }
        
        /// <summary>
        /// Reference to the currently selected record. <seealso cref="SelectedRecord"/>.
        /// </summary>
        protected IEditableDbRecord selectedRecord = null;

        /// <summary>
        /// Gets or sets the record that is currently selected and thus whose collection should be shown.
        /// </summary>
        public IEditableDbRecord SelectedRecord {
            get {
                return selectedRecord;
            }

            set {
                if (selectedRecord == value)
                    return;

                selectedRecord = value;
                EditSelectedRecord();
            }
        }

        /// <summary>
        /// Name of the collection relation. Must correspond to a property name of the owning record that is of type Association&lt;TKey, TValue&gt; or HasMany&lt;T&gt;. Set by <see cref="PrepareControl"/>.
        /// </summary>
        public String PropertyName { get; protected set; }

        /// <summary>
        /// Contains the definitions for the columns to show in the list view.
        /// </summary>
        protected virtual ColumnDefinition[] Columns { get; set; }

        /// <summary>
        /// The property as specified by PropertyName.
        /// </summary>
        /// <see cref="InitializePropertyReflection"/>
        protected virtual PropertyInfo Property { get; set; }

        /// <summary>
        /// Contains the type of collection <see cref="PropertyName"/> stands for. Set by <see cref="InitializePropertyReflection"/>.
        /// </summary>
        protected virtual CollectionType PropertyCollectionType { get; set; }

        /// <summary>
        /// If the type of the property is HasMany&lt;T&gt;, this contains the type of T.
        /// If the property is an Association&ltTKey, TValue&gt;, this contains the type of TKey.
        /// </summary>
        /// <see cref="InitializePropertyReflection"/>
        protected virtual Type KeyType { get; set; }

        /// <summary>
        /// If the type of the property is HasMany&lt;T&gt;, this is null.
        /// If the property is an Association&lt;TKey, TValue&gt;, this contains the type of TValue.
        /// </summary>
        /// <see cref="InitializePropertyReflection"/>
        protected virtual Type ValueType { get; set; }

        /// <summary>
        /// Holds all records associated with the <see cref="SelectedRecord"/> via the <see cref="PropertyName"/>, if the property name corresponds with a HasMany&gt;T&lt; collection.
        /// </summary>
        public List<DbRecord> SelectedRecordsList { get; protected set; }

        /// <summary>
        /// Holds all records associated with the <see cref="SelectedRecord"/> via the <see cref="PropertyName"/>, if the property name corresponds with an Association&gt;TKey, TValue&lt; collection.
        /// </summary>
        public Dictionary<DbRecord, DbRecord> SelectedRecordsDict { get; protected set; }

        /// <summary>
        /// Returns the list of selected items from the right list view.
        /// </summary>
        public ListView.SelectedListViewItemCollection SelectedCollectionItems {
            get {
                return SelectedList.SelectedItems;
            }
        }

        /// <summary>
        /// Determines whether the elements in the right list view can be rearranged by user interaction. Set by <see cref="InitializePropertyReflection"/>.
        /// </summary>
        public Boolean AllowReordering {
            get {
                return SelectedList.AllowDrop;
            }

            protected set {
                SelectedList.AllowDrop = value;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event that is fired when the context down menu for the list view is opening.
        /// </summary>
        public event ContextMenuLoading MenuLoading;

        #endregion

        #region Constructors / Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public DbRecordCollectionView() {
            InitializeComponent();
            SelectedListToolStrip.Renderer = new NoBorderToolStripRenderer();
        }

        /// <summary>
        /// Initializes important properties. Call this before setting the <see cref="SelectedRecord"/> property.
        /// </summary>
        /// <param name="PropertyName">The name of the property of the <see cref="SelectedRecord"/> that represents the record collection to show.</param>
        /// <param name="Columns">Columns to show in the list view (<see cref="ColumnDefinition"/>).</param>
        /// <param name="MenuInitiliazer">A method that is called once to initialize a context menu.</param>
        /// <param name="MenuLoading">A method that is called everytime before a context menu is opened.</param>
        public virtual void PrepareControl(String PropertyName, ColumnDefinition[] Columns,
                ContextMenuInitializer MenuInitiliazer, ContextMenuLoading MenuLoading) {
            this.PropertyName = PropertyName;
            this.Columns = Columns;

            // Initialize the context menu.
            if (MenuInitiliazer != null) {
                MenuInitiliazer(
                    this,
                    this.btnSelectedAdvanced.DropDownItems,
                    this.SelectedList);
            }

            // Add the items of the toolstrip button's drop down menu to the actual context menu.
            SelectedListContextMenu.Items.Clear();
            SelectedListContextMenu.Items.AddRange(btnSelectedAdvanced.DropDownItems);

            // Make sure the button is only visible if it has drop down items.
            btnSelectedAdvanced.Visible = btnSelectedAdvanced.DropDownItems.Count > 0;

            // If there is no visible button on the tool strip, hide it.
            SelectedListToolStrip.Visible = btnSelectedAddRecord.Visible
                || btnSelectedRemoveRecord.Visible
                || btnSelectedAdvanced.Visible;

            // If the tool strip itself is not visible, increase the height of the list view.
            if (!SelectedListToolStrip.Visible)
                SelectedList.Height = Height - SelectedList.Margin.Top - SelectedList.Margin.Bottom;

            // Register an event handler that gets called every time the context menu opens.
            if (MenuLoading != null)
                this.MenuLoading += MenuLoading;
        }

        #endregion

        #region GUI Support

        /// <summary>
        /// Called whenever <see cref="SelectedRecord"/> is set.
        /// </summary>
        void EditSelectedRecord() {
            if (SelectedRecord == null) {
                Enabled = false;
                SelectedList.Items.Clear();
            } else if (LastSelection != SelectedRecord) {
                Enabled = true;
                UpdateData();
            }
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

        /// <summary>
        /// Disabled list redrawing. Call this before updating many records and call <see cref="EndUpdate"/> afterwards.
        /// </summary>
        public void BeginUpdate() {
            SelectedList.BeginUpdate();
        }

        /// <summary>
        /// Re-enabled list redrawing after a call to <see cref="BeginUpdate"/>. Call this after updating many records.
        /// </summary>
        public void EndUpdate() {
            SelectedList.EndUpdate();
        }

        #endregion

        #region Data Handling (Collection)

        /// <summary>
        /// Resets the properties that are initialized by <see cref="UpdateData"/> to
        /// their default values.
        /// </summary>
        protected virtual void ResetProperties() {
            Property = null;
            PropertyCollectionType = CollectionType.HasMany;
            KeyType = null;
            ValueType = null;
            SelectedRecordsList = null;
            SelectedRecordsDict = null;
        }

        /// <summary>
        /// Makes some property and type checking and initializes <see cref="Property"/> and <see cref="KeyType"/>.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when <see cref="SelectedRecord"/> or <see cref="PropertyName"/> are not set.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the property defined by <see cref="PropertyName"/> is not found or does not correspond to either the Association&lt;TKey, TValue&gt or the HasMany&lt;T&gt; type.</exception>
        protected virtual void InitializePropertyReflection() {
            if (SelectedRecord == null)
                throw new ArgumentNullException("SelectedRecord", "SelectedRecord must be set.");

            if (String.IsNullOrEmpty(PropertyName))
                throw new ArgumentNullException("PropertyName", "PropertyName must be set.");

            Property = SelectedRecord.GetType().GetProperty(PropertyName);
            if (Property == null)
                throw new ArgumentException(String.Format("Could not find a property by the name of \"{0}\".", PropertyName), "PropertyName");

            string compiled_name = Property.PropertyType.Namespace + "." + Property.PropertyType.Name;
            if (!CollectionTypeNames.ContainsKey(compiled_name))
                throw new ArgumentException("The given property is not supported.", "PropertyName");

            PropertyCollectionType = CollectionTypeNames[compiled_name];

            Type[] arguments = Property.PropertyType.GetGenericArguments();
            int expected_argument_count = CollectionTypeArguments[PropertyCollectionType];
            Trace.Assert(arguments.Length == expected_argument_count, "Argument count does not match expected argument count.");

            KeyType = arguments[0];
            ValueType = arguments.Length > 1 ? arguments[1] : null;

            // Only allow reordering when working with a HasMany<T> collection
            AllowReordering = PropertyCollectionType == CollectionType.HasMany;
        }

        /// <summary>
        /// Initializes the columns of the passed list view with the given column definition.
        /// </summary>
        /// <param name="Columns">The column definitions to use.</param>
        /// <param name="List">The list view to apply the column definitions too.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="List"/> or <see cref="KeyType"/> are null. Also thrown when <see cref="ValueType"/> is null and it is used.</exception>
        protected virtual void SetupColumns(ColumnDefinition[] Columns, ListView List) {
            if (List == null)
                throw new ArgumentNullException("List");

            if (KeyType == null)
                throw new ArgumentNullException("KeyType");

            if (PropertyCollectionType == CollectionType.Association && ValueType == null)
                throw new ArgumentNullException("ValueType");

            List.BeginUpdate();

            try {

                List.Columns.Clear();

                if (Columns == null || Columns.Length <= 0) {
                    Columns = new ColumnDefinition[] {
                        new ColumnDefinition("Key.Name", "Name", 250)
                    };
                }

                for (int i = 0; i < Columns.Length; i++) {
                    ColumnDefinition column = Columns[i];

                    List.Columns.Add(column.HeaderText, column.InitialWidth);

                    string property_name = column.PropertyName;
                    Type record_type = KeyType;

                    if (property_name.StartsWith("Key.")) {
                        property_name = property_name.Remove(0, "Key.".Length);
                    } else if (PropertyCollectionType == CollectionType.Association && property_name.StartsWith("Value.")) {
                        property_name = property_name.Remove(0, "Value.".Length);
                        record_type = ValueType;
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
        /// Adds all <see cref="SelectedRecordsList"/> to the right list view.
        /// </summary>
        protected virtual void AddRecordsToListViews() {
            if (SelectedRecordsList != null)
                AddRecordsToListView(SelectedRecordsList, SelectedList);
            else if (SelectedRecordsDict != null)
                AddRecordsToListView(SelectedRecordsDict, SelectedList);
            else
                Trace.Assert(false, "Do not know what records to add to the list views.");
        }

        /// <summary>
        /// Updates the list views with new data after the <see cref="SelectedRecord"/> or <see cref="PropertyName"/> have changed.
        /// </summary>
        /// <see cref="SelectedRecord"/>
        /// <see cref="PropertyName"/>
        protected virtual void UpdateData() {
            SelectedList.BeginUpdate();

            try {

                SelectedList.Items.Clear();

                ResetProperties();

                // Make sure the properties are set
                if (SelectedRecord == null || String.IsNullOrEmpty(PropertyName))
                    return;

                // Analyze the SelectedRecord and PropertyName.
                InitializePropertyReflection();

                // Initialize the list view columns
                SetupColumns(Columns, SelectedList);

                // Show records in the right list view.
                RefreshSelectedListData();

            } finally {
                SelectedList.EndUpdate();
            }
        }

        /// <summary>
        /// Refreshes the list of records shown in the list view.
        /// </summary>
        public void RefreshSelectedListData() {
            SelectedList.BeginUpdate();

            try {

                SelectedList.Items.Clear();

                try {
                    // Get a list of all records that are already selected = (selected).
                    if (PropertyCollectionType == CollectionType.HasMany)
                        ReadSelectedRecordsList();
                    else if (PropertyCollectionType == CollectionType.Association)
                        ReadSelectedRecordsDict();
                    else
                        Trace.Assert(false, "Unknown collection type.");

                    // Show (selected) and (available).
                    AddRecordsToListViews();
                } catch { }

            } finally {
                SelectedList.EndUpdate();
            }
        }

        #endregion

        #region Data Handling (Collection: HasMany<T>)

        /// <summary>
        /// Converts an anonymous object that implements the IList interface to a list of DbRecords.
        /// </summary>
        /// <param name="Obj">The anonymous object</param>
        /// <param name="List">The list to which to add the items</param>
        /// <exception cref="System.ArgumentNullException">Thrown when one of the input parameters is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the passed object does not implement the IList interface.</exception>
        /// <exception cref="System.InvalidCastException">Thrown when the list of items passed as anonymous object contains elements that cannot be cast to the DbRecord class.</exception>
        protected virtual void AnonymousObjectToDbRecordList(object Obj, List<DbRecord> List) {
            if (Obj == null)
                throw new ArgumentNullException("Obj");

            if (List == null)
                throw new ArgumentNullException("List");

            if (!(Obj is IList))
                throw new ArgumentException("The given object is not of an IList type.", "Obj");

            int count = ((IList)Obj).Count;

            List.Clear();
            List.Capacity = count;

            for (int i = 0; i < count; i++) {
                object item = ((IList)Obj)[i];
                List.Add(item as DbRecord);
            }
        }

        /// <summary>
        /// Retrieves a list of all records in the HasMany&lt;T&gt; collection as specified by <see cref="SelectedRecordsList"/> and <see cref="PropertyName"/>. The result is stored in the <see cref="SelectedRecordsList"/> property.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when Property or KeyType are null (see <see cref="ResetProperties"/> and <see cref="InitializePropertyReflection"/>.)</exception>
        /// <exception cref="System.MemberAccessException">Thrown when the KeyType does not contain a static method named "Read".</exception>
        /// <exception cref="System.InvalidCastException">Thrown when the list of items contained in the HasMany set (see <see cref="SelectedRecord"/> and <see cref="PropertyName"/>) contains elements that cannot be cast to the DbRecord class.</exception>
        protected virtual void ReadSelectedRecordsList() {
            if (Property == null)
                throw new ArgumentNullException("Property");

            if (KeyType == null)
                throw new ArgumentNullException("KeyType");

            SelectedRecordsList = new List<DbRecord>();

            // Invoke the property's get method
            object list = null;
            try {
                list = Property.GetValue(SelectedRecord, null);
            } catch (Exception ex) {
                throw new MemberAccessException("Could not get the HasMany set.", ex);
            }

            if (list == null)
                return;

            AnonymousObjectToDbRecordList(list, SelectedRecordsList);
        }

        /// <summary>
        /// Updates the list view item with the information provided by the record and according the the column definition.
        /// </summary>
        /// <param name="Record">The record to use.</param>
        /// <param name="Item">The item to apply the information too.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when one of the input parameters is null.</exception>
        protected virtual void UpdateListViewItemForRecord(DbRecord Record, ListViewItem Item) {
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
        /// <param name="List">The list view where the record can be found in.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when one of the input parameters is null.</exception>
        protected virtual void UpdateRecordInListView(DbRecord Record, ListView List) {
            if (Record == null)
                throw new ArgumentNullException("Record");

            if (List == null)
                throw new ArgumentNullException("List");

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
        /// <param name="List">The list view to add the record to.</param>
        /// <returns>The newly created list view item.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when one of the input parameters is null.</exception>
        protected virtual ListViewItem AddRecordToListView(DbRecord Record, ListView List) {
            if (Record == null)
                throw new ArgumentNullException("Record");

            if (List == null)
                throw new ArgumentNullException("List");

            ListViewItem item = new ListViewItem();

            UpdateListViewItemForRecord(Record, item);

            return List.Items.Add(item);
        }

        /// <summary>
        /// Adds all records passed to the given list view.
        /// </summary>
        /// <param name="Records">List of DbRecords to be added to the list view.</param>
        /// <param name="List">The list view to add the records to.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when one of the input parameters is null.</exception>
        protected virtual void AddRecordsToListView(List<DbRecord> Records, ListView List) {
            if (Records == null)
                throw new ArgumentNullException("Records");

            if (List == null)
                throw new ArgumentNullException("List");

            List.BeginUpdate();

            try {
                foreach (var record in Records) {
                    AddRecordToListView(record, List);
                }
            } finally {
                List.EndUpdate();
            }
        }

        /// <summary>
        /// Call this method whenever the value of a collection item has changed.
        /// </summary>
        /// <param name="Record">The collection item that has changed.</param>
        public void RecordChanged(DbRecord Record) {
            if (SelectedRecord == null)
                return;

            UpdateRecordInListView(Record, SelectedList);
        }

        #endregion

        #region Data Handling (Collection: Association<TKey, TValue>)

        /// <summary>
        /// Converts an anonymous object that implements the IDictionary interface to a dictionary of DbRecords.
        /// </summary>
        /// <param name="Obj">The anonymous object</param>
        /// <param name="Dictionary">The dictionary to which to add the items</param>
        /// <exception cref="System.ArgumentNullException">Thrown when one of the input parameters is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the passed object does not implement the IDictionary interface.</exception>
        /// <exception cref="System.InvalidCastException">Thrown when the dictionary of items passed as anonymous object contains elements that cannot be cast to the DbRecord class.</exception>
        protected virtual void AnonymousObjectToDbRecordDict(object Obj, Dictionary<DbRecord, DbRecord> Dictionary) {
            if (Obj == null)
                throw new ArgumentNullException("Obj");

            if (Dictionary == null)
                throw new ArgumentNullException("Dictionary");

            if (!(Obj is IDictionary))
                throw new ArgumentException("The given object is not of an IDictionary type.", "Obj");

            int count = ((IDictionary)Obj).Count;

            Dictionary.Clear();

            foreach (var key in ((IDictionary)Obj).Keys) {
                Dictionary[key as DbRecord] = ((IDictionary)Obj)[key] as DbRecord;
            }
        }

        /// <summary>
        /// Retrieves a list of all records in the HasMany&lt;T&gt; collection as specified by <see cref="SelectedRecordsList"/> and <see cref="PropertyName"/>. The result is stored in the <see cref="SelectedRecordsList"/> property.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when Property, KeyType or ValueType are null (see <see cref="ResetProperties"/> and <see cref="InitializePropertyReflection"/>.)</exception>
        /// <exception cref="System.MemberAccessException">Thrown when neither the KeyType nor the ValueType contain a static method named "Read".</exception>
        /// <exception cref="System.InvalidCastException">Thrown when the dictionary of items contained in the Association set (see <see cref="SelectedRecord"/> and <see cref="PropertyName"/>) contains elements that cannot be cast to the DbRecord class.</exception>
        protected virtual void ReadSelectedRecordsDict() {
            if (Property == null)
                throw new ArgumentNullException("Property");

            if (KeyType == null)
                throw new ArgumentNullException("KeyType");

            if (ValueType == null)
                throw new ArgumentNullException("ValueType");

            SelectedRecordsDict = new Dictionary<DbRecord, DbRecord>();

            // Invoke the property's get method
            object dict = null;
            try {
                dict = Property.GetValue(SelectedRecord, null);
            } catch (Exception ex) {
                throw new MemberAccessException("Could not get the Association set.", ex);
            }

            if (dict == null)
                return;

            AnonymousObjectToDbRecordDict(dict, SelectedRecordsDict);
        }

        /// <summary>
        /// Updates the list view item with the information provided by the record and according the the column definition.
        /// </summary>
        /// <param name="Record">The record to use.</param>
        /// <param name="Item">The item to apply the information too.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when one of the input parameters is null.</exception>
        protected virtual void UpdateListViewItemForRecord(KeyValuePair<DbRecord, DbRecord> Record, ListViewItem Item) {
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
        /// <param name="List">The list view where the record can be found in.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when one of the input parameters is null.</exception>
        protected virtual void UpdateRecordInListView(KeyValuePair<DbRecord, DbRecord> Record, ListView List) {
            if (Record.Key == null)
                throw new ArgumentNullException("Record.Key");

            if (List == null)
                throw new ArgumentNullException("List");

            foreach (ListViewItem item in List.Items) {
                var item_record = (KeyValuePair<DbRecord, DbRecord>)item.Tag;
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
        /// <param name="List">The list view to add the record to.</param>
        /// <returns>The newly created list view item.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when one of the input parameters is null.</exception>
        protected virtual ListViewItem AddRecordToListView(KeyValuePair<DbRecord, DbRecord> Record, ListView List) {
            if (Record.Key == null)
                throw new ArgumentNullException("Record.Key");

            if (List == null)
                throw new ArgumentNullException("List");

            ListViewItem item = new ListViewItem();

            UpdateListViewItemForRecord(Record, item);

            return List.Items.Add(item);
        }

        /// <summary>
        /// Adds all records passed to the given list view.
        /// </summary>
        /// <param name="Records">Dictionary of DbRecords to be added to the list view.</param>
        /// <param name="List">The list view to add the records to.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when one of the input parameters is null.</exception>
        protected virtual void AddRecordsToListView(Dictionary<DbRecord, DbRecord> Records, ListView List) {
            if (Records == null)
                throw new ArgumentNullException("Records");

            if (List == null)
                throw new ArgumentNullException("List");

            List.BeginUpdate();

            try {
                foreach (KeyValuePair<DbRecord, DbRecord> kv in Records) {
                    AddRecordToListView(kv, List);
                }
            } finally {
                List.EndUpdate();
            }
        }

        /// <summary>
        /// Call this method whenever the value of an association has changed.
        /// </summary>
        /// <param name="Record">The association that has changed.</param>
        public void RecordChanged(KeyValuePair<DbRecord, DbRecord> Record) {
            if (SelectedRecord == null)
                return;

            UpdateRecordInListView(Record, SelectedList);
        }

        #endregion

        #region GUI Event Handlers

        private void btnSelectedAdvanced_DropDownOpening(object sender, EventArgs e) {
            SetStateOfMenuItems(btnSelectedAdvanced.DropDownItems);

            if (MenuLoading != null)
                MenuLoading(this, btnSelectedAdvanced.DropDownItems, SelectedList);
        }

        private void SelectedListContextMenu_Opening(object sender, CancelEventArgs e) {
            SetStateOfMenuItems(SelectedListContextMenu.Items);

            if (MenuLoading != null)
                MenuLoading(this, SelectedListContextMenu.Items, SelectedList);
        }

        private void btnSelectedAddRecord_Click(object sender, EventArgs e) {
            // TODO
        }

        private void btnSelectedRemoveRecord_Click(object sender, EventArgs e) {
            // TODO
        }

        private void SelectedList_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Delete || e.KeyValue == 8) { // 8 = Backspace
                // Delete the selected record
                e.Handled = true;
                btnSelectedRemoveRecord_Click(sender, null);
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
