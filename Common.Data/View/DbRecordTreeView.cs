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

namespace Common.Data {
    /// <summary>
    /// Represents a tree view of one or more IEditableDbRecord instances including their collections.
    /// </summary>
    public partial class DbRecordTreeView : UserControl {

        #region Properties / Class Variables

        /// <summary>
        /// Actual contents of <see cref="RootRecords"/>.
        /// </summary>
        protected List<IEditableDbRecord> rootRecords = null;

        // TODO: React to changes in the list.
        /// <summary>
        /// Holds the list of all base records.
        /// </summary>
        [Browsable(false)]
        public List<IEditableDbRecord> RootRecords {
            get {
                return rootRecords;
            }

            set {
                if (rootRecords == value)
                    return;

                rootRecords = value;
                RefreshView();
            }
        }

        /// <summary>
        /// Holds the reference to the selected record (<seealso cref="SelectedRecord"/>).
        /// </summary>
        protected IEditableDbRecord selectedRecord = null;

        /// <summary>
        /// Gets the record that is currently selected in the tree view. If a collection's root node is selected, this returns the record that is owner of the collection.
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

        /// <summary>
        /// Holds the reference to the selected collection (<seealso cref="SelectedCollection"/>).
        /// </summary>
        protected PropertyInfo selectedCollection = null;

        /// <summary>
        /// Gets the currently selected collection of a record (if the collection's root node is selected).
        /// </summary>
        [Browsable(false)]
        public PropertyInfo SelectedCollection {
            get {
                return selectedCollection;
            }

            protected set {
                if (selectedCollection == value)
                    return;

                selectedCollection = value;

                // Notify registered event handlers of the selection change.
                CollectionEvent.Fire(this, CollectionSelected, selectedRecord, selectedCollection);
            }
        }

        /// <summary>
        /// Returns true if a single record is selected and not a collection's root node.
        /// </summary>
        public bool IsRecordSelected {
            get {
                return SelectedRecord != null && SelectedCollection == null;
            }
        }

        /// <summary>
        /// Returns true if a collection's root node is selected.
        /// </summary>
        public bool IsCollectionSelected {
            get {
                return SelectedRecord != null && SelectedCollection != null;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Fired when a record is selected and when a selected record is deselected.
        /// </summary>
        public event RecordEventHandler RecordSelected;

        /// <summary>
        /// Fired when a collection's root node is selected or deselected.
        /// </summary>
        public event CollectionEventHandler CollectionSelected;

        #endregion

        #region Constructor / Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public DbRecordTreeView() {
            InitializeComponent();
            ToolStrip.Renderer = new NoBorderToolStripRenderer();

            InitializePropertes();
        }

        /// <summary>
        /// Basic initialization of the properties. Called by the constructor.
        /// </summary>
        protected virtual void InitializePropertes() {
            RootRecords = new List<IEditableDbRecord>();
        }

        #endregion

        #region Data Handling

        /// <summary>
        /// Adds all root records to the tree view.
        /// </summary>
        protected virtual void AddRecordsToTreeView() {

            if (rootRecords == null)
                return;

            foreach (IEditableDbRecord record in rootRecords) {
                var node = new RecordTreeNode(record);
                Tree.Nodes.Add(node);
            }

        }

        /// <summary>
        /// Updates the tree view with changes made to a record.
        /// </summary>
        /// <param name="record">The record that has changed.</param>
        public virtual void RecordChanged(IEditableDbRecord record) {
            var key = record.Name;

            if (String.IsNullOrEmpty(key))
                return;

            // Find the node of the record.
            var nodes = Tree.Nodes.Find(key, true);
            if (nodes.Length <= 0)
                return;

            // Walk through all nodes that matched the key we looked for.
            for (int i = 0; i < nodes.Length; i++) {
                var node = (RecordTreeNode)nodes[i];

                // Update nodes whose record matches the one that has been changed.
                if (node.Record != record)
                    continue;

                node.RecordChanged();
            }
        }

        #endregion

        #region GUI Support

        /// <summary>
        /// Disables tree view redrawing. Call this before updating many records and call <see cref="EndUpdate"/> afterwards.
        /// </summary>
        public void BeginUpdate() {
            Tree.BeginUpdate();
        }

        /// <summary>
        /// Re-enables tree view redrawing after a call to <see cref="BeginUpdate"/>. Call this after updating many records using the <see cref="RecordChanged"/> method.
        /// </summary>
        public void EndUpdate() {
            Tree.EndUpdate();
        }

        /// <summary>
        /// Updates the tree view with new data after the <see cref="RootRecords"/> have changed.
        /// </summary>
        public void RefreshView() {
            Tree.BeginUpdate();

            try {

                Tree.Nodes.Clear();

                // Show records in the tree view.
                AddRecordsToTreeView();

            } finally {

                Tree.EndUpdate();

            }
        }

        #endregion

        #region GUI Events

        private void Tree_BeforeExpand(object sender, TreeViewCancelEventArgs e) {
            if (e.Node is RecordTreeNode) {
                var recordnode = (RecordTreeNode)e.Node;
                recordnode.LoadChildNotes();
            } else if (e.Node is RecordCollectionTreeNode) {
                var recordnode = (RecordCollectionTreeNode)e.Node;
                recordnode.LoadChildNotes();
            }
        }

        private void Tree_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e) {
            e.CancelEdit = true;

            if (e.Node is RecordTreeNode) {
                var recordnode = (RecordTreeNode)e.Node;
                if (recordnode.Record is IEditableDbRecord)
                    e.CancelEdit = false;
            }
        }

        private void Tree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e) {
            e.CancelEdit = true;
            
            if (e.Node is RecordTreeNode) {
                var recordnode = (RecordTreeNode)e.Node;
                
                if (recordnode.Record is IEditableDbRecord) {
                    e.CancelEdit = false;

                    var record = (recordnode.Record as IEditableDbRecord);
                    record.Name = e.Label;
                    record.Update();
                }
            }
        }

        private void Tree_AfterSelect(object sender, TreeViewEventArgs e) {
            if (e.Node is RecordTreeNode) {
                var recordnode = (RecordTreeNode)e.Node;
                SelectedCollection = null;
                SelectedRecord = (recordnode == null ? null : recordnode.Record);
            } else if (e.Node is RecordCollectionTreeNode) {
                var recordnode = (RecordCollectionTreeNode)e.Node;
                SelectedRecord = (recordnode == null ? null : recordnode.Record);
                SelectedCollection = (recordnode == null ? null : recordnode.Property);
            }
        }

        #endregion

    }
}
