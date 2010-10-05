using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Common.Configuration;
using System.Diagnostics;

namespace Common.Data {
    public partial class DbRecordCollectionWithValueView : UserControl {
        
        #region Properties / Class Variables

        static readonly string debug_category = "DbRecordCollectionWithValueView";

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
                InitializeMenus();
                EditSelectedRecord();
            }
        }

        /// <summary>
        /// Reference to a method that is called everytime either the drop down menu of the advanced button is opening. Set by <see cref="InitializeMenus"/>.
        /// </summary>
        /// <remarks>
        /// Signature: (ToolStripMenuItems items).
        /// Name: [PropertyName]MenuOpening.
        /// </remarks>
        protected MethodInfo CollectionMenuOpening = null;

        /// <summary>
        /// Reference to a method that is called everytime a value has changed and needs to be validated. Set by <see cref="ReloadData"/>.
        /// </summary>
        /// <remarks>
        /// Signature: (object value) => (bool).
        /// Name: [PropertyName]FormatAssociationValue.
        /// </remarks>
        protected MethodInfo ValidateValue = null;

        /// <summary>
        /// Reference to a method that is called everytime a value needs to be converted to a string. Set by <see cref="ReloadData"/>.
        /// </summary>
        /// <remarks>
        /// Signature: (object value) => (string).
        /// Name: [PropertyName]ValidateAssociationValue.
        /// </remarks>
        protected MethodInfo FormatValue = null;

        #endregion

        #region Constructors / Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public DbRecordCollectionWithValueView() {
            InitializeComponent();
            ToolStrip.Renderer = new NoBorderToolStripRenderer();
        }

        #endregion

        #region Data Handling

        /// <summary>
        /// Updates the list views with new data after the <see cref="SelectedRecord"/> or <see cref="PropertyName"/> have changed.
        /// </summary>
        /// <see cref="SelectedRecord"/>
        /// <see cref="PropertyName"/>
        protected virtual void ReloadData() {
            // Read the records from the collection ...
            Collection.LoadData();

            // Make sure the properties are set.
            if (SelectedRecord == null || String.IsNullOrEmpty(PropertyName))
                return;

            if (Collection.PropertyCollectionType != RecordCollection.CollectionType.AssociationWithValue)
                throw new ArgumentOutOfRangeException("Only AssociationWithValue collections are supported.");

            // Use reflection to run a method that can validate input strings.
            var recordType = SelectedRecord.GetType();

            var validateValueMethodName = PropertyName + "ValidateAssociationValue";
            try {
                ValidateValue = recordType.GetMethod(validateValueMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            } catch {
                ValidateValue = null;
            }

            var formatValueMethodName = PropertyName + "FormatAssociationValue";
            try {
                FormatValue = recordType.GetMethod(formatValueMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            } catch {
                FormatValue = null;
            }
        }

        /// <summary>
        /// Creates a new <see cref="Common.Configuration.ConfigurationEntry"/> according to the collection and
        /// assigns it to the matrix.
        /// </summary>
        protected virtual void CreateAndAssignConfiguration() {
            var config = new ConfigurationEntry();

            config.Value = Collection.CollectionDictionaryWithValues;
            config.PropertyChanged += new PropertyChangedEventHandler(config_PropertyChanged);
            config.ValidateValue += new ConfigurationEntry.ValidateEvent(config_ValidateValue);
            config.FormatValue += new ConfigurationEntry.FormatValueHandler(config_FormatValue);

            Matrix.ConfigEntry = config;
        }

        void config_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "Value") {
                Collection.SaveData();
                SelectedRecord.Update();
            }
        }

        void config_ValidateValue(ConfigurationEntry Sender, ref object Value, out bool Valid) {
            Valid = true;
            if (ValidateValue == null)
                return;

            try {
                Valid = (bool)DirectInvoker.InvokeDirect(ValidateValue, SelectedRecord, new object[] { Value });
            } catch {
                Valid = false;
            }
        }

        string config_FormatValue(ConfigurationEntry Sender, object Value) {
            if (Value is IEditableDbRecord) {
                // Column and row headers ...
                return (Value as IEditableDbRecord).Name;
            } else {
                string result;
                try {
                    result = (string)DirectInvoker.InvokeDirect(FormatValue, SelectedRecord, new object[] { Value });
                } catch {
                    result = (Value == null ? "" : Value.ToString());
                }

                return result;
            }
        }

        #endregion

        #region GUI Support

        /// <summary>
        /// Tries to load the menu initializer handler of a property. If will do so by looking for an instance method named "[PropertyName]Menus" and calling that, if it exists, with the parameters (this, contextmenuitems, dropdownitems). The last two parameters represent a <see cref="ToolStripItemCollection"/> that can be modified. Either one of the item collectiong might be null, too.
        /// 
        /// Additionally a method named "[PropertyName]MenuOpening" will be searched that will be loaded everytime a menu is opening.
        /// </summary>
        void InitializeMenus() {
            // Clear all menus first.
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
                        null,
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
                Matrix.ConfigEntry = null;
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
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Updates the list view with new data after the <see cref="SelectedRecord"/> or <see cref="PropertyName"/> have changed.
        /// </summary>
        /// <seealso cref="SelectedRecord"/>
        /// <seealso cref="PropertyName"/>
        public virtual void RefreshView() {
            Matrix.ConfigEntry = null;

            // Reload the whole collection.
            ReloadData();

            // Make sure the properties are set.
            if (SelectedRecord == null || String.IsNullOrEmpty(PropertyName))
                return;

            // Show records in the matrix.
            CreateAndAssignConfiguration();
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
                Matrix.Height = Height - Matrix.Margin.Top - Matrix.Margin.Bottom;
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

        private void btnAddRecord_Click(object sender, EventArgs e) {
            // TODO
        }

        private void btnRemoveRecord_Click(object sender, EventArgs e) {
            // TODO
        }

        #endregion

    }
}
