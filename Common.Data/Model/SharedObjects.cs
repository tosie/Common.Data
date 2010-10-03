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
using System.Runtime.InteropServices;
using System.Reflection;

namespace Common.Data {
    public class SharedObjects {

        #region Properties

        /// <summary>
        /// Contains the default repository to use by the CRUD methods.
        /// </summary>
        public SimpleRepository Repository { get; set; }

        /// <summary>
        /// Cache used by the CRUD methods.
        /// </summary>
        public Dictionary<String, Object> Cache { get; protected set; }

        /// <summary>
        /// Holds a list of repositories that receive a VACUUM command when <see cref="VacuumRepositories"/> is called.
        /// Only entries with a value of true will receive the command.
        /// </summary>
        public Dictionary<SimpleRepository, Boolean> NeedsVacuum { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Create a new SimpleRepository using the given connection string and data provider name as source.
        /// </summary>
        /// <param name="ConnectionString">The connection string to pass to the data provider.</param>
        /// <param name="ProviderName">The name of the data provider to use.</param>
        /// <returns></returns>
        public SimpleRepository OpenRepository(String ConnectionString, String ProviderName) {
            IDataProvider provider = ProviderFactory.GetProvider(ConnectionString, ProviderName);
            SimpleRepository result = new SimpleRepository(provider, SimpleRepositoryOptions.RunMigrations);

            NeedsVacuum[result] = false;

            return result;
        }

        /// <summary>
        /// Lookup the given Connection String in the App.config and call the overloaded OpenRepository method
        /// with the connection string and provider name as set in the config file.
        /// </summary>
        /// <param name="ConnectionString">Key of the connection string to use as defined in the app.config.</param>
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

        /// <summary>
        /// Opens the repository specified by <paramref name="ConnectionString"/> and <paramref name="ProviderName"/>. After the
        /// repository has been opened, it is set as the default repository, too.
        /// </summary>
        /// <param name="ConnectionString">The connection string to pass to the data provider.</param>
        /// <param name="ProviderName">The name of the data provider to use.</param>
        public void SetupRepository(String ConnectionString, String ProviderName) {
            Repository = OpenRepository(ConnectionString, ProviderName);
        }

        /// <summary>
        /// Opens a repository by looking up its real connection string and data provider name in the app.config. The opened repository will become the default repository, too.
        /// </summary>
        /// <param name="ConnectionString">Key of the connection string to use as defined in the app.config.</param>
        public void SetupRepository(String ConnectionString) {
            Repository = OpenRepository(ConnectionString);
        }

        /// <summary>
        /// Executes the VACUUM command on all repositories that are marked using the <see cref="NeedsVacuum"/> property.
        /// </summary>
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
            }

            // Reset the flag
            SimpleRepository[] keys = NeedsVacuum.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++) {
                NeedsVacuum[keys[i]] = false;
            }
        }

        /// <summary>
        /// This method tries to preload the sqlite3 DLL. It does so by looking in <paramref name="libDir"/> first and using the default search paths afterwards. This is automatically called by the constructor of the <see cref="SharedObjects"/> class.
        /// </summary>
        /// <param name="libDir">The full name of the directory containing libraries (may contain environment variables as well as %appdir%, which will be replaced by the directory of the applications executable path, and %platform%, which will be replaced by "x86" or "x64" depending on the current platform the process is running on).</param>
        /// <returns>True, if the SQLite library was loaded, false otherwise.</returns>
        public bool LoadSqliteLibrary(string libDir) {
            // Make sure the program knows about the SQLite Data Provider.
            try {
                var dataSet = ConfigurationManager.GetSection("system.data") as System.Data.DataSet;
                dataSet.Tables[0].Rows.Add("SQLite Data Provider"
                , ".Net Framework Data Provider for SQLite"
                , "System.Data.SQLite"
                , "System.Data.SQLite.SQLiteFactory, System.Data.SQLite");
            } catch (System.Data.ConstraintException) { }

            // Make sure the bundled System.Data.SQLite assembly is used.
            AppDomain.CurrentDomain.AssemblyResolve += (_, e) => {
                if (String.Compare(e.Name, "System.Data.SQLite", true) != 0)
                    return null;

                return Assembly.GetExecutingAssembly();
            };

            return LibraryLoader.TryLoad(libDir, "sqlite3.dll");
        }

        #endregion

        #region Singleton Pattern

        // Reference: http://www.yoda.arachsys.com/csharp/singleton.html

        internal SharedObjects() {
            Cache = new Dictionary<String, Object>();
            NeedsVacuum = new Dictionary<SimpleRepository, Boolean>();

            // Try preloading the SQLite library.
            LoadSqliteLibrary(@"%appdir%\Libs\%platform%");
        }

        /// <summary>
        /// Returns an instace of the SharedObjects class.
        /// </summary>
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
