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
    public class RepositoryCache : IDisposable
    {
        internal EmbeddableDocumentStore _db;
        internal Cache _cache;

        internal RepositoryCache(Cache cache, string cacheId)
        {
            _cache = cache;
            _db = new EmbeddableDocumentStore()
            {
                DataDirectory = Path.Combine(_cache._path, cacheId)
            };
            if (!Directory.Exists(_db.DataDirectory))
                Directory.CreateDirectory(_db.DataDirectory);
            _db.Initialize();
        }

        public void Dispose()
        {
            if (!_db.WasDisposed)
                _db.Dispose();
        }

        public void DeletePackage(string id, Package package)
        {
            using (var session = _db.OpenSession())
            {
                session.Delete(package);
                session.SaveChanges();
            }
        }

        public void SavePackage(string id, Package package)
        {
            using (var session = _db.OpenSession())
            {
                session.Store(package, id);
                session.SaveChanges();
            }
        }

        public Package[] GetPackages(params string[] IDs)
        {
            using (var session = _db.OpenSession())
            {
                return session.Load<Package>(IDs);
            }
        }

        public Package GetPackage(string ID)
        {
            using (var session = _db.OpenSession())
            {
                return session.Load<Package>(ID);
            }
        }
    }
}
