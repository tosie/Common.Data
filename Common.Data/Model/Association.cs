using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO.Compression;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace Common.Data {
    [DebuggerDisplay("Count = {Count}")]
    public class Association<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary
        where TKey : DbRecord, IDbRecord, new()
        where TValue : DbRecord, IDbRecord, new() {

        #region Properties / Class Variables

        /// <summary>
        /// Contains data after it has been loaded. Used by <see cref="Data"/>. Do not use anywhere else!
        /// </summary>
        protected Dictionary<TKey, TValue> RealData;

        /// <summary>
        /// Property that acts as a proxy to the underlying data as it is stored in <see cref="RealData"/>.
        /// </summary>
        protected Dictionary<TKey, TValue> Data {
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

        public Association() {
        }

        #endregion

        #region Serialization / Deserialization

        // Data storage format: "<item1.key.id><tab><item1.value.id><newline><item2.key.id><tab><item2.value.id><newline><item3.key.id><tab><item3.value.id><newline>..."
        //   where <newline> = "\n"
        //     and <tab>     = "\t"

        protected byte[] SerializeData(Dictionary<TKey, TValue> data) {
            // No need to deserialize and then serialize if nothing has been modified
            if (data == null || data.Count <= 0)
                return null;

            // Serialize the data and compress the resulting string
            using (var zipped = new MemoryStream()) {
                using (var s = new GZipStream(zipped, CompressionMode.Compress)) {
                    using (var sw = new StreamWriter(s)) {
                        foreach (KeyValuePair<TKey, TValue> kv in data) {
                            if (kv.Key == null || kv.Value == null)
                                continue;

                            sw.Write(kv.Key.Id);
                            sw.Write("\t");
                            sw.Write(kv.Value.Id);
                            sw.Write("\n");
                        }
                    }
                }

                return zipped.GetBuffer();
            }
        }

        protected Dictionary<TKey, TValue> DeserializeData(byte[] buffer) {
            // If there is nothing to deserialize, create an empty list
            if (buffer == null || buffer.Length <= 0) {
                return new Dictionary<TKey, TValue>();
            }

            // Method to call when reading from the database
            // (this is assumed to be there since T has to be
            // a descendent of DbRecord).
            MethodInfo methodTKey, methodTValue;
            try {
                methodTKey = typeof(TKey).GetMethod(
                    "Read",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new Type[] { typeof(long) },
                    null);

                methodTValue = typeof(TValue).GetMethod(
                    "Read",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new Type[] { typeof(long) },
                    null);
            } catch {
                return new Dictionary<TKey, TValue>();
            }

            // Decompress and deserialize the data
            var result = new Dictionary<TKey, TValue>();
            using (var zipped = new MemoryStream(buffer)) {
                using (var s = new GZipStream(zipped, CompressionMode.Decompress)) {
                    using (var sr = new StreamReader(s)) {
                        while (!sr.EndOfStream) {
                            string[] line = sr.ReadLine().Split('\t');
                            if (line.Length != 2)
                                continue;

                            string skeyid = line[0];
                            string svalueid = line[1];

                            long keyid;
                            long valueid; 

                            if (!long.TryParse(skeyid, out keyid))
                                continue;

                            if (!long.TryParse(svalueid, out valueid))
                                continue;
                            
                            TKey keyrecord = null;
                            TValue valuerecord = null;

                            keyrecord = (TKey)methodTKey.Invoke(null, new object[] { keyid });
                            if (keyrecord == null)
                                continue;

                            valuerecord = (TValue)methodTValue.Invoke(null, new object[] { valueid });
                            if (valuerecord == null)
                                continue;

                            result[keyrecord] = valuerecord;
                        }
                    }
                }
            }

            return result;
        }

        #endregion

        #region IDictionary<TKey,TValue> Members

        public void Add(TKey key, TValue value) {
            Data.Add(key, value);
        }

        public bool ContainsKey(TKey key) {
            return Data.ContainsKey(key);
        }

        public ICollection<TKey> Keys {
            get { return Data.Keys; }
        }

        public bool Remove(TKey key) {
            return Data.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value) {
            return Data.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values {
            get { return Data.Values; }
        }

        public TValue this[TKey key] {
            get {
                return Data[key];
            }
            set {
                Data[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        public void Add(KeyValuePair<TKey, TValue> item) {
            Data.Add(item.Key, item.Value);
        }

        public void Clear() {
            Data.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) {
            return Data.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public int Count {
            get { return Data.Count; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) {
            if (Data.Contains(item))
                return Data.Remove(item.Key);
            else
                return false;
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return Data.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() {
            return Data.GetEnumerator();
        }

        #endregion

        #region IDictionary Members

        public void Add(object key, object value) {
            Data.Add((TKey)key, (TValue)value);
        }

        public bool Contains(object key) {
            return Data.ContainsKey((TKey)key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator() {
            return Data.GetEnumerator();
        }

        public bool IsFixedSize {
            get { return false; }
        }

        ICollection IDictionary.Keys {
            get { return Data.Keys; }
        }

        public void Remove(object key) {
            Data.Remove((TKey)key);
        }

        ICollection IDictionary.Values {
            get { return Data.Values; }
        }

        public object this[object key] {
            get {
                return Data[(TKey)key];
            }
            set {
                Data[(TKey)key] = (TValue)value;
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
