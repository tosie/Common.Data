using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections;

namespace Common.Data {
    /// <summary>
    /// Represents a collection of IDbRecords as specified by an instance of a record and a property name that matches the name of one of the record's properties which in turn represents a collection.
    /// </summary>
    public class RecordCollection {

        #region Data Structures

        /// <summary>Enumeration of all supported collection types.</summary>
        public enum CollectionType {
            /// <summary>The collection is of the HasMany&lt;T&gt; type.</summary>
            HasMany,
            
            /// <summary>The collection is of the Association&lt;TKey, TValue&gt; type.</summary>
            Association,

            /// <summary>The collection is of the AssociationWithValue&lt;TFrom, TTo&gt; type.</summary>
            AssociationWithValue
        }

        /// <summary>
        /// Dictionary to convert from a full namespace to a collection type.
        /// </summary>
        internal static readonly Dictionary<String, CollectionType> CollectionTypeNames = new Dictionary<String, CollectionType>() {
            { "Common.Data.HasMany`1", CollectionType.HasMany },
            { "Common.Data.Association`2", CollectionType.Association },
            { "Common.Data.AssociationWithValue`2", CollectionType.AssociationWithValue }
        };

        /// <summary>
        /// Dictionary to validate the number of generc arguments for a given collection type.
        /// </summary>
        internal static readonly Dictionary<CollectionType, Int32> CollectionTypeArguments = new Dictionary<CollectionType, Int32>() {
            { CollectionType.HasMany, 1 },
            { CollectionType.Association, 2 },
            { CollectionType.AssociationWithValue, 2 }
        };

        #endregion

        #region Properties / Class Variables

        /// <summary>
        /// Reference to the currently selected record. <seealso cref="Record"/>.
        /// </summary>
        protected IDbRecord record = null;

        /// <summary>
        /// Gets or sets the record that is currently selected and thus whose collection should be shown.
        /// </summary>
        public IDbRecord Record {
            get {
                return record;
            }

            set {
                if (record == value)
                    return;

                record = value;
            }
        }

        /// <summary>
        /// Name of the collection relation. Must correspond to a property name of the owning record that is of type Association&lt;TKey, TValue&gt; or HasMany&lt;T&gt;. Must be set before calling <see cref="LoadData"/>.
        /// </summary>
        public String PropertyName { get; set; }

        /// <summary>
        /// The property as specified by PropertyName.
        /// </summary>
        /// <see cref="InitializePropertyReflection"/>
        public virtual PropertyInfo Property { get; protected set; }

        /// <summary>
        /// Contains the type of collection <see cref="PropertyName"/> stands for. Set by <see cref="InitializePropertyReflection"/>.
        /// </summary>
        public virtual CollectionType? PropertyCollectionType { get; protected set; }

        /// <summary>
        /// If the type of the property is HasMany&lt;T&gt;, this contains the type of T.
        /// If the property is an Association&lt;TKey, TValue&gt; or AssociationWithValue&lt;TFrom, TTo&gt;, this contains the type of TKey.
        /// </summary>
        /// <see cref="InitializePropertyReflection"/>
        public virtual Type KeyType { get; protected set; }

        /// <summary>
        /// If the type of the property is HasMany&lt;T&gt;, this is null.
        /// If the property is an Association&lt;TKey, TValue&gt; or AssociationWithValue&lt;TFrom, TTo&gt;, this contains the type of TValue.
        /// </summary>
        /// <see cref="InitializePropertyReflection"/>
        public virtual Type ValueType { get; protected set; }

        /// <summary>
        /// Holds all records associated with the <see cref="Record"/> via the <see cref="PropertyName"/>, if the property name corresponds with a HasMany&gt;T&lt; collection.
        /// </summary>
        public List<IDbRecord> CollectionList { get; protected set; }

        /// <summary>
        /// Holds all records associated with the <see cref="Record"/> via the <see cref="PropertyName"/>, if the property name corresponds with an Association&gt;TKey, TValue&lt; collection.
        /// </summary>
        public Dictionary<IDbRecord, IDbRecord> CollectionDictionary { get; protected set; }

        /// <summary>
        /// Holds all records associated with the <see cref="Record"/> via the <see cref="PropertyName"/>, if the property name corresponds with an AssociationWithValue&gt;TKey, TValue&lt; collection.
        /// </summary>
        public Dictionary<IDbRecord, Dictionary<IDbRecord, string>> CollectionDictionaryWithValues { get; protected set; }

        /// <summary>
        /// Determines whether the elements in the collections can be rearranged. Depends on <see cref="PropertyCollectionType"/>.
        /// </summary>
        public Boolean AllowReordering {
            get {
                return PropertyCollectionType == CollectionType.HasMany;
            }
        }

        #endregion

        #region Constructors / Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public RecordCollection() {
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Checks if the given property represents a collection (HasMany&lt;T&gt; or Association&lt;TKey, TValue&gt;) or not.
        /// </summary>
        /// <param name="property">The property to check.</param>
        /// <returns>Returns true if the property is a collection, false otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the input parameter is null.</exception>
        static public bool IsCollection(PropertyInfo property) {
            if (property == null)
                throw new ArgumentNullException("property");

            string compiled_name = property.PropertyType.Namespace + "." + property.PropertyType.Name;
            return CollectionTypeNames.ContainsKey(compiled_name);
        }

        /// <summary>
        /// Returns the collectio type of a given property.
        /// </summary>
        /// <param name="property">The property to check.</param>
        /// <returns>The type of collection.</returns>
        /// <exception cref="System.ArgumentException">Thrown when the property's type does not correspond to one of the known collection types.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when the input parameter is null.</exception>
        static public CollectionType GetCollectionType(PropertyInfo property) {
            if (property == null)
                throw new ArgumentNullException("property");

            string compiled_name = property.PropertyType.Namespace + "." + property.PropertyType.Name;
            if (CollectionTypeNames.ContainsKey(compiled_name))
                return CollectionTypeNames[compiled_name];
            else
                throw new ArgumentException(String.Format("The property \"{0}\" does not correspond to a known collection type.", property.Name), "property");
        }

        #endregion

        #region Data Handling (Collection)

        /// <summary>
        /// Resets the properties their default values.
        /// </summary>
        protected virtual void ResetProperties() {
            Property = null;
            PropertyCollectionType = null;
            KeyType = null;
            ValueType = null;
            CollectionList = null;
            CollectionDictionary = null;
            CollectionDictionaryWithValues = null;
        }

        /// <summary>
        /// Makes some property and type checking and initializes <see cref="Property"/> and <see cref="KeyType"/>.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when <see cref="Record"/> or <see cref="PropertyName"/> are not set.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the property defined by <see cref="PropertyName"/> is not found or does not correspond to either the Association&lt;TKey, TValue&gt; or the HasMany&lt;T&gt; type.</exception>
        protected virtual void InitializePropertyReflection() {
            if (Record == null)
                throw new ArgumentNullException("SelectedRecord", "SelectedRecord must be set.");

            if (String.IsNullOrEmpty(PropertyName))
                throw new ArgumentNullException("PropertyName", "PropertyName must be set.");

            Property = Record.GetType().GetProperty(PropertyName);
            if (Property == null)
                throw new ArgumentException(String.Format("Could not find a property by the name of \"{0}\".", PropertyName), "PropertyName");

            string compiled_name = Property.PropertyType.Namespace + "." + Property.PropertyType.Name;
            if (!CollectionTypeNames.ContainsKey(compiled_name))
                throw new ArgumentException("The given property is not supported.", "PropertyName");

            PropertyCollectionType = CollectionTypeNames[compiled_name];

            Type[] arguments = Property.PropertyType.GetGenericArguments();
            int expected_argument_count = CollectionTypeArguments[PropertyCollectionType.Value];
            Trace.Assert(arguments.Length == expected_argument_count, "Argument count does not match expected argument count.");

            KeyType = arguments[0];
            ValueType = arguments.Length > 1 ? arguments[1] : null;
        }

        /// <summary>
        /// Updates the abstract list (or dictionary) with new data after the <see cref="Record"/> or <see cref="PropertyName"/> have changed.
        /// </summary>
        /// <see cref="Record"/>
        /// <see cref="PropertyName"/>
        public virtual void LoadData() {
            // Reset basic properties before initializing the property reflection.
            ResetProperties();

            // Make sure the properties are set.
            if (Record == null || String.IsNullOrEmpty(PropertyName))
                return;

            // Analyze the SelectedRecord and PropertyName.
            InitializePropertyReflection();

            // Get a list of all records that are contained in the collection.
            switch (PropertyCollectionType) {
                case CollectionType.HasMany:
                    LoadListCollectionData();
                    break;
                case CollectionType.Association:
                    LoadDictionaryCollectionData();
                    break;
                case CollectionType.AssociationWithValue:
                    LoadDictionaryCollectionWithValuesData();
                    break;
                default:
                    Trace.Assert(false, "Unknown collection type.");
                    break;
            }
                
        }

        /// <summary>
        /// Writes the abstract list (or dictionary) back to the original collection as set by <see cref="Record"/> and <see cref="PropertyName"/>.
        /// </summary>
        public virtual void SaveData() {
            // Make sure the properties are set.
            if (Record == null || String.IsNullOrEmpty(PropertyName))
                return;

            // Save the orignal collections ...
            switch (PropertyCollectionType) {
                case CollectionType.HasMany:
                    SaveListCollectionData();
                    break;
                case CollectionType.Association:
                    SaveDictionaryCollectionData();
                    break;
                case CollectionType.AssociationWithValue:
                    SaveDictionaryCollectionWithValuesData();
                    break;
                default:
                    Trace.Assert(false, "Unknown collection type.");
                    break;
            }
        }

        #endregion

        #region Data Handling (Collection: HasMany<T>)

        /// <summary>
        /// Converts an anonymous object that implements the IList interface to a list of IDbRecords.
        /// </summary>
        /// <param name="Obj">The anonymous object</param>
        /// <param name="List">The list to which to add the items</param>
        /// <exception cref="System.ArgumentNullException">Thrown when one of the input parameters is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the passed object does not implement the IList interface.</exception>
        /// <exception cref="System.InvalidCastException">Thrown when the list of items passed as anonymous object contains elements that cannot be cast to the IDbRecord interface.</exception>
        protected virtual void AnonymousObjectToDbRecordList(object Obj, IList<IDbRecord> List) {
            if (Obj == null)
                throw new ArgumentNullException("Obj");

            if (List == null)
                throw new ArgumentNullException("List");

            if (!(Obj is IList))
                throw new ArgumentException("The given object is not of an IList type.", "Obj");

            int count = ((IList)Obj).Count;

            List.Clear();

            for (int i = 0; i < count; i++) {
                object item = ((IList)Obj)[i];
                List.Add(item as IDbRecord);
            }
        }

        /// <summary>
        /// Retrieves a list of all records in the HasMany&lt;T&gt; collection as specified by <see cref="CollectionList"/> and <see cref="PropertyName"/>. The result is stored in the <see cref="CollectionList"/> property.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when Property or KeyType are null (see <see cref="ResetProperties"/> and <see cref="InitializePropertyReflection"/>.)</exception>
        /// <exception cref="System.MemberAccessException">Thrown when the KeyType does not contain a static method named "Read".</exception>
        /// <exception cref="System.InvalidCastException">Thrown when the list of items contained in the HasMany set (see <see cref="Record"/> and <see cref="PropertyName"/>) contains elements that cannot be cast to the DbRecord class.</exception>
        protected virtual void LoadListCollectionData() {
            if (Property == null)
                throw new ArgumentNullException("Property");

            if (KeyType == null)
                throw new ArgumentNullException("KeyType");

            CollectionList = new List<IDbRecord>();

            // Invoke the property's get method
            object list = null;
            try {
                list = Property.GetValue(Record, null);
            } catch (Exception ex) {
                throw new MemberAccessException("Could not get the HasMany set.", ex);
            }

            if (list == null)
                return;

            AnonymousObjectToDbRecordList(list, CollectionList);
        }

        /// <summary>
        /// Saves the list of all records in the HasMany&lt;T&gt; collection specified by <see cref="Record"/> and <see cref="PropertyName"/>. The source is <see cref="CollectionList"/>.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when Property or KeyType are null (see <see cref="ResetProperties"/> and <see cref="InitializePropertyReflection"/>.)</exception>
        /// <exception cref="System.MemberAccessException">Thrown when the KeyType does not contain a static method named "Read".</exception>
        /// <exception cref="System.ArgumentException">Thrown when the Property's type does not implement the IList interface.</exception>
        protected virtual void SaveListCollectionData() {
            if (Property == null)
                throw new ArgumentNullException("Property");

            if (KeyType == null)
                throw new ArgumentNullException("KeyType");

            // Invoke the property's get method
            object list = null;
            try {
                list = Property.GetValue(Record, null);
            } catch (Exception ex) {
                throw new MemberAccessException("Could not get the HasMany set.", ex);
            }

            if (list == null)
                return;

            if (!(list is IList))
                throw new ArgumentException("The given property is not of an IList type.", "Property");

            (list as IList).Clear();
            for (int i = 0; i < CollectionList.Count; i++) {
                (list as IList).Add(CollectionList[i]);
            }
        }

        #endregion

        #region Data Handling (Collection: Association<TKey, TValue>)

        /// <summary>
        /// Converts an anonymous object that implements the IDictionary interface to a dictionary of IDbRecords.
        /// </summary>
        /// <param name="Obj">The anonymous object</param>
        /// <param name="Dictionary">The dictionary to which to add the items</param>
        /// <exception cref="System.ArgumentNullException">Thrown when one of the input parameters is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the passed object does not implement the IDictionary interface.</exception>
        /// <exception cref="System.InvalidCastException">Thrown when the dictionary of items passed as anonymous object contains elements that cannot be cast to the IDbRecord interface.</exception>
        protected virtual void AnonymousObjectToDbRecordDict(object Obj, Dictionary<IDbRecord, IDbRecord> Dictionary) {
            if (Obj == null)
                throw new ArgumentNullException("Obj");

            if (Dictionary == null)
                throw new ArgumentNullException("Dictionary");

            if (!(Obj is IDictionary))
                throw new ArgumentException("The given object is not of an IDictionary type.", "Obj");

            Dictionary.Clear();

            foreach (var key in ((IDictionary)Obj).Keys) {
                Dictionary[key as IDbRecord] = ((IDictionary)Obj)[key] as IDbRecord;
            }
        }

        /// <summary>
        /// Retrieves a list of all records in the Association&lt;TKey, TValue&gt; collection as specified by <see cref="CollectionList"/> and <see cref="PropertyName"/>. The result is stored in the <see cref="CollectionList"/> property.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when Property, KeyType or ValueType are null (see <see cref="ResetProperties"/> and <see cref="InitializePropertyReflection"/>.)</exception>
        /// <exception cref="System.MemberAccessException">Thrown when neither the KeyType nor the ValueType contain a static method named "Read".</exception>
        /// <exception cref="System.InvalidCastException">Thrown when the dictionary of items contained in the Association set (see <see cref="Record"/> and <see cref="PropertyName"/>) contains elements that cannot be cast to the DbRecord class.</exception>
        protected virtual void LoadDictionaryCollectionData() {
            if (Property == null)
                throw new ArgumentNullException("Property");

            if (KeyType == null)
                throw new ArgumentNullException("KeyType");

            if (ValueType == null)
                throw new ArgumentNullException("ValueType");

            CollectionDictionary = new Dictionary<IDbRecord, IDbRecord>();

            // Invoke the property's get method
            object dict = null;
            try {
                dict = Property.GetValue(Record, null);
            } catch (Exception ex) {
                throw new MemberAccessException("Could not get the Association set.", ex);
            }

            if (dict == null)
                return;

            AnonymousObjectToDbRecordDict(dict, CollectionDictionary);
        }

        /// <summary>
        /// Saves all associations back to the <see cref="Record"/>'s property names <see cref="PropertyName"/>. The source is <see cref="CollectionDictionary"/>.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when Property, KeyType or ValueType are null (see <see cref="ResetProperties"/> and <see cref="InitializePropertyReflection"/>.)</exception>
        /// <exception cref="System.MemberAccessException">Thrown when neither the KeyType nor the ValueType contain a static method named "Read".</exception>
        /// <exception cref="System.ArgumentException">Thrown when the Property's type does not implement the IDictionary interface.</exception>
        protected virtual void SaveDictionaryCollectionData() {
            if (Property == null)
                throw new ArgumentNullException("Property");

            if (KeyType == null)
                throw new ArgumentNullException("KeyType");

            if (ValueType == null)
                throw new ArgumentNullException("ValueType");

            // Invoke the property's get method
            object dictionary = null;
            try {
                dictionary = Property.GetValue(Record, null);
            } catch (Exception ex) {
                throw new MemberAccessException("Could not get the Association relation.", ex);
            }

            if (dictionary == null)
                return;

            if (!(dictionary is IDictionary))
                throw new ArgumentException("The given property is not of an IDictionary type.", "Property");

            (dictionary as IDictionary).Clear();
            foreach (KeyValuePair<IDbRecord, IDbRecord> kv in CollectionDictionary) {
                (dictionary as IDictionary).Add(kv.Key, kv.Value);
            }
        }

        #endregion

        #region Data Handling (Collection: AssociationWithValue<TFrom, TTo>)

        /// <summary>
        /// Converts an anonymous object that implements the IDictionary interface to a dictionary of IDbRecords with values.
        /// </summary>
        /// <param name="Obj">The anonymous object</param>
        /// <param name="Dictionary">The dictionary to which to add the items</param>
        /// <exception cref="System.ArgumentNullException">Thrown when one of the input parameters is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the passed object does not implement the IDictionary interface.</exception>
        /// <exception cref="System.InvalidCastException">Thrown when the dictionary of items passed as anonymous object contains elements that cannot be cast to the IDbRecord interface.</exception>
        protected virtual void AnonymousObjectToDbRecordDict(object Obj, Dictionary<IDbRecord, Dictionary<IDbRecord, string>> Dictionary) {
            if (Obj == null)
                throw new ArgumentNullException("Obj");

            if (Dictionary == null)
                throw new ArgumentNullException("Dictionary");

            if (!(Obj is IDictionary))
                throw new ArgumentException("The given object is not of an IDictionary<IDbRecord, IDictionary<IDbRecord, string>> type.", "Obj");

            Dictionary.Clear();

            foreach (var from in ((IDictionary)Obj).Keys) {
                if (!Dictionary.ContainsKey(from as IDbRecord))
                    Dictionary[from as IDbRecord] = new Dictionary<IDbRecord, string>();

                if (!(((IDictionary)Obj)[from] is IDictionary))
                    throw new ArgumentException("The given object is not of an IDictionary<IDbRecord, IDictionary<IDbRecord, string>> type.", "Obj");

                var from_dict = (IDictionary)((IDictionary)Obj)[from];
                foreach (DictionaryEntry kv in from_dict) {
                    var to = kv.Key;
                    var value = kv.Value;
                    Dictionary[from as IDbRecord][to as IDbRecord] = value as string;
                }
            }
        }

        /// <summary>
        /// Retrieves a list of all records in the AssociationWithValue&lt;TFrom, TTo&gt; collection as specified by <see cref="CollectionList"/> and <see cref="PropertyName"/>. The result is stored in the <see cref="CollectionList"/> property.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when Property, KeyType or ValueType are null (see <see cref="ResetProperties"/> and <see cref="InitializePropertyReflection"/>.)</exception>
        /// <exception cref="System.MemberAccessException">Thrown when neither the KeyType nor the ValueType contain a static method named "Read".</exception>
        /// <exception cref="System.InvalidCastException">Thrown when the dictionary of items contained in the Association set (see <see cref="Record"/> and <see cref="PropertyName"/>) contains elements that cannot be cast to the DbRecord class.</exception>
        protected virtual void LoadDictionaryCollectionWithValuesData() {
            if (Property == null)
                throw new ArgumentNullException("Property");

            if (KeyType == null)
                throw new ArgumentNullException("KeyType");

            if (ValueType == null)
                throw new ArgumentNullException("ValueType");

            CollectionDictionaryWithValues = new Dictionary<IDbRecord, Dictionary<IDbRecord, string>>();

            // Invoke the property's get method
            object dict = null;
            try {
                dict = Property.GetValue(Record, null);
            } catch (Exception ex) {
                throw new MemberAccessException("Could not get the Association set.", ex);
            }

            if (dict == null)
                return;

            AnonymousObjectToDbRecordDict(dict, CollectionDictionaryWithValues);
        }

        /// <summary>
        /// Saves all associations with values back to the <see cref="Record"/>'s property names <see cref="PropertyName"/>. The source is <see cref="CollectionDictionaryWithValues"/>.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when Property, KeyType or ValueType are null (see <see cref="ResetProperties"/> and <see cref="InitializePropertyReflection"/>.)</exception>
        /// <exception cref="System.MemberAccessException">Thrown when neither the KeyType nor the ValueType contain a static method named "Read".</exception>
        /// <exception cref="System.ArgumentException">Thrown when the Property's type does not implement the IDictionary interface.</exception>
        protected virtual void SaveDictionaryCollectionWithValuesData() {
            if (Property == null)
                throw new ArgumentNullException("Property");

            if (KeyType == null)
                throw new ArgumentNullException("KeyType");

            if (ValueType == null)
                throw new ArgumentNullException("ValueType");

            // Invoke the property's get method
            object from_dict = null;
            try {
                from_dict = Property.GetValue(Record, null);
            } catch (Exception ex) {
                throw new MemberAccessException("Could not get the Association relation.", ex);
            }

            if (from_dict == null)
                return;

            if (!(from_dict is IDictionary))
                throw new ArgumentException("The given property is not of an IDictionary type.", "Property");

            foreach (KeyValuePair<IDbRecord, Dictionary<IDbRecord, string>> kv in CollectionDictionaryWithValues) {
                var from = kv.Key;

                var to_dict = (from_dict as IDictionary)[from];

                foreach (KeyValuePair<IDbRecord, string> kv2 in CollectionDictionaryWithValues[from]) {
                    var to = kv2.Key;
                    var value = kv2.Value;

                    (to_dict as IDictionary)[to] = value;
                }
            }
        }

        #endregion

    }
}
