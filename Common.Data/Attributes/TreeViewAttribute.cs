using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Common.Configuration;

namespace Common.Data {
    /// <summary>
    /// Attribute used to configure the way database records are shown in a tree view.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class TreeViewAttribute : Attribute {

        /// <summary>
        /// Text to use as a collections label.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Attribute used to configure the way database records are shown in a tree view.
        /// </summary>
        /// <param name="text">Text to use as a collections label.</param>
        public TreeViewAttribute(string text) {
            Text = text;
        }

        /// <summary>
        /// Returns the TreeViewAttribute instance of a property. If it has no such attribute, null is returned.
        /// </summary>
        /// <param name="property">The property to check.</param>
        /// <returns>Returns the TreeViewAttribute instance of the property. If it has no such attribute, null is returned.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="property"/> is null.</exception>
        public static TreeViewAttribute GetAttribute(PropertyInfo property) {
            if (property == null)
                throw new ArgumentNullException("property");

            var attrs = property.GetCustomAttributes(typeof(TreeViewAttribute), true);
            if (attrs.Length <= 0) {
                // Fallback to the ConfigurationAttribute ...
                attrs = property.GetCustomAttributes(typeof(ConfigurationAttribute), true);
                if (attrs.Length <= 0)
                    return null;

                // If there is a ConfigurationAttribute, create a new TreeViewAttribute with the text of the config attribute.
                var configattr = (ConfigurationAttribute)attrs[0];
                attrs = new Attribute[] { new TreeViewAttribute(configattr.Text) };
            }

            var attr = (TreeViewAttribute)attrs[0];
            return attr;
        }

        /// <summary>
        /// Determines whether a property has a TreeViewAttribute.
        /// </summary>
        /// <param name="property">The property to check.</param>
        /// <returns>Returns true if the property has a TreeViewAttribute, otherwise false.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="property"/> is null.</exception>
        public static bool HasAttribute(PropertyInfo property) {
            if (property == null)
                throw new ArgumentNullException("property");

            return GetAttribute(property) != null;
        }

        /// <summary>
        /// Returns the text of a property as set by its TreeViewAttribute. If it has no such attribute, null is returned.
        /// </summary>
        /// <param name="property">The property to check.</param>
        /// <returns>Returns the text of the property as set by its TreeViewAttribute. If it has no such attribute, null is returned.</returns>
        public static string GetText(PropertyInfo property) {
            if (property == null)
                return null;

            var attr = GetAttribute(property);
            if (attr == null)
                return null;
            else
                return attr.Text;
        }

    }
}
