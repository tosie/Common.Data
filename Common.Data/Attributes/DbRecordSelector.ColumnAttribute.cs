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
using System.Collections;

namespace Common.Data {
    public partial class DbRecordSelector : UserControl {

        #region Data Structures

        /// <summary>
        /// Attribute that specifies a column that should be shown by a <see cref="DbRecordCollectionView"/>.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
        public class ColumnAttribute : Attribute {

            /// <summary>
            /// Determines the order in which columns should appear.
            /// </summary>
            public int SortKey { get; set; }

            /// <summary>
            /// The text to be shown as the column header.
            /// </summary>
            public string HeaderText { get; set; }

            /// <summary>
            /// The width with which the column is initialized.
            /// </summary>
            public int InitialWidth { get; set; }

            /// <summary>
            /// Creates a new column to be shown when editing the collection.
            /// </summary>
            /// <param name="sortKey">Determines the order in which columns should appear.</param>
            /// <param name="headerText">The text to be shown as the column header.</param>
            /// <param name="initialWidth">The width with which the column is initialized.</param>
            public ColumnAttribute(int sortKey, string headerText, int initialWidth) {
                this.SortKey = sortKey;
                this.HeaderText = headerText;
                this.InitialWidth = initialWidth;
            }

            /// <summary>
            /// Creates a new <see cref="ColumnDefinition"/> based on the attribute values.
            /// </summary>
            /// <returns>A new <see cref="ColumnDefinition"/> with it's values set to the one of this attribute.</returns>
            public ColumnDefinition CreateColumnDefinition(PropertyInfo property) {
                var column = new ColumnDefinition(property, HeaderText, InitialWidth);
                return column;
            }

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="recordType">TODO</param>
            /// <returns>An array of <see cref="ColumnDefinition"/> instances.</returns>
            static public ColumnDefinition[] GetColumns(Type recordType) {
                if (recordType == null)
                    return new ColumnDefinition[0];

                var properties = recordType.GetProperties();
                if (properties.Length <= 0)
                    return new ColumnDefinition[0];

                List<ColumnDefinition> result = new List<ColumnDefinition>();

                for (int i = 0; i < properties.Length; i++) {
                    var property = properties[i];

                    var attrs = property.GetCustomAttributes(typeof(ColumnAttribute), true);
                    if (attrs.Length <= 0)
                        continue;

                    result.AddRange(
                        from attr in attrs
                        orderby (attr as ColumnAttribute).SortKey
                        select (attr as ColumnAttribute).CreateColumnDefinition(property));
                }

                return result.ToArray();
            }

        }

        #endregion

    }
}