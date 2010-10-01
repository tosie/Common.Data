using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Configuration;

namespace Common.Data {
    /// <summary>
    /// Provides a user control that is able to edit a single type that implements the <see cref="IEditableDbRecord"/> interface.
    /// </summary>
    public partial class DbRecordView : UserControl {

        #region Properties / Class Variables

        /// <summary>
        /// This field holds an instance to the record that is to be edited.
        /// </summary>
        protected IEditableDbRecord selectedRecord = null;

        /// <summary>
        /// Gets/sets the record instance to be edited.
        /// </summary>
        [Browsable(false)]
        public IEditableDbRecord SelectedRecord {
            get {
                return selectedRecord;
            }

            set {
                if (selectedRecord == value)
                    return;

                selectedRecord = value;
                RefreshView();
            }
        }

        /// <summary>
        /// Used by the event handlers that are called whenever record properties are changes. This property is used to prevent saving to the database while loading a record.
        /// </summary>
        protected Boolean LoadingRecord { get; set; }

        /// <summary>
        /// Is the type of record a configuration has been created for.
        /// </summary>
        protected Type RecordConfigForType { get; set; }

        #endregion

        #region Constructors / Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public DbRecordView() {
            InitializeComponent();
            InitializeProperties();
        }

        /// <summary>
        /// Initializes basic properties.
        /// </summary>
        protected virtual void InitializeProperties() {
            LoadingRecord = false;
            RecordConfigForType = null;
        }

        #endregion

        #region GUI Support

        /// <summary>
        /// Updates the configuration control with new data after the <see cref="SelectedRecord"/> has changed.
        /// </summary>
        /// <seealso cref="SelectedRecord"/>
        public virtual void RefreshView() {

            LoadingRecord = true;

            try {

                if (SelectedRecord == null) {

                    ConfigControl.Enabled = false;

                } else {

                    LoadRecordConfiguration();

                    ConfigControl.Enabled = true;

                }

            } finally {

                LoadingRecord = false;

            }
        }

        #endregion

        #region Data Handling

        void LoadRecordConfiguration() {
            var t = SelectedRecord.GetType();
            if (t == RecordConfigForType && ConfigControl.Configuration != null) {

                ConfigControl.Configuration.BoundObject = SelectedRecord;

            } else {

                // Create a configuration for the record, if that has not been done previously.
                ConfigControl.Configuration = GenericConfiguration.CreateFor(SelectedRecord);
                RecordConfigForType = t;

                foreach (ConfigurationEntry entry in ConfigControl.Configuration) {
                    entry.PropertyChanged += new PropertyChangedEventHandler(entry_PropertyChanged);
                }

            }
        }

        /// <summary>
        /// Event handler that is called whenever a value changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void entry_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (LoadingRecord)
                return;

            if (e.PropertyName == "Value") {

                // TODO: Do not save here, bust just mark the record as dirty. Then save whenever
                //       another record is selected, the window is closed, etc.

                // Save any changes to the record
                SelectedRecord.Update();
            }
        }

        #endregion

    }
}
