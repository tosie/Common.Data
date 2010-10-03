using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace Common.Data {

    #region RecordEvent

    /// <summary>
    /// Provides the static <see cref="Fire"/> method, that helps handling record events.
    /// </summary>
    public static class RecordEvent {
        /// <summary>
        /// Fires a given event and passes it with the correct arguments to the registered event handlers. It returns the <see cref="RecordEventArgs"/> structure it created and that the event handlers might have modified.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="handler">The event to fire.</param>
        /// <param name="record">The record to pass to the event handlers (may be null).</param>
        /// <returns>A new instance of the <see cref="RecordEventArgs"/> class that has been passed to every event handler and might have been modified.</returns>
        public static RecordEventArgs Fire(Control sender, RecordEventHandler handler, IEditableDbRecord record) {
            var args = new RecordEventArgs() {
                Record = record,
                Continue = true
            };

            if (handler == null)
                return args;

            handler(sender, args);

            return args;
        }
    }

    #endregion

    #region RecordEventArgs

    /// <summary>
    /// Data structure used to pass arguments between <see cref="RecordEventHandler"/> methods.
    /// </summary>
    public class RecordEventArgs {

        /// <summary>
        /// If the event handler is a "before" event handler, this is null and can be set by the event handler to a new record that should be used instead of creating a new one.
        /// If the event handler is an "after" event handler, this is a reference to the record that has been added to the list. Changes here will not have any effect.
        /// </summary>
        public IEditableDbRecord Record { get; set; }

        /// <summary>
        /// Only available for "before" event handlers. If set to true, the process continues. If set to false, record creation/deletion/... processes are aborted.
        /// </summary>
        public bool Continue { get; set; }

        /// <summary>
        /// Copies values from another instance of this class.
        /// </summary>
        /// <param name="e">The source to copy from.</param>
        public void CopyFrom(RecordEventArgs e) {
            this.Record = e.Record;
            this.Continue = e.Continue;
        }
    }

    #endregion

    #region CollectionEvent

    /// <summary>
    /// Provides the static <see cref="Fire"/> method, that helps handling collection events.
    /// </summary>
    public static class CollectionEvent {
        /// <summary>
        /// Fires a given event and passes it with the correct arguments to the registered event handlers. It returns the <see cref="CollectionEventArgs"/> structure it created and that the event handlers might have modified.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="handler">The event to fire.</param>
        /// <param name="record">The record the collection belongs to.</param>
        /// <param name="collection">The collection to pass to the event handlers.</param>
        /// <returns>A new instance of the <see cref="CollectionEventArgs"/> class that has been passed to every event handler and might have been modified.</returns>
        public static CollectionEventArgs Fire(Control sender, CollectionEventHandler handler, IEditableDbRecord record, PropertyInfo collection) {
            var args = new CollectionEventArgs(record, collection);

            if (handler == null)
                return args;

            handler(sender, args);

            return args;
        }
    }

    #endregion

    #region CollectionEventArgs

    /// <summary>
    /// Data structure used to pass arguments between <see cref="CollectionEventHandler"/> methods.
    /// </summary>
    public class CollectionEventArgs {

        /// <summary>
        /// This is a reference to the record that owns the collection.
        /// </summary>
        public IEditableDbRecord Record { get; protected set; }

        /// <summary>
        /// This is a reference to the property of the record that represents the collection.
        /// </summary>
        public PropertyInfo Collection { get; protected set; }

        /// <summary>
        /// Creates a new instance of the <see cref="CollectionEventArgs"/> class and initializes it with the given values.
        /// </summary>
        /// <param name="record">Reference to the record that owns the collection.</param>
        /// <param name="collection">Reference to the property of the record that represents the collection.</param>
        public CollectionEventArgs(IEditableDbRecord record, PropertyInfo collection) {
            this.Record = record;
            this.Collection = collection;
        }

        /// <summary>
        /// Copies values from another instance of this class.
        /// </summary>
        /// <param name="e">The source to copy from.</param>
        public void CopyFrom(CollectionEventArgs e) {
            this.Record = e.Record;
            this.Collection = e.Collection;
        }
    }

    #endregion

    #region Delegates

    /// <summary>
    /// Delegate that defines the method signature that is used with record events.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void RecordEventHandler(Control sender, RecordEventArgs e);

    /// <summary>
    /// Delegate that defines the method signature that is used with collection events.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void CollectionEventHandler(Control sender, CollectionEventArgs e);

    #endregion

}
