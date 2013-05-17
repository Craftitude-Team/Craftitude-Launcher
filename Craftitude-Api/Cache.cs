using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using YaTools.Yaml;
using Raven;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Storage;
using Raven.Database;

namespace Craftitude
{
    public class Cache
    {
        // TODO: Add operations and complete code for them

        internal DocumentStore _store;
        internal Client _client;

        internal Cache(Client client, string folderPath)
        {
            this._client = client;

            // RavenDB connection to local cache database
            this._store = new EmbeddableDocumentStore()
            {
                DataDirectory = folderPath,
                DefaultDatabase = "Craftitude_Installation"
            };
            _store.Initialize();
        }

        private IDocumentSession GetSessionForInstallationCache()
        {
            return _store.OpenSession();
        }
        private IAsyncDocumentSession GetAsyncSessionForInstallationCache()
        {
            return _store.OpenAsyncSession();
        }
        private IDocumentSession GetSessionForRepository(Uri repositoryUri)
        {
            return _store.OpenSession("Craftitude_Repository_" + GenerateCacheId(repositoryUri.ToString()));
        }
        private IAsyncDocumentSession GetAsyncSessionForRepository(Uri repositoryUri)
        {
            return _store.OpenAsyncSession("Craftitude_Repository_" + GenerateCacheId(repositoryUri.ToString()));
        }
        
        /// <summary>
        /// Generates a unique ID for a repository's URL.
        /// </summary>
        /// <param name="url">The repository URL</param>
        /// <returns>A unique hash of the repository URL</returns>
        private static string GenerateCacheId(string url)
        {
            return BitConverter.ToString(SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(url))).Replace("-", "\\");
        }
    }
}
