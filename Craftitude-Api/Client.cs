#region Imports (20)

using Raven;
using Raven.Client;
using Raven.Client.Connection;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Linq;
using Raven.Database;
using Raven.Imports.Newtonsoft.Json;
using Raven.Json;
using Raven.Json.Linq;
using Raven.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion Imports (20)

namespace Craftitude
{


    public class Client
    {
        private int nextOperationIndexToAssign = 0;

        /// <summary>
        /// The working directory of the client. This is also the target path for all installations.
        /// </summary>
        public string BasePath { get; private set; }

        /// <summary>
        /// The cache for this client.
        /// </summary>
        public Cache Cache { get; private set; }

        /// <summary>
        /// Currently running operations.
        /// </summary>
        public ObservableCollection<Operation> Operations { get; private set; }

        /// <summary>
        /// The repositories which have been added to the client.
        /// </summary>
        internal List<DocumentStore> _repositories { get; private set; }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="basePath">The working directory of the client. This is also the target path for all installations.</param>
        public Client(string basePath)
        {
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);
            this.BasePath = Path.GetFullPath(basePath);

            string craftitudeBasePath = Path.Combine(basePath, ".craftitude");
            if (!Directory.Exists(craftitudeBasePath))
                Directory.CreateDirectory(craftitudeBasePath);

            this.Cache = new Cache(this, Path.Combine(craftitudeBasePath, "craftitude.cache"));

            // Load repositories
            this._repositories = new List<Repository>();
            string repositoriesConf = Path.Combine(craftitudeBasePath, "repositories.conf");
            if (!File.Exists(repositoriesConf))
            {
                using (var f = File.CreateText(repositoriesConf))
                    f.Flush();
            }
            else
            {
                using (var f = File.OpenText(repositoriesConf))
                {
                    while (!f.EndOfStream)
                    {
                        var line = f.ReadLine();
                        if (Repository.IsValidConfigString(line))
                            this._repositories.AddRange(Repository.FromConfigString(line, this));
                    }
                }
            }

            // Operations collection
            this.Operations = new ObservableCollection<Operation>();
        }

        /// <summary>
        /// Add a repository and connect to it.
        /// </summary>
        /// <param name="repositoryUrl">The repository url. The syntax is "cref://host[:port][/path]/repositoryId".</param>
        public void AddRepository(string repositoryUrl)
        {
            AddRepository(new Uri(repositoryUrl));
        }

        /// <summary>
        /// Add a repository and connect to it.
        /// </summary>
        /// <param name="repositoryUri">The repository uri. Scheme must be "crep" and path must at least contain the repositoryId.</param>
        public void AddRepository(Uri repositoryUri)
        {
            string databaseName = string.Empty;
            repositoryUri = GetHttpFromCrepUrl(repositoryUri, out databaseName);

            if (_repositories.Where(r => r.Url == repositoryUri.ToString() && r.DefaultDatabase == databaseName).Any())
                return; // already added

            var document = new DocumentStore();
            document.Url = repositoryUri.ToString();
            document.DefaultDatabase = databaseName;
            document.Initialize();

            _repositories.Add(document);
        }

        /// <summary>
        /// Disconnect from a repository and remove it.
        /// </summary>
        /// <param name="repositoryUri">The repository uri. Scheme must be "crep" and path must at least contain the repositoryId.</param>
        public void DeleteRepository(Uri repositoryUri)
        {
            string databaseName = string.Empty;
            repositoryUri = GetHttpFromCrepUrl(repositoryUri, out databaseName);

            // Dispose connection
            foreach (var r in _repositories.Where(r => r.Url == repositoryUri.ToString() && r.DefaultDatabase == databaseName))
                r.Dispose();

            // Remove connection
            _repositories.RemoveAll(r => r.Url == repositoryUri.ToString() && r.DefaultDatabase == databaseName);
        }

        /// <summary>
        /// Fetches up-to-date information about a package.
        /// </summary>
        /// <param name="packageID">The package ID to ask information for</param>
        /// <returns>All available package information for a package ID.
        /// This is usually only containing exact one PackageInfo item but in case there are duplicates on multiple repositories it can also be more.</returns>
        public IEnumerable<PackageInfo> GetRemotePackageInfo(string packageID)
        {
            foreach (var repository in _repositories)
                using (var session = repository.OpenSession())
                    foreach (var result in session.Query<PackageInfo>(packageID))
                        yield return result;
        }

        /// <summary>
        /// Gets locally cached package information for a specific package.
        /// </summary>
        /// <param name="packageID">The specific package ID</param>
        /// <exception cref="LocalCacheCorruptException" />
        /// <returns>NULL, if the package isn't installed. Package information if it is installed.</returns>
        public PackageInfo GetLocalPackageInfo(string packageID)
        {
            using (var session = Cache._store.OpenSession())
            {
                var result = session.Query<PackageInfo>(packageID);
                if (!result.Any())
                    return null;
                else
                    if (result.Count() == 1)
                        return result.Single();
                    else
                        throw new LocalCacheCorruptException("Local cache corrupt.");
            }
        }

        /// <summary>
        /// Checks if a Uri is a valid reference to a Craftitude repository database server.
        /// </summary>
        /// <param name="repositoryUri">The repository uri to check.</param>
        /// <returns>True for valid Uris, False for invalid Uris.</returns>
        protected static bool IsValidCrepUrl(Uri repositoryUri)
        {
            if (!repositoryUri.Scheme.Equals("crep", StringComparison.OrdinalIgnoreCase))
                return false;

            string databaseName = repositoryUri.AbsolutePath.Split('/').Last();
            if (string.IsNullOrEmpty(databaseName))
                return false;

            return true;
        }

        /// <summary>
        /// Gets the raw HTTP Uri from a Craftitude repository database server reference Uri.
        /// </summary>
        /// <param name="repositoryUri">The repository uri. The syntax is "cref://host[:port][/path]/repositoryId".</param>
        /// <param name="databaseName">The database name (repositoryId) read from the Uri.</param>
        /// <returns></returns>
        protected static Uri GetHttpFromCrepUrl(Uri repositoryUri, out string databaseName)
        {
            if (!repositoryUri.Scheme.Equals("crep", StringComparison.OrdinalIgnoreCase))
                throw new UriFormatException("Not a valid craftitude repository URI.");

            databaseName = repositoryUri.AbsolutePath.Split('/').Last();
            if (string.IsNullOrEmpty(databaseName))
                throw new UriFormatException("Not a valid Craftitude repository URI.");

            var uriRebuilder = new UriBuilder(repositoryUri);
            uriRebuilder.Path = repositoryUri.AbsolutePath.Substring(0, repositoryUri.AbsolutePath.Length - databaseName.Length - 1);
            uriRebuilder.Scheme = "http";

            return uriRebuilder.Uri;
        }

        /// <summary>
        /// Gets the ID for the next operation and increases it by one.
        /// </summary>
        /// <returns>The next available operation ID</returns>
        internal int GetOperationID()
        {
            int newOperationIndex = nextOperationIndexToAssign;
            nextOperationIndexToAssign++;
            return newOperationIndex;
        }
    }
}
