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
        /// Used by the <see cref="DbRecordSelector"/> as a tuple to determine what columns to show in the list view.
        /// </summary>
        [DebuggerDisplay("{Property} as {HeaderText} with a width of {InitialWidth}")]
        public class ColumnDefinition {

            /// <summary>
            /// The text to be shown as the column header.
            /// </summary>
            public String HeaderText { get; set; }

            /// <summary>
            /// The width with which the column is initialized.
            /// </summary>
            public Int32 InitialWidth { get; set; }

            /// <summary>
            /// Reference to the property info for this column.
            /// </summary>
            public PropertyInfo Property { get; set; }

            /// <summary>
            /// Reference to the "&lt;PropertyName&gt;FormatValueForCell" method of the record.
            /// </summary>
            /// <remarks>The method's signature should look like this: (object {collection item}) => (object).</remarks>
            public MethodInfo FormatValueMethod { get; set; }

            /// <summary>
            /// Create a new instance of this class.
            /// </summary>
            /// <param name="Property">The value shown in a cell is retrieved using this property.</param>
            /// <param name="HeaderText">The text to be shown as the column header.</param>
            public ColumnDefinition(PropertyInfo Property, String HeaderText) {
                this.Property = Property;
                this.HeaderText = HeaderText;

                this.InitialWidth = 150;
            }

            /// <summary>
            /// Create a new instance of this class.
            /// </summary>
            /// <param name="Property">The value shown in a cell is retrieved using this property.</param>
            /// <param name="HeaderText">The text to be shown as the column header.</param>
            /// <param name="InitialWidth">The width with which the column is initialized.</param>
            public ColumnDefinition(PropertyInfo Property, String HeaderText, Int32 InitialWidth) {
                this.Property = Property;
                this.HeaderText = HeaderText;
                this.InitialWidth = InitialWidth;
            }

            /// <summary>
            /// Returns a formatted string of the value of the property linked to by <see cref="Property"/>.
            /// </summary>
            /// <param name="Instance">Instance of an object that is based on the HasMany-Type and implements the <see cref="Property"/>.</param>
            /// <returns>A string to be shown in a list view cell.</returns>
            /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="Instance"/> is null.</exception>
            public String GetFormattedValue(Object Instance) {
                if (Instance == null)
                    return ""; // TODO
                // throw new ArgumentNullException("Instance");

                if (Property == null)
                    return "";

                object value = Property.GetValue(Instance, null);

                if (FormatValueMethod != null) {
                    value = FormatValueMethod.Invoke(Instance, new object[] { value });
                }

                if (value == null)
                    return "";
                else
                    return value.ToString();
            }

        }

        #endregion

    }
}
