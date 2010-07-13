using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubSonic.Repository;
using System.Configuration;
using System.Windows.Forms;
using SubSonic.DataProviders;
using System.IO;
using SubSonic.Query;
using System.Diagnostics;

namespace Common.Data {
    public class SharedObjects {

        #region Properties

        public SimpleRepository Repository { get; set; }

        public Dictionary<String, Object> Cache { get; protected set; }

        public Dictionary<SimpleRepository, Boolean> NeedsVacuum { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Create a new SimpleRepository using the given Connection String and
        /// Data Provider Name as source.
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <param name="ProviderName"></param>
        /// <returns></returns>
        public SimpleRepository OpenRepository(String ConnectionString, String ProviderName) {
            IDataProvider provider = ProviderFactory.GetProvider(ConnectionString, ProviderName);
            SimpleRepository result = new SimpleRepository(provider, SimpleRepositoryOptions.RunMigrations);

            NeedsVacuum[result] = false;

            return result;
        }

        /// <summary>
        /// Lookup the given Connection String in the App.config and call the overloaded
        /// +OpenRepository+ method with the connection string and provider name as set
        /// in the config file.
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <returns></returns>
        public SimpleRepository OpenRepository(String ConnectionString) {
            if (ConfigurationManager.ConnectionStrings[ConnectionString] == null)
                throw new ArgumentException(String.Format("ConnectionString \"{0}\" not found in the App.config.", ConnectionString), "ConnectionString");

            String connection_string = ConfigurationManager.ConnectionStrings[ConnectionString].ConnectionString;
            String provider_name = ConfigurationManager.ConnectionStrings[ConnectionString].ProviderName;

            // String Replacements
            // - %appdir% is special
            connection_string = connection_string.Replace("%appdir%", Path.GetDirectoryName(Application.ExecutablePath));
            // - all other environment variables are simple
            connection_string = Environment.ExpandEnvironmentVariables(connection_string);

            return OpenRepository(connection_string, provider_name);
        }

        public void SetupRepository(String ConnectionString) {
            Repository = OpenRepository(ConnectionString);
        }

        public void VacuumRepositories() {
            foreach (var kv in NeedsVacuum) {
                SimpleRepository repository = kv.Key;
                Boolean needs_vacuum = kv.Value;

                if (!needs_vacuum)
                    continue;

                IDataProvider provider = DbRecord.GetDataProvider(repository);

                // Do the vacuum on SQLite databases, only
                if (String.Compare(provider.Name, "System.Data.SQLite", true) != 0)
                    continue;

                // Execute the VACUUM statement
                new CodingHorror(provider, "VACUUM;").Execute();

                // Reset the flag
                NeedsVacuum[repository] = false;
            }
        }

        #endregion

        #region Singleton Pattern

        // Reference: http://www.yoda.arachsys.com/csharp/singleton.html

        public SharedObjects() {
            Cache = new Dictionary<String, Object>();
            NeedsVacuum = new Dictionary<SimpleRepository, Boolean>();
        }

        public static SharedObjects Instance {
            get {
                return NestedSharedObjects.instance;
            }
        }

        class NestedSharedObjects {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static NestedSharedObjects() {
            }

            internal static SharedObjects instance = new SharedObjects();
        }

        #endregion

    }
}
