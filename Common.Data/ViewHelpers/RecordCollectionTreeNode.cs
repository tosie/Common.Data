using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

namespace Common.Data {
    /// <summary>
    /// Represents the parent of a collection of items in a tree view.
    /// </summary>
    public class RecordCollectionTreeNode : TreeNode {

        #region Properties / Class Variables

        /// <summary>
        /// The record collection that holds all the data.
        /// </summary>
        protected RecordCollection Collection = new RecordCollection();

        /// <summary>
        /// Reference to the record this node is associated with. Can only be set by using the constructor.
        /// </summary>
        public IEditableDbRecord Record {
            get {
                return (IEditableDbRecord)Collection.Record;
            }

            set {
                if (Collection.Record == value)
                    return;

                Collection.Record = value;

                ResetProperties();
            }
        }

        /// <summary>
        /// Holds the actual data for <see cref="Property"/>.
        /// </summary>
        protected PropertyInfo property = null;

        /// <summary>
        /// Property that represents the collection.
        /// </summary>
        public PropertyInfo Property {
            get {
                return property;
            }

            set {
                if (property == value)
                    return;

                property = value;

                Collection.PropertyName = property.Name;

                var text = TreeViewAttribute.GetText(property);

                if (String.IsNullOrEmpty(text))
                    Text = Collection.PropertyName;
                else
                    Text = text;

                ResetProperties();
            }
        }

        /// <summary>
        /// Contains the type of collection <see cref="Property"/> stands for.
        /// </summary>
        public virtual RecordCollection.CollectionType? CollectionType {
            get {
                return Collection.PropertyCollectionType;
            }
        }

        /// <summary>
        /// Holds all records associated with the <see cref="Record"/> via the <see cref="Property"/>, if the property name corresponds with a HasMany&gt;T&lt; collection.
        /// </summary>
        public List<IDbRecord> CollectionList {
            get {
                return Collection.CollectionList;
            }
        }

        /// <summary>
        /// Holds all records associated with the <see cref="Record"/> via the <see cref="Property"/>, if the property name corresponds with an Association&gt;TKey, TValue&lt; collection.
        /// </summary>
        public Dictionary<IDbRecord, IDbRecord> CollectionDictionary {
            get {
                return Collection.CollectionDictionary;
            }
        }

        /// <summary>
        /// Determines whether the elements in the list view can be rearranged by user interaction. Depends on the collection type.
        /// </summary>
        public Boolean AllowReordering {
            get {
                return Collection.AllowReordering;
            }
        }

        /// <summary>
        /// Helps <see cref="RecordCollectionTreeNode.LoadChildNotes"/> in loading items only once. Set to false by <see cref="ResetProperties"/>.
        /// </summary>
        bool childNodesLoaded = false;

        #endregion

        #region Constructor / Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="record">The record the collection belongs to.</param>
        /// <param name="property">The property of the record that represents the collections.</param>
        public RecordCollectionTreeNode(IEditableDbRecord record, PropertyInfo property) {
            this.Record = record;
            this.Property = property;
        }

        #endregion

        #region Data Handling

        /// <summary>
        /// Resets all properties after the <see cref="Record"/> has changed.
        /// </summary>
        protected virtual void ResetProperties() {
            Nodes.Clear();

            // Add a dummy node so that this node can be expanded (only important when it represents a HasMany<T> collection).
            if (Property != null) {
                var collection_type = RecordCollection.GetCollectionType(Property);
                if (collection_type == RecordCollection.CollectionType.HasMany) {
                    Nodes.Add("dummy");
                    childNodesLoaded = false;
                } else {
                    childNodesLoaded = true;
                }
            }
        }

        #endregion

        #region Tree Handling

        /// <summary>
        /// Adds the items of an association collection to the tree view.
        /// </summary>
        protected virtual void AddAssociationNodes() {
            // TODO: Association Collections are not really supported, for now.

            //foreach (KeyValuePair<DbRecord, DbRecord> kv in Collection.CollectionDictionary) {
            //    IEditableDbRecord record = (IEditableDbRecord)kv.Key;
            //    var node = new RecordTreeNode(record);
            //    Nodes.Add(node);
            //}
        }

        /// <summary>
        /// Adds the items of an association collection to the tree view.
        /// </summary>
        protected virtual void AddAssociationWithValueNodes() {
            // TODO: Association Collections are not really supported, for now.
        }

        /// <summary>
        /// Adds the items of a has many collection to the tree view.
        /// </summary>
        protected virtual void AddHasManyNodes() {
            Collection.CollectionList.Sort((a, b) => String.Compare((a as IEditableDbRecord).Name, (b as IEditableDbRecord).Name));

            foreach (IEditableDbRecord record in Collection.CollectionList) {
                var node = new RecordTreeNode(record);
                Nodes.Add(node);
            }
        }

        /// <summary>
        /// Adds all collections of the <see cref="Record"/> as child notes to this node.
        /// </summary>
        public void LoadChildNotes() {
            // Only do this once.
            if (childNodesLoaded)
                return;

            // Clear any existing child nodes.
            Nodes.Clear();

            // Load the collection data.
            Collection.LoadData();
            
            switch (Collection.PropertyCollectionType) {
                case RecordCollection.CollectionType.HasMany:
                    AddHasManyNodes();
                    break;
                case RecordCollection.CollectionType.Association:
                    AddAssociationNodes();
                    break;
                case RecordCollection.CollectionType.AssociationWithValue:
                    AddAssociationWithValueNodes();
                    break;
                default:
                    Trace.Assert(false, "Unknown collection type.");
                    break;
            }

            childNodesLoaded = true;
        }

        #endregion

    }
}
