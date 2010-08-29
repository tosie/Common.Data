using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubSonic.SqlGeneration.Schema;
using Common.Configuration;
using System.Windows.Forms;

namespace Common.Data.Test {
    class DummyModelHasMany : DbRecord, IEditableDbRecord {

        #region Properties

        [Configuration("Name", 10, "Common",
            ControlType = ConfigurationEntry.ControlTypes.TextBox)]
        public String Name { get; set; }

        [Configuration("Value", 20, "Common",
            ControlType = ConfigurationEntry.ControlTypes.Slider,
            Minimum = 0, Maximum = 200, Validator = "Int32")]
        public Int32 Value { get; set; }

        [Configuration("Has-Many-Relation", 30, "HasMany",
            ControlType = ConfigurationEntry.ControlTypes.Button)]
        [SubSonicIgnore]
        public HasMany<DummyModel> Set { get; protected set; }
        protected byte[] SetAsBinary {
            get { return Set.BinaryData; }
            set { Set.BinaryData = value; }
        }
        bool SetEditor(ConfigurationEntry Sender, IWin32Window Owner) {
            HasManyEditForm.SelectRecords(Owner, "DummyModelHasManySetEditor",
                "Play around", this, "Set");

            // Nothing else to save, the has-many-set has been saved by the edit form.
            return false;
        }

        #endregion

        #region DbRecord Members

        public static DummyModelHasMany Create(Object Tag) {
            return DummyModelHasMany.Create<DummyModelHasMany>(Tag);
        }

        public static DummyModelHasMany Read(Int64 Id) {
            return DummyModelHasMany.Read<DummyModelHasMany>(Id);
        }

        public static List<DummyModelHasMany> Read() {
            return DummyModelHasMany.Read<DummyModelHasMany>();
        }

        public void Update() {
            Update<DummyModelHasMany>();
        }

        public bool Delete() {
            return Delete<DummyModelHasMany>();
        }

        #endregion

        #region IEditableDbRecord Members

        public IEditableDbRecord Duplicate() {
            var result = DummyModelHasMany.Create(null);

            result.Name = Name;
            result.Value = Value;
            result.SetAsBinary = SetAsBinary;

            return result;
        }

        public void InitializeWithDefaults(Object Tag) {
            Set = new HasMany<DummyModel>();

            if (Tag is object[]) {
                object[] options = (object[])Tag;
                Name = (String)options[0];
                Value = (Int32)options[1];
            }
        }

        public void AfterLoad() {
            //
        }

        public bool BeforeUpdate() {
            return true;
        }

        public bool BeforeDelete() {
            return true;
        }

        #endregion

    }
}
