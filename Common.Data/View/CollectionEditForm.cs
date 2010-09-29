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
        public delegate void DropDownMenuInitializer(CollectionEditForm Sender, ToolStripItemCollection DropDownItems, ListView List);

        /// <summary>
        /// Delegate to use when specifying a method that is called whenever the menu created by <see cref="DropDownMenuInitializer"/> is opened.
        /// </summary>
        /// <param name="Sender">Reference to the form that called this method.</param>
        /// <param name="DropDownItems">Collection of drop down menu items to show.</param>
        /// <param name="List">Reference to the list that the menu items should be associated with.</param>
        public delegate void DropDownMenuLoading(CollectionEditForm Sender, ToolStripItemCollection DropDownItems, ListView List);

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
        /// Stores the actual property name.
        /// </summary>
        protected String propertyName;

        /// <summary>
        /// Name of the HasMany relation. Must correspond to a property name of
        /// the owning record that is of type HasMany&lt;T&gt;.
        /// </summary>
        public String PropertyName {
            get {
                return propertyName;
            }

            set {
                propertyName = value;
            }
        }

        /// <summary>
        /// Contains the definitions for the columns to show in the right list view.
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
        /// Event that is fired when the drop down menu for the right list view is opening.
        /// </summary>
        public event DropDownMenuLoading MenuLoading;

        #endregion

        #region Constructors / Initialization

        /// <summary>
        /// Constructor. Use the static <see cref="EditRecords"/> method instead.
        /// </summary>
        protected CollectionEditForm() {
            InitializeComponent();
            ListToolStrip.Renderer = new NoBorderToolStripRenderer();
            SelectedListToolStrip.Renderer = new NoBorderToolStripRenderer();

            List.Items.Clear();
            List_Resize(List, null);

            EditSelectedRecord();
        }

        protected virtual void InitializeForm(String Title, Type RecordType,
                String PropertyName, ColumnDefinition[] Columns,
                DropDownMenuInitializer MenuInitiliazer,
                DropDownMenuLoading MenuLoading) {
            // Important for FormData.LoadFormData and FormData.SaveFormData
            string Name = RecordType.Name;
            this.Name = Name;

            this.Text = Title;
            this.lblText.Text = Title;

            this.RecordType = RecordType;
            this.PropertyName = PropertyName;
            this.Columns = Columns;

            if (MenuInitiliazer != null) {
                MenuInitiliazer(
                    this,
                    this.btnSelectedAdvanced.DropDownItems,
                    this.SelectedList);
            }

            if (MenuLoading != null)
                this.MenuLoading += MenuLoading;
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
                String PropertyName, ColumnDefinition[] Columns,
                DropDownMenuInitializer MenuInitiliazer,
                DropDownMenuLoading MenuLoading) {
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

                SelectedList.Items.Clear();
            } else if (LastSelection != SelectedRecord) {
                splitContainer.Panel2.Enabled = true;
                //Grid.BackColor = SystemColors.Window;

                UpdateData();
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
        /// Updates the list views with new data after the SelectedRecord or PropertyName have changed.
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
        /// Refreshes the list of records shown in the right list view.
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

        private void ScenarioForm_Load(object sender, EventArgs e) {
            FormData.LoadFormData(this);

            btnSelectedAdvanced.Visible = btnSelectedAdvanced.DropDownItems.Count > 0;

            SelectedListToolStrip.Visible = btnSelectedAddRecord.Visible
                || btnSelectedRemoveRecord.Visible
                || btnSelectedAdvanced.Visible;

            if (!SelectedListToolStrip.Visible)
                SelectedList.Height = splitContainer.Panel2.Height - SelectedList.Margin.Top - SelectedList.Margin.Bottom;
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

            var record_selected = SelectedRecord != null;

            foreach (ToolStripMenuItem item in button.DropDownItems) {
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

        private void btnSelectedAdvanced_DropDownOpening(object sender, EventArgs e) {
            btnAdvanced_DropDownOpening(sender, e);

            if (MenuLoading != null)
                MenuLoading(this, btnSelectedAdvanced.DropDownItems, SelectedList);
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

        private void btnSelectedAddRecord_Click(object sender, EventArgs e) {
            // TODO
        }

        private void btnSelectedRemoveRecord_Click(object sender, EventArgs e) {
            // TODO
        }

        private void SelectedList_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Delete || e.KeyValue == 8) { // 8 = Backspace
                e.Handled = true;
                btnSelectedRemoveRecord_Click(sender, null);
            }
        }

        private void SelectedList_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == System.Windows.Forms.MouseButtons.Right) {
                btnSelectedAdvanced.ShowDropDown();
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

            // TODO: Update selected record
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
