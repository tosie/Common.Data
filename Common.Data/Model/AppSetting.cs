using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using SubSonic.Repository;
using SubSonic.SqlGeneration.Schema;

namespace Common.Data {
    public class AppSetting : DbRecord, IDbRecord {

        #region Properties

        [SubSonicIgnore]
        public new Int64 Id { get; set; }

        [SubSonicPrimaryKey]
        public String Name { get; set; }

        public Int32 IntValue { get; set; }

        [SubSonicNullString]
        public String StringValue { get; set; }

        public Boolean BoolValue { get; set; }

        #endregion

        #region Custom Model Methods

        public static Int32 Int(String key, Int32 default_value) {
            AppSetting setting = AppSetting.Read(key);
            if (setting == null) {
                return default_value;
            } else {
                return setting.IntValue;
            }
        }

        public static void SetInt(String key, Int32 value) {
            AppSetting setting = AppSetting.Read(key);
            if (setting == null) {
                setting = AppSetting.Create(value);
            } else {
                setting.IntValue = value;
                setting.Update();
            }
        }

        public static String String(String key, String default_value) {
            AppSetting setting = AppSetting.Read(key);
            if (setting == null) {
                return default_value;
            } else {
                return setting.StringValue;
            }
        }

        public static void SetString(String key, String value) {
            AppSetting setting = AppSetting.Read(key);
            if (setting == null) {
                setting = AppSetting.Create(value);
            } else {
                setting.StringValue = value;
                setting.Update();
            }
        }

        public static Boolean Bool(String key, Boolean default_value) {
            AppSetting setting = AppSetting.Read(key);
            if (setting == null) {
                return default_value;
            } else {
                return setting.BoolValue;
            }
        }

        public static void SetBool(String key, Boolean value) {
            AppSetting setting = AppSetting.Read(key);
            if (setting == null) {
                setting = AppSetting.Create(value);
            } else {
                setting.BoolValue = value;
                setting.Update();
            }
        }

        #endregion

        #region DbRecord Members

        public static SimpleRepository ActiveRepository;

        public static AppSetting Create(Object Tag) {
            return AppSetting.Create<AppSetting>(ActiveRepository, Tag);
        }

        public static AppSetting Read(Int64 Id) {
            return AppSetting.Read<AppSetting>(ActiveRepository, Id);
        }

        public static AppSetting Read(String Name) {
            // Look inside the object cache if it has already been loaded
            List<AppSetting> Cache = GetCache<AppSetting>(ActiveRepository);
            AppSetting instance = Cache.Find(i => i.Name == Name);
            if (instance != null)
                return instance;

            // If not found in the cache, get it from the database
            try {
                instance = ActiveRepository.Find<AppSetting>(i => i.Name == Name).First();
                Trace.Assert(instance != null);
            } catch {
                return null;
            }

            // Let the object load itself
            instance.Deleted = false;
            instance.OwningRepository = ActiveRepository;
            instance.AfterLoad();

            // Add the instance to the cache
            Cache.Add(instance);

            // Return the object
            return instance;
        }

        public static List<AppSetting> Read() {
            return AppSetting.Read<AppSetting>(ActiveRepository);
        }

        public void Update() {
            Update<AppSetting>();
        }

        public bool Delete() {
            return Delete<AppSetting>();
        }

        #endregion

        #region IDbRecord Members

        public void InitializeWithDefaults(Object Tag) {
            if (Tag == null)
                return;

            if (Tag is Int32)
                IntValue = (Int32)Tag;
            else if (Tag is String)
                StringValue = (String)Tag;
            else if (Tag is Boolean)
                BoolValue = (Boolean)Tag;
            else
                throw new ArgumentException(System.String.Format("Unbekannter Datentyp für AppSetting: {0}", Tag.GetType()), "Tag");
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
