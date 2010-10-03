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
    public partial class DbRecordCollectionView : UserControl {

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
            /// The value shown in a cell is retrieved using this property name.
            /// </summary>
            public string PropertyName { get; set; }

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
            /// <param name="propertyName">The value shown in a cell is retrieved using this property name.</param>
            /// <param name="headerText">The text to be shown as the column header.</param>
            /// <param name="initialWidth">The width with which the column is initialized.</param>
            public ColumnAttribute(int sortKey, string propertyName, string headerText, int initialWidth) {
                this.SortKey = sortKey;
                this.PropertyName = propertyName;
                this.HeaderText = headerText;
                this.InitialWidth = initialWidth;
            }

            /// <summary>
            /// Creates a new <see cref="ColumnDefinition"/> based on the attribute values.
            /// </summary>
            /// <returns>A new <see cref="ColumnDefinition"/> with it's values set to the one of this attribute.</returns>
            public ColumnDefinition CreateColumnDefinition() {
                var column = new ColumnDefinition(PropertyName, HeaderText, InitialWidth);
                return column;
            }

            /// <summary>
            /// Returns an array of all column definitions that are assigned to a property by using the <see cref="ColumnAttribute"/> and orders them by their <see cref="SortKey"/>.
            /// </summary>
            /// <param name="property">The property to create the columns for.</param>
            /// <returns>A list of <see cref="ColumnDefinition"/> instances.</returns>
            static public ColumnDefinition[] GetColumnsForCollection(PropertyInfo property) {
                var attrs = property.GetCustomAttributes(typeof(ColumnAttribute), true);
                var count = attrs.Length;
                if (count <= 0)
                    return new ColumnDefinition[0];

                var result = from attr in attrs
                             orderby (attr as ColumnAttribute).SortKey
                             select (attr as ColumnAttribute).CreateColumnDefinition();

                return result.ToArray();
            }

        }

        #endregion

    }
}