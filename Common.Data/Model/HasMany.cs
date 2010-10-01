using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Collections;
using System.Diagnostics;

namespace Common.Data {
    [DebuggerDisplay("Count = {Count}")]
    public class HasMany<T> : IList<T>, IList where T : DbRecord, IDbRecord, new() {

        #region Properties / Class Variables

        /// <summary>
        /// Contains data after it has been loaded. Used by <see cref="Data"/>. Do not use anywhere else!
        /// </summary>
        protected List<T> RealData;

        /// <summary>
        /// Property that acts as a proxy to the underlying data as it is stored in <see cref="RealData"/>.
        /// </summary>
        protected List<T> Data {
            get {
                if (RealBinaryData != null || RealData == null) {
                    RealData = DeserializeData(RealBinaryData);
                    RealBinaryData = null;
                }

                return RealData;
            }

            set {
                RealData = value;
            }
        }

        protected byte[] RealBinaryData;

        public byte[] BinaryData {
            get {
                // TODO: Maybe use RealData here, but what happens if the
                //       RealBinaryData is assigned to and there was no
                //       call to +DeserializeData+ before?
                return SerializeData(Data);
            }

            set {
                RealBinaryData = value;
                RealData = null;
            }
        }

        #endregion

        #region Constructor

        public HasMany() {
        }

        #endregion

        #region Serialization / Deserialization

        // Data storage format: "<item1.id><newline><item2.id><newline><item3.id><newline>...<newline><itemn.id>"
        //   where <newline> = "\n"

        protected byte[] SerializeData(List<T> data) {
            // No need to deserialize and then serialize if nothing has been modified
            if (data == null || data.Count <= 0)
                return null;

            // Serialize the data and compress the resulting string
            using (var zipped = new MemoryStream()) {
                using (var s = new GZipStream(zipped, CompressionMode.Compress)) {
                    using (var sw = new StreamWriter(s)) {
                        for (int i = 0; i < data.Count; i++) {
                            sw.Write(data[i].Id);
                            sw.Write("\n");
                        }
                    }
                }

                return zipped.GetBuffer();
            }
        }

        protected List<T> DeserializeData(byte[] buffer) {
            // If there is nothing to deserialize, create an empty list
            if (buffer == null || buffer.Length <= 0) {
                return new List<T>();
            }

            // Method to call when reading from the database
            // (this is assumed to be there since T has to be
            // a descendent of DbRecord).
            MethodInfo method;
            try {
                method = typeof(T).GetMethod(
                    "Read",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new Type[] { typeof(long) },
                    null);
            } catch {
                return new List<T>();
            }

            // Decompress and deserialize the data
            var result = new List<T>();
            using (var zipped = new MemoryStream(buffer)) {
                using (var s = new GZipStream(zipped, CompressionMode.Decompress)) {
                    using (var sr = new StreamReader(s)) {
                        while (!sr.EndOfStream) {
                            string sid = sr.ReadLine();
                            long id;
                            if (long.TryParse(sid, out id)) {
                                T record = (T)method.Invoke(null, new object[] { id });
                                if (record != null)
                                    result.Add(record);
                            }
                        }
                    }
                }
            }

            return result;
        }

        #endregion

        #region Sorting Methods

        public void Sort() {
            Data.Sort();
        }

        public void Sort(Comparison<T> comparison) {
            Data.Sort(comparison);
        }

        public void Sort(IComparer<T> comparer) {
            Data.Sort(comparer);
        }

        public void Sort(int index, int count, IComparer<T> comparer) {
            Data.Sort(index, count, comparer);
        }

        #endregion

        #region List Members

        /// <summary>
        /// Performs the specified action on each element of the System.Collections.Generic.List<T>.
        /// </summary>
        /// <param name="action">The System.Action<T> delegate to perform on each element of the System.Collections.Generic.List<T>.</param>
        public void ForEach(Action<T> action) {
            Data.ForEach(action);
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the HasMany&lt;T&gt; collection.
        /// </summary>
        /// <param name="collection">The collections whose elements should be added to the end of the HasMany&lt;T&gt; collection. The collection itself cannot be null, but it can contain elements that are null, if T is a reference type.</param>
        public void AddRange(IEnumerable<T> collection) {
            Data.AddRange(collection);
        }

        #endregion

        #region IList<T> Members

        public int IndexOf(T item) {
            return Data.IndexOf(item);
        }

        public void Insert(int index, T item) {
            Data.Insert(index, item);
        }

        public void RemoveAt(int index) {
            Data.RemoveAt(index);
        }

        public T this[int index] {
            get {
                return Data[index];
            }
            set {
                Data[index] = value;
            }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item) {
            Data.Add(item);
        }

        public void Clear() {
            Data.Clear();
        }

        public bool Contains(T item) {
            return Data.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            Data.CopyTo(array, arrayIndex);
        }

        public int Count {
            get { return Data.Count; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        public bool Remove(T item) {
            return Data.Remove(item);
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator() {
            return Data.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return Data.GetEnumerator();
        }

        #endregion

        #region IList Members

        public int Add(object value) {
            Add((T)value);
            return Count - 1;
        }

        public bool Contains(object value) {
            return Contains((T)value);
        }

        public int IndexOf(object value) {
            return IndexOf((T)value);
        }

        public void Insert(int index, object value) {
            Insert(index, (T)value);
        }

        public bool IsFixedSize {
            get { return false; }
        }

        public void Remove(object value) {
            Remove((T)value);
        }

        object IList.this[int index] {
            get {
                return Data[index];
            }
            set {
                this[index] = (T)value;
            }
        }

        #endregion

        #region ICollection Members

        /// <summary>Not implemented.</summary>
        /// <exception cref="System.NotImplementedException">Thrown always.</exception>
        public void CopyTo(Array array, int index) {
            throw new NotImplementedException();
        }

        /// <summary>Not implemented.</summary>
        /// <exception cref="System.NotImplementedException">Thrown always.</exception>
        public bool IsSynchronized {
            get { throw new NotImplementedException(); }
        }

        /// <summary>Not implemented.</summary>
        /// <exception cref="System.NotImplementedException">Thrown always.</exception>
        public object SyncRoot {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
