using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Common.Data {

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

    /// <summary>
    /// Delegate that defines the method signature that is used with record events.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <seealso cref="AddingRecord"/>
    /// <seealso cref="AddedRecord"/>
    /// <seealso cref="DeletingRecord"/>
    /// <seealso cref="DeletedRecord"/>
    public delegate void RecordEventHandler(Control sender, RecordEventArgs e);

}
