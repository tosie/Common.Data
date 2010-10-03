using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;

namespace Common.Data {
    /// <summary>
    /// Represents a single record in a tree view.
    /// </summary>
    public class RecordTreeNode : TreeNode {

        #region Properties / Class Variables

        /// <summary>
        /// Holds the actual data of <see cref="Record"/>.
        /// </summary>
        protected IEditableDbRecord record = null;

        /// <summary>
        /// Reference to the record this node is associated with. Can only be set by using the constructor.
        /// </summary>
        public IEditableDbRecord Record {
            get {
                return record;
            }

            set {
                if (record == value)
                    return;

                record = value;

                // Reset properties.
                ResetProperties();
            }
        }

        /// <summary>
        /// The class type of the <see cref="Record"/>. Set by <see cref="ResetProperties"/>.
        /// </summary>
        Type recordType = null;

        /// <summary>
        /// The list of all collections of <see cref="Record"/>. Nulled by <see cref="ResetProperties"/> and loaded by <see cref="PrepareCollectionReflection"/>.
        /// </summary>
        List<PropertyInfo> collections = null;

        /// <summary>
        /// Helps <see cref="LoadChildNotes"/> in loading items only once. Set to false by <see cref="ResetProperties"/>.
        /// </summary>
        bool childNodesLoaded = false;

        #endregion

        #region Constructor / Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="record">The record the new node should be associated with.</param>
        public RecordTreeNode(IEditableDbRecord record) {
            this.Record = record;
        }

        #endregion

        #region Data Handling

        /// <summary>
        /// Resets all properties after the <see cref="Record"/> has changed.
        /// </summary>
        protected virtual void ResetProperties() {
            Nodes.Clear();
            collections = null;

            if (record == null) {
                
                recordType = null;
                Text = "<unknown>";

            } else {
                
                recordType = record.GetType();

                // Add a dummy node so that this node can be expanded.
                Nodes.Add("dummy");
            }

            SetNodeProperties();

            childNodesLoaded = false;
        }

        /// <summary>
        /// Sets properties related to the tree view, like the text to be shown.
        /// </summary>
        protected virtual void SetNodeProperties() {
            // Set the node's text.
            if (record == null)
                Text = "<unknown name>";
            else
                Text = record.Name;
        }

        /// <summary>
        /// Initializes the reflection information that is used to load the child nodes.
        /// </summary>
        protected virtual void PrepareCollectionReflection() {
            collections = new List<PropertyInfo>(2);

            PropertyInfo[] properties = recordType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (properties.Length <= 0)
                return;

            for (int i = 0; i < properties.Length; i++) {
                var property = properties[i];
                if (!RecordCollection.IsCollection(property))
                    continue;

                collections.Add(property);
            }

        }

        #endregion

        #region Tree Handling

        /// <summary>
        /// Adds all collections of the <see cref="Record"/> as child notes to this node.
        /// </summary>
        public void LoadChildNotes() {
            // Only do this once.
            if (childNodesLoaded)
                return;

            // Clear any existing child nodes.
            Nodes.Clear();

            // Prepare the reflection information.
            PrepareCollectionReflection();

            // Enumerate all collections ...
            foreach (var property in collections) {
                Nodes.Add(new RecordCollectionTreeNode(record, property));
            }

            childNodesLoaded = true;
        }

        /// <summary>
        /// Updates the tree view with changes made to a record.
        /// </summary>
        public void RecordChanged() {
            if (IsExpanded) {
                // Reload the children.
                ResetProperties();
                LoadChildNotes();
                Expand();
            } else {
                ResetProperties();
            }
        }

        #endregion

    }
}
