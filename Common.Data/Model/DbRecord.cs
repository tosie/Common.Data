using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using SubSonic.Repository;
using SubSonic.SqlGeneration.Schema;
using SubSonic.DataProviders;

namespace Common.Data {
    public abstract class DbRecord {

        #region Properties

        /// <summary>
        /// Is the repository, from which an object has been loaded.
        /// </summary>
        [SubSonicIgnore]
        public SimpleRepository OwningRepository { get; set; }

        public Int64 Id { get; set; }

        /// <summary>
        /// Is true after a successful call to +Delete+.
        /// </summary>
        [SubSonicIgnore]
        public Boolean Deleted { get; protected set; }

        #endregion

        #region CRUD

        /// <summary>
        /// Returns an instance of the database reader/writer that is
        /// used throughout the code.
        /// </summary>
        [SubSonicIgnore]
        public static SimpleRepository Repository {
            get {
                return SharedObjects.Instance.Repository;
            }
        }

        public static IDataProvider GetDataProvider(SimpleRepository repository) {
            if (repository == null)
                repository = Repository;

            // It is not possible to get the IDataProvider of a SimpleRepository
            // without the use of reflection, since the corresponding field
            // in the SimpleRepository class is non-public
            FieldInfo field = repository.GetType().GetField("_provider", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            IDataProvider provider = (IDataProvider)field.GetValue(repository);
            return provider;
        }

        public static IDataProvider GetDataProvider() {
            return GetDataProvider(Repository);
        }

        /// <summary>
        /// A unique string that is used when working with the cache.
        /// </summary>
        protected static String GetCacheKey<T>(SimpleRepository repository) {
            IDataProvider provider = GetDataProvider(repository);
            return "DbRecord#" + typeof(T).Name + "@" + provider.DbDataProviderName + ":/" + provider.ConnectionString;
        }

        /// <summary>
        /// Object cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        protected static List<T> GetCache<T>(SimpleRepository repository) where T : DbRecord, IDbRecord, new() {
            String cache_key = GetCacheKey<T>(repository);
            if (!SharedObjects.Instance.Cache.ContainsKey(cache_key)) {
                SharedObjects.Instance.Cache[cache_key] = new List<T>();
            }

            return (List<T>)SharedObjects.Instance.Cache[cache_key];
        }

        public static void ResetCacheLoaded<T>(SimpleRepository repository) where T : DbRecord, IDbRecord, new() {
            String cache_key = GetCacheKey<T>(repository) + "_LOADED";
            if (SharedObjects.Instance.Cache.ContainsKey(cache_key))
                SharedObjects.Instance.Cache.Remove(cache_key);
        }

        protected static Boolean IsCached<T>(SimpleRepository repository, Int64 Id) where T : DbRecord, IDbRecord, new() {
            if (repository == null)
                repository = Repository;

            return GetCache<T>(repository).Find(i => i.Id == Id) != null;
        }

        /// <summary>
        /// Creates a new instance of the class, initializes it with default value
        /// and saves it to the database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repository"></param>
        /// <param name="tag">An object which will be passed to InitializeWithDefaults before the instance is added to the database</param>
        /// <returns></returns>
        public static T Create<T>(SimpleRepository repository, Object tag) where T : DbRecord, IDbRecord, new() {
            if (repository == null)
                repository = Repository;

            // Create a new instance of T
            T instance = new T();
            instance.Deleted = false;
            instance.OwningRepository = repository;

            // Initialize the new object with default values
            instance.InitializeWithDefaults(tag);

            // Add it to the database
            repository.Add<T>(instance);
            
            // Add it to the cache
            List<T> Cache = GetCache<T>(repository);
            Cache.Add(instance);

            return instance;
        }

        /// <summary>
        /// Creates a new instance of the class, initializes it with default values
        /// and saves it to the database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repository"></param>
        /// <returns></returns>
        public static T Create<T>(SimpleRepository repository) where T : DbRecord, IDbRecord, new() {
            return Create<T>(repository, null);
        }

        /// <summary>
        /// Creates a new instance of the class, initializes it with default values
        /// and saves it to the database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static T Create<T>(Object tag) where T : DbRecord, IDbRecord, new() {
            return Create<T>(Repository, tag);
        }

        /// <summary>
        /// Creates a new instance of the class, initializes it with default values
        /// and saves it to the database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repository"></param>
        /// <returns></returns>
        public static T Create<T>() where T : DbRecord, IDbRecord, new() {
            return Create<T>(Repository, null);
        }

        /// <summary>
        /// Returns a specific record from the database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repository"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static T Read<T>(SimpleRepository repository, Int64 Id) where T : DbRecord, IDbRecord, new() {
            if (repository == null)
                repository = Repository;

            // Look inside the object cache if it has already been loaded
            List<T> Cache = GetCache<T>(repository);
            T instance = Cache.Find(i => i.Id == Id);
            if (instance != null)
                return instance;

            // If not found in the cache, get it from the database
            try {
                IList<T> instances = repository.Find<T>(i => i.Id == Id);
                if (instances.Count <= 0)
                    return null;

                instance = instances[0];
                Trace.Assert(instance != null);
            } catch {
                return null;
            }

            // Let the object load itself
            instance.Deleted = false;
            instance.OwningRepository = repository;
            instance.AfterLoad();

            // Add the instance to the cache
            Cache.Add(instance);

            // Return the object
            return instance;
        }

        /// <summary>
        /// Returns a specific record from the database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static T Read<T>(Int64 Id) where T : DbRecord, IDbRecord, new() {
            return Read<T>(Repository, Id);
        }

        /// <summary>
        /// Returns all records from the database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repository"></param>
        /// <returns></returns>
        public static List<T> Read<T>(SimpleRepository repository) where T : DbRecord, IDbRecord, new() {
            if (repository == null)
                repository = Repository;

            // Read everything from the database, that is not already
            // in the cache and ensure object consistency
            // (only do this once!)
            List<T> Cache = GetCache<T>(repository);
            if (!SharedObjects.Instance.Cache.ContainsKey(GetCacheKey<T>(repository) + "_LOADED")) {
                List<Int64> ids = new List<Int64>(Cache.Count);
                Cache.ForEach(i => ids.Add(i.Id));
                List<T> instances = repository.All<T>().ToList();

                // This cannot be done inside the query, as +ids+ is not locally available there
                instances = instances.Where(i => !ids.Contains(i.Id)).ToList();

                // Let the objects load themselves
                instances.ForEach(instance => {
                    instance.Deleted = false;
                    instance.OwningRepository = repository;
                    instance.AfterLoad();
                });

                // Add the new objects to the cache
                Cache.AddRange(instances);

                // Make sure that this gets executed only once
                SharedObjects.Instance.Cache[GetCacheKey<T>(repository) + "_LOADED"] = true;
            }

            // Return _all_ elements, not only the ones just read from
            // the database
            return Cache;
        }

        /// <summary>
        /// Returns all records from the database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> Read<T>() where T : DbRecord, IDbRecord, new() {
            return Read<T>(Repository);
        }

        /// <summary>
        /// Returns all records that have been loaded and are in the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repository"></param>
        /// <returns></returns>
        public static List<T> ReadFromCache<T>(SimpleRepository repository) where T : DbRecord, IDbRecord, new() {
            if (repository == null)
                repository = Repository;

            // Read everything from the database, that is not already
            // in the cache and ensure object consistency
            // (only do this once!)
            List<T> Cache = GetCache<T>(repository);
            if (!SharedObjects.Instance.Cache.ContainsKey(GetCacheKey<T>(repository) + "_LOADED")) {
                return null;
            }

            // Return _all_ elements from the cache
            return Cache;
        }

        /// <summary>
        /// Returns all records that have been loaded and are in the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> ReadFromCache<T>() where T : DbRecord, IDbRecord, new() {
            return ReadFromCache<T>(Repository);
        }

        /// <summary>
        /// Saves changes of an object to the database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Update<T>() where T : DbRecord, IDbRecord, new() {
            if (!(this as T).BeforeUpdate())
                return;

            OwningRepository.Update<T>(this as T);
        }

        /// <summary>
        /// Removes a record from the database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Boolean Delete<T>() where T : DbRecord, IDbRecord, new() {
            // Nested delete
            if (!(this as T).BeforeDelete())
                return false;

            // Delete from cache
            GetCache<T>(OwningRepository).Remove(this as T);

            // Delete from database
            OwningRepository.Delete<T>((this as T).Id);

            // Remember that something has been deleted from this repository
            // (so that vacuum is not done on all repositories)
            SharedObjects.Instance.NeedsVacuum[OwningRepository] = true;

            Deleted = true;
            return true;
        }

        #endregion

    }
}
