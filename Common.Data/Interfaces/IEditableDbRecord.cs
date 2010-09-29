using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Data {
    /// <summary>
    /// This interface must be implemented by database record classes that want to be editable through one of the predefined edit forms and controls.
    /// </summary>
    public interface IEditableDbRecord : IDbRecord {
        /// <summary>
        /// Name of a database record. Is shown in the controls to distinguish records.
        /// </summary>
        String Name { get; set; }

        /// <summary>
        /// Is called when a record should be duplicated. Must return an exact duplication (except for the <see cref="DbRecord.Id"/> property) of the record this method was called on.
        /// </summary>
        /// <returns></returns>
        IEditableDbRecord Duplicate();

        /// <summary>
        /// Called whenever changes to the record should be written to the database.
        /// </summary>
        void Update();

        /// <summary>
        /// Called when the record should be removed from the database.
        /// </summary>
        /// <returns>Return false if the record was not deleted, return true if it was deleted successfully.</returns>
        Boolean Delete();
    }
}
