using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Raven;
using Raven.Abstractions;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Extensions;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Database;


namespace Craftitude
{
    public class Repository
    {
        EmbeddableDocumentStore _cacheStore;
        DocumentStore _onlineStore;

        protected Guid GetGuidForUri(Uri uri)
        {
            return GetGuidForUri(uri.ToString());
        }
        protected Guid GetGuidForUri(string uri)
        {
            // A usable index name for RavenDB
            var uriindex = BitConverter.ToString(Encoding.UTF8.GetBytes(uri)).Replace("-", "");

            // Look if guid is already available for this uri
            using (var session = _cacheStore.OpenSession("guidindex"))
            {
                var q = session.Query<Guid>(uriindex);
                if (q.Any())
                    return q.Single();
                else
                {
                    var guid = new Guid();
                    session.Store(guid, uriindex);
                    session.SaveChanges();
                    return guid;
                }
            }
        }

        public Repository(Uri storeUri, string cachePath)
        {
            _cacheStore = new EmbeddableDocumentStore()
            {
                DataDirectory = cachePath,
                DefaultDatabase = GetGuidForUri(storeUri).ToString()
            };
            _cacheStore.Initialize();
            var existantDatabases = _cacheStore.DatabaseCommands.GetDatabaseNames(8);

            if (!existantDatabases.Contains("installation"))
                _cacheStore.DatabaseCommands.CreateDatabase(new DatabaseDocument()
                {
                    Id = "installation",
                    Settings = {
                            { "Raven/ActiveBundles", "Compression" }
                        }
                });
        }
    }
}
