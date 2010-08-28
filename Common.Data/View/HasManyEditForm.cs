using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.Collections;

namespace Common.Data {
    public partial class HasManyEditForm : Form {

        #region Properties / Class Variables

        /// <summary>
        /// Stores the actual instance of the owning record.
        /// </summary>
        protected DbRecord owningRecord;

        /// <summary>
        /// Instance of a DbRecord class, that is owner of the HasMany relation.
        /// </summary>
        public DbRecord OwningRecord {
            get {
                return owningRecord;
            }

            set {
                owningRecord = value;
                UpdateData();
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
                UpdateData();
            }
        }

        /// <summary>
        /// The type of the property as specified by PropertyName.
        /// </summary>
        /// <see cref="InitializePropertyReflection"/>
        protected PropertyInfo Property { get; set; }

        /// <summary>
        /// If the type of the property is HasMany&lt;T&gt;, this
        /// contains the type of T.
        /// </summary>
        /// <see cref="InitializePropertyReflection"/>
        protected Type HasManyType { get; set; }

        public List<DbRecord> AllRecords { get; protected set; }

        public List<DbRecord> SelectedRecords { get; protected set; }

        public List<DbRecord> UnselectedRecords { get; protected set; }

        #endregion

        #region Construtor / Initialization

        public HasManyEditForm() {
            InitializeComponent();
        }

        private void HasManyEditForm_Load(object sender, EventArgs e) {
            // Set up the split container
            splitContainer_Resize(sender, e);
        }

        #endregion

        #region Static Methods

        public static bool SelectRecords(IWin32Window Owner, DbRecord OwningRecord, String PropertyName) {
            using (var form = new HasManyEditForm()) {
                form.OwningRecord = OwningRecord;
                form.PropertyName = PropertyName;

                return (form.ShowDialog(Owner) == DialogResult.OK);
            }
        }

        #endregion

        #region Data Handling

        /// <summary>
        /// Resets the properties that are initialized by <see cref="UpdateData"/> to
        /// their default values.
        /// </summary>
        protected void ResetProperties() {
            Property = null;
            HasManyType = null;
            AllRecords = new List<DbRecord>();
            SelectedRecords = new List<DbRecord>();
            UnselectedRecords = new List<DbRecord>();
        }

        /// <summary>
        /// Makes some property and type checking and initializes +property+ and +hasManyType+.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when OwningRecord or PropertyName are not set.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the property defined by PropertyName is not found or does not correspond to the HasMany&lt;T&gt; type.</exception>
        protected void InitializePropertyReflection() {
            if (OwningRecord == null)
                throw new ArgumentNullException("OwningRecord", "OwningRecord must be set.");

            if (String.IsNullOrEmpty(PropertyName))
                throw new ArgumentNullException("PropertyName", "PropertyName must be set.");

            Property = OwningRecord.GetType().GetProperty(PropertyName);
            if (Property == null)
                throw new ArgumentException(String.Format("Could not find a property by the name of \"{0}\".", PropertyName), "PropertyName");

            string compiled_name = Property.PropertyType.Namespace + "." + Property.PropertyType.Name;
            if (compiled_name != "Common.Data.HasMany`1")
                throw new ArgumentException("The given property does not represent a HasMany<T> type.", "PropertyName");

            Type[] arguments = Property.PropertyType.GetGenericArguments();
            Trace.Assert(arguments.Length == 1);

            HasManyType = arguments[0];
        }

        /// <summary>
        /// Converts an anonymous object that implements the IList interface to a list of DbRecords.
        /// </summary>
        /// <param name="Obj">The anonymous object</param>
        /// <param name="List">The list to which to add the items</param>
        /// <exception cref="System.ArgumentNullException">Thrown when one of the input parameters is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the passed object does not implement the IList interface.</exception>
        /// <exception cref="System.InvalidCastException">Thrown when the list of items passed as anonymous object contains elements that cannot be cast to the DbRecord class.</exception>
        protected void AnonymousObjectToDbRecordList(object Obj, List<DbRecord> List) {
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
        /// Reads all records of the type specified in <see cref="HasManyType"/> from the database and stores the result in the property <see cref="AllRecords"/>.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when AllRecords, Property or HasManyType are null (see <see cref="ResetProperties"/> and <see cref="InitializePropertyReflection"/>.)</exception>
        /// <exception cref="System.MethodAccessException">Thrown when the HasManyType does not contain
        /// a static method named "Read".</exception>
        /// <exception cref="System.InvalidCastException">Thrown when the list of items returned by the read method (static member of the type in <see cref="HasManyType"/>) contains elements that cannot be cast to the DbRecord class.</exception>
        protected void ReadAllRecords() {
            if (AllRecords == null)
                throw new ArgumentNullException("AllRecords");

            if (Property == null)
                throw new ArgumentNullException("Property");

            if (HasManyType == null)
                throw new ArgumentNullException("HasManyType");

            // Bind to the read method
            MethodInfo read = null;
            try {
                read = HasManyType.GetMethod(
                    "Read",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new Type[] { },
                    null);
            } catch { }

            if (read == null)
                throw new MethodAccessException("Could not find the Read method.");

            // Invoke the read method
            object list = null;
            try {
                list = read.Invoke(null, null);
            } catch(Exception ex) {
                throw new MethodAccessException("Could not invoke the Read method.", ex);
            }

            if (list == null)
                return;

            AnonymousObjectToDbRecordList(list, AllRecords);
        }

        /// <summary>
        /// Retrieves a list of all records in the HasMany&lt;T&gt; collection as specified by <see cref="OwningRecords"/> and <see cref="PropertyName"/>. The result is stored in the <see cref="SelectedRecords"/> property.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when SelectedRecords, Property or HasManyType are null (see <see cref="ResetProperties"/> and <see cref="InitializePropertyReflection"/>.)</exception>
        /// <exception cref="System.MethodAccessException">Thrown when the HasManyType does not contain
        /// a static method named "Read".</exception>
        /// <exception cref="System.InvalidCastException">Thrown when the list of items contained in the HasMany set (see <see cref="OwningRecord"/> and <see cref="PropertyName"/>) contains elements that cannot be cast to the DbRecord class.</exception>
        protected void ReadSelectedRecords() {
            if (SelectedRecords == null)
                throw new ArgumentNullException("SelectedRecords");

            if (Property == null)
                throw new ArgumentNullException("Property");

            if (HasManyType == null)
                throw new ArgumentNullException("HasManyType");

            // Invoke the read method
            object list = null;
            try {
                list = Property.GetValue(OwningRecord, null);
            } catch (Exception ex) {
                throw new MethodAccessException("Could not get the HasMany set.", ex);
            }

            if (list == null)
                return;

            AnonymousObjectToDbRecordList(list, SelectedRecords);
        }

        /// <summary>
        /// Adds items to the <see cref="UnselectedRecords"/> property, that are contained in
        /// <see cref="AllRecords"/>, but not in <see cref="SelectedRecords"/>.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when <see cref="AllRecords"/> or <see cref="SelectedRecords"/> are null.</exception>
        protected void UpdateUnselectedRecords() {
            if (AllRecords == null)
                throw new ArgumentNullException("AllRecords");

            if (SelectedRecords == null)
                throw new ArgumentNullException("SelectedRecords");

            UnselectedRecords = (from r in AllRecords
                                where !SelectedRecords.Contains(r)
                                select r).ToList();
        }

        /// <summary>
        /// Adds a specific record to the given list view.
        /// </summary>
        /// <param name="Record">The record to add to the list view.</param>
        /// <param name="List">The list view to add the record to.</param>
        /// <returns>The newly created list view item.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when one of the input parameters is null.</exception>
        protected ListViewItem AddRecordToListView(DbRecord Record, ListView List) {
            if (Record == null)
                throw new ArgumentNullException("Record");

            if (List == null)
                throw new ArgumentNullException("List");

            ListViewItem item = new ListViewItem();

            if (Record is IEditableDbRecord)
                item.Text = ((IEditableDbRecord)Record).Name;
            else
                item.Text = Record.ToString();

            item.Tag = Record;

            return List.Items.Add(item);
        }

        /// <summary>
        /// Adds all records passed to the given list view.
        /// </summary>
        /// <param name="Records">List of DbRecords to be added to the list view.</param>
        /// <param name="List">The list view to add the records to.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when one of the input parameters is null.</exception>
        protected void AddRecordsToListView(List<DbRecord> Records, ListView List) {
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
        /// Adds all <see cref="SelectedRecords"/> to the left list view and all <see cref="UnselectedRecords"/> to the list view on the right.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when <see cref="SelectedRecords"/> or <see cref="UnselectedRecords"/> is null.</exception>
        protected void AddRecordsToListViews() {
            if (SelectedRecords == null)
                throw new ArgumentNullException("SelectedRecords");

            if (UnselectedRecords == null)
                throw new ArgumentNullException("UnselectedRecords");

            AddRecordsToListView(SelectedRecords, SelectedList);
            AddRecordsToListView(UnselectedRecords, UnselectedList);
        }

        /// <summary>
        /// Updates the list views with new data after the OwningRecord or PropertyName have changed.
        /// </summary>
        /// <see cref="OwningRecord"/>
        /// <see cref="PropertyName"/>
        protected void UpdateData() {
            UnselectedList.BeginUpdate();
            SelectedList.BeginUpdate();

            try {

                UnselectedList.Items.Clear();
                SelectedList.Items.Clear();

                ResetProperties();

                // Make sure the properties are set
                if (OwningRecord == null || String.IsNullOrEmpty(PropertyName))
                    return;

                // Analyze the OwningRecord and PropertyName.
                InitializePropertyReflection();

                // Get a list of all available records = (all).
                ReadAllRecords();

                // Get a list of all records that are already selected = (selected).
                ReadSelectedRecords();

                // Build a set (all - selected) = (available).
                UpdateUnselectedRecords();

                // Show (selected) and (available).
                AddRecordsToListViews();

            } finally {
                UnselectedList.EndUpdate();
                SelectedList.EndUpdate();
            }
        }

        #endregion

        #region GUI Event Handlers

        private void splitContainer_Resize(object sender, EventArgs e) {
            splitContainer.SplitterDistance = splitContainer.Width / 2;
        }

        private void List_Resize(object sender, EventArgs e) {
            ((ListView)sender).Columns[0].Width = ((ListView)sender).Width - 30;
        }

        #endregion

    }
}
