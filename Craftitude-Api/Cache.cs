#region Imports (13)

using Raven;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Database;
using Raven.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

#endregion Imports (13)

namespace Craftitude
{


    public class Cache
    {
        internal Client _client;
        internal DocumentStore _store;

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

        /// <summary>
        /// Generates a unique ID for a repository's URL.
        /// </summary>
        /// <param name="url">The repository URL</param>
        /// <returns>A unique hash of the repository URL</returns>
        private static string GenerateCacheId(string url)
        {
            return BitConverter.ToString(SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(url))).Replace("-", "\\");
        }

        private IAsyncDocumentSession GetAsyncSessionForInstallationCache()
        {
            return _store.OpenAsyncSession();
        }

        private IAsyncDocumentSession GetAsyncSessionForRepository(Uri repositoryUri)
        {
            return _store.OpenAsyncSession("Craftitude_Repository_" + GenerateCacheId(repositoryUri.ToString()));
        }

        private IDocumentSession GetSessionForInstallationCache()
        {
            return _store.OpenSession();
        }

        private IDocumentSession GetSessionForRepository(Uri repositoryUri)
        {
            return _store.OpenSession("Craftitude_Repository_" + GenerateCacheId(repositoryUri.ToString()));
        }
    }
}
