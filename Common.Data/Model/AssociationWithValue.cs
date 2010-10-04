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
    public class AssociationWithValue<TFrom, TTo> : IDictionary<TFrom, Dictionary<TTo, String>>, IDictionary
        where TFrom : DbRecord, IDbRecord, new()
        where TTo : DbRecord, IDbRecord, new() {

        #region Properties / Class Variables

        /// <summary>
        /// Contains data after it has been loaded. Used by <see cref="Data"/>. Do not use anywhere else!
        /// </summary>
        protected Dictionary<TFrom, Dictionary<TTo, String>> RealData;

        /// <summary>
        /// Property that acts as a proxy to the underlying data as it is stored in <see cref="RealData"/>.
        /// </summary>
        protected Dictionary<TFrom, Dictionary<TTo, String>> Data {
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

        public AssociationWithValue() {
        }

        #endregion

        #region Serialization / Deserialization

        // Data storage format: "<item1.key.id><tab><item1.value.key.id><tab><item1.value.value><newline>..."
        //   where <newline> = "\n"
        //     and <tab>     = "\t"

        protected byte[] SerializeData(Dictionary<TFrom, Dictionary<TTo, String>> data) {
            // No need to deserialize and then serialize if nothing has been modified
            if (data == null || data.Count <= 0)
                return null;

            // Serialize the data and compress the resulting string
            using (var zipped = new MemoryStream()) {
                using (var s = new GZipStream(zipped, CompressionMode.Compress)) {
                    using (var sw = new StreamWriter(s)) {
                        foreach (KeyValuePair<TFrom, Dictionary<TTo, String>> kv in data) {
                            if (kv.Key == null || kv.Value == null || kv.Value.Count <= 0)
                                continue;

                            foreach (KeyValuePair<TTo, String> kv2 in kv.Value) {
                                if (kv.Value == null)
                                    continue;

                                sw.Write(kv.Key.Id);
                                sw.Write("\t");
                                sw.Write(kv2.Key.Id);
                                sw.Write("\t");
                                sw.Write(kv2.Value.ToString());
                                sw.Write("\n");
                            }
                        }
                    }
                }

                return zipped.GetBuffer();
            }
        }

        protected Dictionary<TFrom, Dictionary<TTo, String>> DeserializeData(byte[] buffer) {
            // If there is nothing to deserialize, create an empty list
            if (buffer == null || buffer.Length <= 0) {
                return new Dictionary<TFrom, Dictionary<TTo, String>>();
            }

            // Method to call when reading from the database
            // (this is assumed to be there since T has to be
            // a descendent of DbRecord).
            MethodInfo methodTFrom, methodTTo;
            try {
                methodTFrom = typeof(TFrom).GetMethod(
                    "Read",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new Type[] { typeof(long) },
                    null);

                methodTTo = typeof(TTo).GetMethod(
                    "Read",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new Type[] { typeof(long) },
                    null);
            } catch {
                return new Dictionary<TFrom, Dictionary<TTo, String>>();
            }

            // Decompress and deserialize the data
            var result = new Dictionary<TFrom, Dictionary<TTo, String>>();
            using (var zipped = new MemoryStream(buffer)) {
                using (var s = new GZipStream(zipped, CompressionMode.Decompress)) {
                    using (var sr = new StreamReader(s)) {
                        while (!sr.EndOfStream) {
                            string[] line = sr.ReadLine().Split('\t');
                            if (line.Length != 3)
                                continue;

                            string sfromid = line[0];
                            string stoid = line[1];
                            string svalue = line[2];

                            long fromid;
                            long toid;

                            if (!long.TryParse(sfromid, out fromid))
                                continue;

                            if (!long.TryParse(stoid, out toid))
                                continue;

                            TFrom fromrecord = null;
                            TTo torecord = null;

                            fromrecord = (TFrom)methodTFrom.Invoke(null, new object[] { fromid });
                            if (fromrecord == null)
                                continue;

                            torecord = (TTo)methodTTo.Invoke(null, new object[] { toid });
                            if (torecord == null)
                                continue;

                            if (!result.ContainsKey(fromrecord))
                                result[fromrecord] = new Dictionary<TTo, String>();

                            result[fromrecord][torecord] = svalue;
                        }
                    }
                }
            }

            return result;
        }

        #endregion

        #region IDictionary<TFrom, Dictionary<TTo, String>> Members

        public void Add(TFrom key, Dictionary<TTo, String> value) {
            Data.Add(key, value);
        }

        public bool ContainsKey(TFrom key) {
            return Data.ContainsKey(key);
        }

        public ICollection<TFrom> Keys {
            get { return Data.Keys; }
        }

        public bool Remove(TFrom key) {
            return Data.Remove(key);
        }

        public bool TryGetValue(TFrom key, out Dictionary<TTo, String> value) {
            return Data.TryGetValue(key, out value);
        }

        public ICollection<Dictionary<TTo, String>> Values {
            get { return Data.Values; }
        }

        public Dictionary<TTo, String> this[TFrom key] {
            get {
                return Data[key];
            }
            set {
                Data[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey, Dictionary<TTo, String>>> Members

        public void Add(KeyValuePair<TFrom, Dictionary<TTo, String>> item) {
            Data.Add(item.Key, item.Value);
        }

        public void Clear() {
            Data.Clear();
        }

        public bool Contains(KeyValuePair<TFrom, Dictionary<TTo, String>> item) {
            return Data.Contains(item);
        }

        public void CopyTo(KeyValuePair<TFrom, Dictionary<TTo, String>>[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public int Count {
            get { return Data.Count; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TFrom, Dictionary<TTo, String>> item) {
            if (Data.Contains(item))
                return Data.Remove(item.Key);
            else
                return false;
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,String>> Members

        public IEnumerator<KeyValuePair<TFrom, Dictionary<TTo, String>>> GetEnumerator() {
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
            Data.Add((TFrom)key, (Dictionary<TTo, String>)value);
        }

        public bool Contains(object key) {
            return Data.ContainsKey((TFrom)key);
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
            Data.Remove((TFrom)key);
        }

        ICollection IDictionary.Values {
            get { return Data.Values; }
        }

        public object this[object key] {
            get {
                return Data[(TFrom)key];
            }
            set {
                Data[(TFrom)key] = (Dictionary<TTo, String>)value;
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
