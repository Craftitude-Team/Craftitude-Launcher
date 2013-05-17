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

        public string BasePath { get; private set; }

        public Cache Cache { get; private set; }

        public ObservableCollection<Operation> Operations { get; private set; }

        public List<DocumentStore> Repositories { get; private set; }

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
            this.Repositories = new List<Repository>();
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
                            this.Repositories.AddRange(Repository.FromConfigString(line, this));
                    }
                }
            }

            // Operations collection
            this.Operations = new ObservableCollection<Operation>();
        }

        public void AddRepository(string repositoryUrl)
        {
            AddRepository(new Uri(repositoryUrl));
        }

        public void AddRepository(Uri repositoryUri)
        {
            string databaseName = string.Empty;
            repositoryUri = GetHttpFromCrepUrl(repositoryUri, out databaseName);

            if (Repositories.Where(r => r.Url == repositoryUri.ToString() && r.DefaultDatabase == databaseName).Any())
                return; // already added

            var document = new DocumentStore();
            document.Url = repositoryUri.ToString();
            document.DefaultDatabase = databaseName;
            document.Initialize();

            Repositories.Add(document);
        }

        public void DeleteRepository(Uri repositoryUri)
        {
            string databaseName = string.Empty;
            repositoryUri = GetHttpFromCrepUrl(repositoryUri, out databaseName);

            // Dispose connection
            foreach (var r in Repositories.Where(r => r.Url == repositoryUri.ToString() && r.DefaultDatabase == databaseName))
                r.Dispose();

            // Remove connection
            Repositories.RemoveAll(r => r.Url == repositoryUri.ToString() && r.DefaultDatabase == databaseName);
        }

        public IEnumerable<PackageInfo> GetRemotePackageInfo(string packageID)
        {
            foreach (var repository in Repositories)
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

        internal int GetOperationID()
        {
            int newOperationIndex = nextOperationIndexToAssign;
            nextOperationIndexToAssign++;
            return newOperationIndex;
        }
    }
}
